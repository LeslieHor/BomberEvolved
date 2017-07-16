using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionScript : MonoBehaviour {
    private bool moving;
    private Vector3 prevPosition;
    private Vector3 prevSupportPosition;

    // Use this for initialization
    void Start() {
        moving = false;
        prevPosition = transform.position;
        prevSupportPosition = Vector3.zero;
        StartCoroutine(FallingCheck());
    }

    // Update is called once per frame
    void Update() {
        // Check if the object is being supported
        
    }

    IEnumerator FallingCheck() {
        while (true) {
            GetComponent<Renderer>().material.color = Color.white;
            Vector3 boxColliderSize = transform.localScale / 2.1f;
            Collider[] hitColliders = Physics.OverlapBox(transform.position - new Vector3(0f, 0.1f, 0f), boxColliderSize);
            if (hitColliders.Length == 1 && HighSupportPositionDifference()) {
                //GetComponent<Renderer>().material.color = Color.green;
                StartMovementSimulation();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool HighSupportPositionDifference() {
        float positionChangeSize = Vector3.Distance(prevSupportPosition, transform.position);
        prevSupportPosition = transform.position;
        if (positionChangeSize > 0.1f) {
            return true;
        }
        else {
            return false;
        }
    }

    public void OnCollisionEnter(Collision other) {
        if (other.impulse.magnitude > 0.2f) {
            StartMovementSimulation();
        }
    }

    public void StartMovementSimulation() {
        GetComponent<Rigidbody>().isKinematic = false;

        if (moving == false) {
            moving = true;
            StartCoroutine(FreezeCountdownCheck());
        }
    }

    IEnumerator FreezeCountdownCheck() {
        yield return new WaitForSeconds(1f);

        while (HighPositionDifference()) {
            yield return new WaitForSeconds(2f);
        }

        GetComponent<Rigidbody>().isKinematic = true;
        moving = false;
    }

    public bool HighPositionDifference() {
        float positionChangeSize = Vector3.Distance(prevPosition, transform.position);
        prevPosition = transform.position;
        if (positionChangeSize > 0.1f) {
            return true;
        }
        else {
            return false;
        }
    }
}
