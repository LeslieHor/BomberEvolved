using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Time.timeScale = 0.2f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Time.timeScale = 1.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Time.timeScale = 5.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Time.timeScale = 10.0f;
        }
    }

    
}
