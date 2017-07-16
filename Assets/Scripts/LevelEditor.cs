using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelEditor : MonoBehaviour {
    public GameObject cursor;
    public bool gridLock;
    public GameObject LevelEditorInterface;
    public GameObject VerticalBeam;
    public GameObject HorizontalBeam;
    public GameObject HorizontalBrick;
    public GameObject Foundation;
    public Material GhostMaterial;
    public GameObject LevelStorage;

    private Vector3 prevMousePos;
    private Vector3 newMousePos;
    private Quaternion globalRotation;

    private GameObject GhostObject;

    public UnityEngine.UI.Text debugMousePos;
    public Level levelMap;

    public EventSystem eventSystem;

    public class Level {
        public List<Structure> objects;
        public Level() {
            objects = new List<Structure>();
        }

        public void AddObject(Structure s) {
            objects.Add(s);
        }

        public void SpawnAll() {
            foreach (Structure s in objects) {
                s.Spawn();
            }
        }

        public void Delete(GameObject obj) {
            Structure structureToDelete = null;
            foreach (Structure s in objects) {
                if (obj.transform.position == s.position) {
                    structureToDelete = s;
                }
            }
            objects.Remove(structureToDelete);
        }

        public void DeleteAll() {
            objects = new List<Structure>();
        }
    }

    public class Structure {
        public GameObject structure;
        public Vector3 position;
        public Quaternion rotation;

        public Structure(GameObject s, Vector3 p, Quaternion r) {
            structure = s;
            position = p;
            rotation = r;
        }

        public void Spawn() {
            Instantiate(structure, position, rotation);
        }
    }

    // Use this for initialization
    void Start() {
        Time.timeScale = 0f;
        levelMap = new Level();
        globalRotation = Quaternion.LookRotation(Vector3.forward);
    }

    // Update is called once per frame
    void Update() {
        Destroy(GhostObject);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            newMousePos = hit.point;
        }

        if (gridLock) {
            float x = (Mathf.RoundToInt(2 * newMousePos.x)) / 2.0f;
            //float y = newMousePos.y;
            float y = (Mathf.RoundToInt(2 * newMousePos.y)) / 2.0f;
            float z = (Mathf.RoundToInt(2 * newMousePos.z)) / 2.0f;

            newMousePos = new Vector3(x, y, z);
        }

        cursor.transform.position = newMousePos;

        if (Input.GetKeyDown(KeyCode.Period)) {
            globalRotation *= Quaternion.Euler(new Vector3(0f, 90f, 0f));
        }
        else if (Input.GetKeyDown(KeyCode.Comma)) {
            globalRotation *= Quaternion.Euler(new Vector3(0f, -90f, 0f));
        }

            Vector3 objectPosition = newMousePos;
        switch (LevelEditorInterface.GetComponent<LevelEditorInterface>().StructureSelector.captionText.text) {
            case ("Foundation"):
                objectPosition += globalRotation * new Vector3(0f, 0.5f, 0f);
                GhostObject = CreateGhostStructure(Foundation, objectPosition, globalRotation);

                if (!eventSystem.IsPointerOverGameObject()) {
                    if (Input.GetMouseButtonUp(0)) {
                        Structure newObject = new Structure(Foundation, objectPosition, globalRotation);
                        levelMap.AddObject(newObject);
                        newObject.Spawn();
                    }
                }
                break;
            case ("Vertical Beam"):
                objectPosition += globalRotation * new Vector3(0f, 1f, 0f);
                GhostObject = CreateGhostStructure(VerticalBeam, objectPosition, globalRotation);

                if (!eventSystem.IsPointerOverGameObject()) {
                    if (Input.GetMouseButtonUp(0)) {
                        Structure newObject = new Structure(VerticalBeam, objectPosition, globalRotation);
                        levelMap.AddObject(newObject);
                        newObject.Spawn();
                    }
                }    
                break;
            case ("Horizontal Beam (Right)"):
                objectPosition += globalRotation * new Vector3(0f, 0.25f, 0.75f);
                GhostObject = CreateGhostStructure(HorizontalBeam, objectPosition, globalRotation);

                if (!eventSystem.IsPointerOverGameObject()) {
                    if (Input.GetMouseButtonUp(0)) {
                        Structure newObject = new Structure(HorizontalBeam, objectPosition, globalRotation);
                        levelMap.AddObject(newObject);
                        newObject.Spawn();
                    }
                }
                break;
            case ("Horizontal Beam (Left)"):
                objectPosition += globalRotation * new Vector3(0f, 0.25f, -0.75f);
                GhostObject = CreateGhostStructure(HorizontalBeam, objectPosition, globalRotation);

                if (!eventSystem.IsPointerOverGameObject()) {
                    if (Input.GetMouseButtonUp(0)) {
                        Structure newObject = new Structure(HorizontalBeam, objectPosition, globalRotation);
                        levelMap.AddObject(newObject);
                        newObject.Spawn();
                    }
                }
                break;
            case ("Horizontal Brick (Right)"):
                objectPosition += globalRotation * new Vector3(0f, 0.25f, 0.25f);
                GhostObject = CreateGhostStructure(HorizontalBrick, objectPosition, globalRotation);

                if (!eventSystem.IsPointerOverGameObject()) {
                    if (Input.GetMouseButtonUp(0)) {
                        Structure newObject = new Structure(HorizontalBrick, objectPosition, globalRotation);
                        levelMap.AddObject(newObject);
                        newObject.Spawn();
                    }
                }
                break;
            case ("Horizontal Brick (Left)"):
                objectPosition += globalRotation * new Vector3(0f, 0.25f, -0.25f);
                GhostObject = CreateGhostStructure(HorizontalBrick, objectPosition, globalRotation);

                if (!eventSystem.IsPointerOverGameObject()) {
                    if (Input.GetMouseButtonUp(0)) {
                        Structure newObject = new Structure(HorizontalBrick, objectPosition, globalRotation);
                        levelMap.AddObject(newObject);
                        newObject.Spawn();
                    }
                }
                break;
            case ("Delete"):
                if (!eventSystem.IsPointerOverGameObject()) {
                    if (Input.GetMouseButtonUp(0)) {
                        if (hit.transform.gameObject.tag == "Structure" || hit.transform.gameObject.tag == "Foundation") {
                            levelMap.Delete(hit.transform.gameObject);
                            Destroy(hit.transform.gameObject);
                        }
                    }
                }
                break;
        }

        prevMousePos = newMousePos;
        debugMousePos.text = newMousePos.ToString();
    }

    public GameObject CreateGhostStructure(GameObject structure, Vector3 position, Quaternion rotation) {
        GameObject ghostObject = (GameObject)Instantiate(structure, position, rotation);
        ghostObject.layer = 2;
        ghostObject.GetComponent<Renderer>().material = GhostMaterial;
        ghostObject.GetComponent<BoxCollider>().isTrigger = true;

        return ghostObject;
    }
    
    public void Simulate() {
        Time.timeScale = 1f;
    }

    public void Reset() {
        Time.timeScale = 0f;
        foreach (GameObject s in GameObject.FindGameObjectsWithTag("Structure")) {
            Destroy(s);
        }

        levelMap.SpawnAll();
    }

    public void Play() {
        LevelStorage.GetComponent<LevelStorage>().level = levelMap;
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

}