using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserControls : MonoBehaviour {
    public float turboSpeed;
    public InputField GenerationInput;
    public Text GenerationCounter;
    public Text IndividualCounter;
    public Text GeneOutput;
    public Text ScoreChart;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SkipThisAttempt() {
        GameObject.Find("Controller").GetComponent<ControllerScript>().EndAttempt();
    }

    public void Turbo() {
        string generationInput = GenerationInput.text;
        int generationSkip = 0;
        if (generationInput.Length > 0) {
            generationSkip = int.Parse(generationInput);
        }

        int generationToSkipTo = GameObject.Find("Controller").GetComponent<ControllerScript>().currentGeneration + generationSkip;
        StartCoroutine(TurboSkip(generationToSkipTo));
    }

    IEnumerator TurboSkip(int generationToSkipTo) {
        Camera main = Camera.main;
        main.enabled = false;
        float timea = Time.timeSinceLevelLoad;

        Time.timeScale = turboSpeed;

        while (GameObject.Find("Controller").GetComponent<ControllerScript>().currentGeneration < generationToSkipTo) {
            yield return null;
        }

        Time.timeScale = 1f;
        main.enabled = true;
        float timeb = Time.timeSinceLevelLoad;
        Debug.Log("Time for turbo: " + (timeb - timea));
    }

    public void UpdateGenerationCounter(int g) {
        GenerationCounter.text = g.ToString();
    }

    public void UpdateIndividualCounter(int i) {
        IndividualCounter.text = i.ToString();
    }

    public void DisplayGeneticCode(ControllerScript.BomberGroup bomberGroup) {
        string s = "";
        foreach (ControllerScript.Bomber bomber in bomberGroup.bombers) {
            s += "T: " + bomber.target.ToString() + "; F: " + bomber.launchForce.ToString() + "; T: " + bomber.waitTime.ToString() + "\n";
        }
        GeneOutput.text = s;
    }

    public void UpdateScore(int individual, float score) {
        ScoreChart.text += individual.ToString() + ": " + score.ToString() + "\n";
    }

    public void ClearScoreChart() {
        ScoreChart.text = "";
    }
}
