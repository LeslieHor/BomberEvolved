using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BomberScript : MonoBehaviour {
    public GameObject explosionPrefab;
    public float explosionForce;
    public float explosionRadius;

    public Vector3 target;
    public float launchForce;
    public float waitTime;


    public bool exploded;

    public void Run(Vector3 tar, float f, float t) {
        target = tar;
        exploded = false;
        launchForce = f;
        waitTime = t;
        
        // TEST STUFF
        //target = new Vector3(1f, Random.Range(0.2f, 0.4f), 0f);
        //launchForce = 1000f;
        // END OF TEST STUFF

        StartCoroutine(Launch());
        StartCoroutine(TimeLimit());
    }

    IEnumerator Launch() {
        yield return new WaitForSeconds(waitTime);
        GetComponent<SphereCollider>().isTrigger = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().AddForce(target * launchForce);
    }

    IEnumerator TimeLimit() {
        yield return new WaitForSeconds(5f);

        Explode();
    }

    private void Explode() {
        if (exploded) {
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        int i = 0;
        while (i < hitColliders.Length) {
            if (hitColliders[i].tag == "Structure") {
                hitColliders[i].GetComponent<DestructionScript>().StartMovementSimulation();
                hitColliders[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
            i++;
        }

        exploded = true;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<SphereCollider>().isTrigger = true;

        GameObject explosion = (GameObject)Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity);
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().duration / 2.1f);
    }

    public void OnCollisionEnter(Collision other) {
        //if (other.gameObject.tag == "Structure"){
        Explode();
        //}
    }
}
