using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SaveLoad : MonoBehaviour {
    public GameObject LevelEditorObj;
    public GameObject StructurePiecesObj;

    private string saveDir = "Saves\\";
    private string saveExt = ".map";
    private string splitter = "\n";
    private string delimiter = ",";

    public Dropdown loadDropdown;
    public InputField saveNameField;
    public GameObject menu;

    // Use this for initialization
    void Start () {
        PopulateLoadDropdown();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PopulateLoadDropdown() {
        loadDropdown.ClearOptions();

        string[] files = System.IO.Directory.GetFiles(saveDir);

        List<string> saveFilenames = new List<string>();

        foreach (string file in files) {
            string filename = System.IO.Path.GetFileNameWithoutExtension(file);
            saveFilenames.Add(filename);
        }

        loadDropdown.AddOptions(saveFilenames);
    }

    public void Save() {
        string filename = saveNameField.text;

        LevelEditor.Level level = LevelEditorObj.GetComponent<LevelEditor>().levelMap;

        string saveString = "";
        foreach (LevelEditor.Structure s in level.objects) {
            saveString += StructurePiecesObj.GetComponent<StructurePieces>().StructureDictionaryObject[s.structure] + delimiter;
            saveString += s.position.x.ToString() + delimiter + s.position.y.ToString() + delimiter + s.position.z.ToString() + delimiter;
            saveString += s.rotation.eulerAngles.x + delimiter + s.rotation.eulerAngles.y + delimiter + s.rotation.eulerAngles.z + splitter;
        }

        System.IO.FileInfo file = new System.IO.FileInfo(saveDir + filename + saveExt);
        file.Directory.Create(); // If the directory already exists, this method does nothing.
        System.IO.File.WriteAllText(file.FullName, saveString);
    }

    public void Load() {
        string filename = loadDropdown.captionText.text;
        string saveString = System.IO.File.ReadAllText(saveDir + filename + saveExt);
        string[] saveParts = Regex.Split(saveString, splitter);

        LevelEditorObj.GetComponent<LevelEditor>().levelMap.DeleteAll();

        foreach (string structureString in saveParts) {
            if (structureString.Length > 0) {
                string[] s = Regex.Split(structureString, delimiter);
                float x = float.Parse(s[1]);
                float y = float.Parse(s[2]);
                float z = float.Parse(s[3]);

                Vector3 position = new Vector3(x, y, z);

                x = float.Parse(s[4]);
                y = float.Parse(s[5]);
                z = float.Parse(s[6]);

                Quaternion rotation = Quaternion.Euler(new Vector3(x, y, z));

                GameObject structureObj = null;

                structureObj = StructurePiecesObj.GetComponent<StructurePieces>().StructureDictionaryString[s[0]];

                LevelEditor.Structure structure = new LevelEditor.Structure(structureObj, position, rotation);

                LevelEditorObj.GetComponent<LevelEditor>().levelMap.AddObject(structure);
            }
        }

        LevelEditorObj.GetComponent<LevelEditor>().Reset();
    }

    public void ToggleSaveLoadMenu() {
        menu.SetActive(!menu.activeSelf);
        if (menu.activeSelf) {
            PopulateLoadDropdown();
        }
    }
}
