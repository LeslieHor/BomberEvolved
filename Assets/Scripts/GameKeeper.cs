using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameKeeper : MonoBehaviour {
    public float minimumHeight;
    public GameObject[] structureObjects;

	// Use this for initialization
	void Start () {
        structureObjects = new GameObject[0];
        ResetLevel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetScore() {
        float score = 0f;
        foreach (GameObject structurePiece in GameObject.FindGameObjectsWithTag("Structure")) {
            if (structurePiece.transform.position.y <= minimumHeight) {
                score++;
            }
        }
        return score;
    }

    public void LoadLevel() {
        GameObject.Find("LevelStorage").GetComponent<LevelStorage>().level.SpawnAll();
    }

    public void ResetLevel() {
        foreach (GameObject structurePiece in GameObject.FindGameObjectsWithTag("Structure")) {
            Destroy(structurePiece);
        }
        foreach (GameObject structurePiece in GameObject.FindGameObjectsWithTag("Foundation")) {
            Destroy(structurePiece);
        }
        LoadLevel();
    }
}
