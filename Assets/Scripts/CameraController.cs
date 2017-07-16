using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float panSpeed;
    public float rotateSpeed;
    public float zoomSpeed;

    public bool panning;
    public Vector3 prevMousePos;
    public Vector3 newMousePos;

    // Use this for initialization
    void Start() {
        panning = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) {
                newMousePos = hit.point;
            }
            
            if (panning) {
                Vector3 translateVector = prevMousePos - newMousePos;
                Camera.main.transform.position += translateVector;
                prevMousePos = newMousePos;
            }
            prevMousePos = newMousePos;
            panning = true;
        }

        if (Input.GetMouseButtonUp(1)) {
            panning = false;
        }

        // Pan camera
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(new Vector3(panSpeed * (1f / 60f), 0, 0));
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(new Vector3(-panSpeed * (1f / 60f), 0, 0));
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(new Vector3(0, -panSpeed * (1f / 60f), 0));
        }
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(new Vector3(0, panSpeed * (1f / 60f), 0));
        }

        // Rotate camera
        if (Input.GetKey(KeyCode.Q)) {
            transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * (1f / 60f));
        }
        if (Input.GetKey(KeyCode.E)) {
            transform.RotateAround(transform.position, Vector3.up, rotateSpeed * (1f / 60f));
        }
        if (Input.GetKey(KeyCode.R)) {
            transform.RotateAround(transform.position, transform.right, -rotateSpeed * (1f / 60f));
        }
        if (Input.GetKey(KeyCode.F)) {
            transform.RotateAround(transform.position, transform.right, rotateSpeed * (1f / 60f));
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            transform.Translate(new Vector3(0, 0, zoomSpeed * (1f / 60f)));
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            transform.Translate(new Vector3(0, 0, -zoomSpeed * (1f / 60f)));
        }
    }


}
