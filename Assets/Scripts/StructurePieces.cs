using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePieces : MonoBehaviour {
    public Dictionary<string, GameObject> StructureDictionaryString;
    public Dictionary<GameObject, string> StructureDictionaryObject;

    public GameObject VerticalBeam;
    public GameObject HorizontalBeam;
    public GameObject HorizontalBrick;
    public GameObject Foundation;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
        StructureDictionaryString = new Dictionary<string, GameObject>();
        StructureDictionaryString["Vertical Beam"] = VerticalBeam;
        StructureDictionaryString["Horizontal Beam"] = HorizontalBeam;
        StructureDictionaryString["Horizontal Brick"] = HorizontalBrick;
        StructureDictionaryString["Foundation"] = Foundation;

        StructureDictionaryObject = new Dictionary<GameObject, string>();
        foreach (KeyValuePair<string, GameObject> s in StructureDictionaryString) {
            StructureDictionaryObject[s.Value] = s.Key;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
