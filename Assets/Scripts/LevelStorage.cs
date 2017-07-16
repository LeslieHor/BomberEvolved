using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStorage : MonoBehaviour {
    public LevelEditor.Level level;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
