using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour {
    [Header("Config")]
    public GameObject bomberPrefab;
    public int currentGeneration;
    private List<BomberGroup> population;

    [Header("Timeout")]
    public float timeLimit;

    [Header("Evolution Settings")]
    public SelectionMethod selectionMethod;
    public enum SelectionMethod {
        Tournament,
        Roulette
    }
    public float directionMu;
    public float directionSigma;
    public float launchForceMu;
    public float launchForceSigma;
    public float waitTimeMu;
    public float waitTimeSigma;
    public float mutationDirectionMu;
    public float mutationDirectionSigma;
    public float mutationLaunchForceMu;
    public float mutationLaunchForceSigma;
    public float mutationWaitTimeMu;
    public float mutationWaitTimeSigma;
    public int individualsPerGeneration;
    public float mutationRate;
    public float mutationGeneSizeIncreaseRate;
    public float crossoverRate;
    public float randomisationChance;
    [Range(1, 20)]
    public int startGeneSize;
    
    private int currentBomberIndex;
    private bool currentBomberEvaluating;

    private float bomberEvaluationStartTime;

    public class Bomber {
        public Vector3 target;
        public float launchForce;
        public float waitTime;

        public Bomber(float x, float y, float z, float f, float t) {
            target = Vector3.Normalize(new Vector3(x, y, z));
            launchForce = f;
            waitTime = t;
        }
    }

    public class BomberGroup {
        public List<Bomber> bombers;
        public float score;
        public int geneSize;

        public BomberGroup(int gs) {
            bombers = new List<Bomber>();
            score = 0f;
            geneSize = gs;
        }

        public void AddBomber(Bomber bomber) {
            bombers.Add(bomber);
        }

        public void RandomiseGeneOrder() {
            for (int i = 0; i < bombers.Count; i++) {
                Bomber temp = bombers[i];
                int randomindex = Random.Range(i, bombers.Count);
                bombers[i] = bombers[randomindex];
                bombers[randomindex] = temp;
            }
        }
    }

    // Use this for initialization
    void Start() {
        population = new List<BomberGroup>();
        currentBomberIndex = 0;
        currentBomberEvaluating = false;
        RandomPopulation();
        SpawnBomberGroup();
    }

    // Update is called once per frame
    void Update() {
        if (AllBombersExploded()) {
            if (currentBomberEvaluating == false) {
                // Wait a bit for the pieces to fall
                currentBomberEvaluating = true;
                StartCoroutine(GameEnd());
            }
        }
    }

    private bool AllBombersExploded() {
        if ((GameObject.FindGameObjectsWithTag("Bomber")).Length > 0) {
            foreach (GameObject bomber in GameObject.FindGameObjectsWithTag("Bomber")) {
                if (bomber.GetComponent<BomberScript>().exploded == false) {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CheckForMovingStructure() {
        foreach (GameObject structureObj in GameObject.FindGameObjectsWithTag("Structure")) {
            if (structureObj.GetComponent<Rigidbody>().IsSleeping() == false) {
                return true;
            }
        }
        return false;
    }

    IEnumerator GameEnd() {
        int tempCurrentBomberIndex = currentBomberIndex; // Need to make sure we don't skip the next bomber if the user presses skip after the structure has come to a rest

        while (CheckForMovingStructure() == true && Time.time < bomberEvaluationStartTime + timeLimit) {
            yield return null;
        }
        yield return new WaitForSeconds(3f);

        if (currentBomberIndex == tempCurrentBomberIndex) {
            EndAttempt();
        }
    }

    public void EndAttempt() {
        string debugString = "";

        population[currentBomberIndex].score = GameObject.Find("Controller").GetComponent<GameKeeper>().GetScore();

        foreach (GameObject bomber in GameObject.FindGameObjectsWithTag("Bomber")) {
            Destroy(bomber);
        }

        debugString += "GI: " + currentGeneration + ":" + currentBomberIndex;
        debugString += " - Score " + population[currentBomberIndex].score;

        GameObject.Find("UserInterface").GetComponent<UserControls>().UpdateScore(currentBomberIndex, population[currentBomberIndex].score);

        GameObject.Find("Controller").GetComponent<GameKeeper>().ResetLevel();

        // Check if we're at the last individual for that generation
        if (currentBomberIndex == individualsPerGeneration - 1) {
            // If we are, cull the weak and breed the strong
            currentBomberIndex = 0;
            currentGeneration++;
            GameObject.Find("UserInterface").GetComponent<UserControls>().UpdateGenerationCounter(currentGeneration);
            GameObject.Find("UserInterface").GetComponent<UserControls>().ClearScoreChart();
            BreedNextGeneration();
        }
        else {
            // otherwise, just load the next individual
            currentBomberIndex++;
        }

        SpawnBomberGroup();
        currentBomberEvaluating = false;
        
        GameObject.Find("UserInterface").GetComponent<UserControls>().UpdateIndividualCounter(currentBomberIndex);

        //Debug.Log(debugString);
    }

    private void NewPopulationIfNoSuccesses() {
        foreach (BomberGroup bomberGroup in population) {
            if (bomberGroup.score > 0) {
                return;
            }
        }
        population = new List<BomberGroup>();
        RandomPopulation();
    }

    private void BreedNextGeneration() {
        switch (selectionMethod) {
            case SelectionMethod.Tournament:
                // Start of tournament selection
                NewPopulationIfNoSuccesses();

                List<BomberGroup> nextPopulation = new List<BomberGroup>();
                for (int i = 0; i < individualsPerGeneration; i++) {
                    BomberGroup bomberGroup1 = population[Random.Range(0, population.Count - 1)];
                    BomberGroup bomberGroup2 = population[Random.Range(0, population.Count - 1)];

                    BomberGroup bomberGroupToAdd = bomberGroup1;
                    if (bomberGroup2.score > bomberGroup1.score) {
                        bomberGroupToAdd = bomberGroup2;
                    }

                    nextPopulation.Add(CopyBomberGroup(bomberGroupToAdd));
                }
                population = nextPopulation;
                // End of tourament selection
                break;
            case SelectionMethod.Roulette:
                // Start of roulette selection
                List<BomberGroup> weightedPopulation = new List<BomberGroup>();
                foreach (BomberGroup bomberGroup in population) {
                    for (int i = 0; i < bomberGroup.score; i++) {
                        weightedPopulation.Add(bomberGroup);
                    }
                }

                if (weightedPopulation.Count > 0) {
                    population = new List<BomberGroup>();
                    for (int i = 0; i < individualsPerGeneration; i++) {
                        BomberGroup bomberGroup = weightedPopulation[Random.Range(0, weightedPopulation.Count - 1)];
                        population.Add(CopyBomberGroup(bomberGroup));
                    }
                }

                NewPopulationIfNoSuccesses();
                // End of roulette selection
                break;
        }


        foreach (BomberGroup bomberGroup in population) {
            if (Random.Range(0f, 1f) < randomisationChance) {
                bomberGroup.RandomiseGeneOrder();
            }
        }

        // Mutation time
        foreach (BomberGroup bomberGroup in population) {
            if (Random.Range(0f, 1f) < mutationRate) {
                foreach (Bomber bomber in bomberGroup.bombers) {
                    float x = bomber.target.x + GenerateNormalRandom(mutationDirectionMu, mutationDirectionSigma);
                    float y = bomber.target.y + GenerateNormalRandom(mutationDirectionMu, mutationDirectionSigma);
                    float z = bomber.target.z + GenerateNormalRandom(mutationDirectionMu, mutationDirectionSigma);
                    float f = bomber.launchForce + GenerateNormalRandom(mutationLaunchForceMu, mutationLaunchForceSigma);
                    float t = bomber.waitTime + GenerateNormalRandom(mutationWaitTimeMu, mutationWaitTimeSigma);

                    Vector3 newTarget = new Vector3(x, y, z);
                    bomber.target = newTarget;
                    bomber.launchForce = f;
                    bomber.waitTime = t;
                }
            }

            if (Random.Range(0f, 1f) < mutationGeneSizeIncreaseRate) {
                bomberGroup.geneSize += 1;
                bomberGroup.AddBomber(CreateRandomBomber());
            }
        }

        // Crossover time
        foreach (BomberGroup bomberGroup1 in population) {
            if (Random.Range(0f, 1f) < crossoverRate) {
                BomberGroup bomberGroup2 = population[Random.Range(0, population.Count - 1)];

                // Uniform Crossover
                //BomberGroup secondBomberGroup = population[Random.Range(0, population.Count - 1)];
                //for (int i = 0; i < bomberGroup.bombers.Count; i++) {
                //    if (Random.Range(0f, 1f) < 0.5f) {
                //        Bomber tempBomber = bomberGroup.bombers[i];

                //        bomberGroup.bombers[i] = secondBomberGroup.bombers[i];
                //        secondBomberGroup.bombers[i] = tempBomber;
                //    }
                //}

                // TODO NOT QUITE RIGHT. NEED TO PUT THE EXTRA GENES IN THE RIGHT PLACE
                // One-point Crossover
                int crossoverPoint = Random.Range(0, Mathf.Min(bomberGroup1.geneSize, bomberGroup2.geneSize) - 1);
                BomberGroup bomberGroup1Copy = CopyBomberGroup(bomberGroup1);
                for (int i = 0; i < Mathf.Min(bomberGroup1.geneSize, bomberGroup2.geneSize); i++) {
                    if (i >= crossoverPoint) {
                        bomberGroup1.bombers[i] = bomberGroup2.bombers[i];
                        bomberGroup2.bombers[i] = bomberGroup1Copy.bombers[i];
                    }
                }
            }
        }
    }

    private BomberGroup CopyBomberGroup(BomberGroup bomberGroup) {
        BomberGroup newBomberGroup = new BomberGroup(bomberGroup.geneSize);
        foreach (Bomber b in bomberGroup.bombers) {
            Bomber newBomber = new Bomber(b.target.x, b.target.y, b.target.z, b.launchForce, b.waitTime);
            newBomberGroup.AddBomber(newBomber);
        }
        return newBomberGroup;
    }

    private void RandomPopulation() {
        for (int i = 0; i < individualsPerGeneration; i++) {
            BomberGroup newBomberGroup = new BomberGroup(startGeneSize);

            for (int j = 0; j < startGeneSize; j++) {
                Bomber newBomber = CreateRandomBomber();
                newBomberGroup.AddBomber(newBomber);
            }

            population.Add(newBomberGroup);
        }
    }

    private Bomber CreateRandomBomber() {
        float x = Random.Range(0.5f, 1.5f);
        float y = Random.Range(0f, 1f);
        float z = Random.Range(-1f, 1f);

        float f = GenerateNormalRandom(launchForceMu, launchForceSigma);
        f = Mathf.Abs(f);

        float t = GenerateNormalRandom(waitTimeMu, waitTimeSigma);
        t = Mathf.Abs(t);

        Bomber newBomber = new Bomber(x, y, z, f, t);

        return newBomber;
    }

    public void SpawnBomberGroup() {
        BomberGroup bomberGroup = population[currentBomberIndex];

        foreach (Bomber bomber in bomberGroup.bombers) {
            GameObject newBomber = (GameObject)Instantiate(bomberPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity);
            newBomber.GetComponent<BomberScript>().Run(bomber.target, bomber.launchForce, bomber.waitTime);
        }

        bomberEvaluationStartTime = Time.time;
        GameObject.Find("UserInterface").GetComponent<UserControls>().DisplayGeneticCode(bomberGroup);
    }

    public float GenerateNormalRandom(float mu, float sigma) {
        float rand1 = Random.Range(0.0f, 1.0f);
        float rand2 = Random.Range(0.0f, 1.0f);

        float n = Mathf.Sqrt(-2.0f * Mathf.Log(rand1)) * Mathf.Cos((2.0f * Mathf.PI) * rand2);

        return (mu + sigma * n);
    }

    // TODO: Calculate how similar two bomber groups are
    public float CalculateSimilarity(BomberGroup bomberGroup1, BomberGroup bomberGroup2) {
        float similarity = 0f;

        for (int i = 0; i < bomberGroup1.bombers.Count; i++) {

        }

        return 0f;
    }
}
