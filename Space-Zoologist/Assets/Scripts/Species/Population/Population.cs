using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A runtime instance of a population.
/// </summary>
public class Population : MonoBehaviour, Life
{
    public AnimalSpecies Species { get => species; }
    public int Count { get => this.AnimalPopulation.Count; }
    public float Dominance => Count * species.Dominance;
    public int PrePopulationCount => this.prePopulationCount;
    [HideInInspector]
    public bool HasAccessibilityChanged = false;
    public System.Random random = new System.Random();

    public Dictionary<string, Need> Needs => needs;
    public Dictionary<Need, Dictionary<NeedCondition, SpecieBehaviorTrigger>> NeedBehaviors => needBehaviors;
    public AnimalPathfinding.Grid grid { get; private set; }
    public List<Vector3Int>  AccessibleLocations { get; private set; }

    [SerializeField] public GridSystem TestingBehaviors = default;
    [Expandable] public AnimalSpecies species = default;
    [SerializeField] private GameObject AnimalPrefab = default;
    [Header("Add existing animals")]
    [SerializeField] public List<GameObject> AnimalPopulation = default;
    [Header("Lowest Priority Behaviors")]
    [SerializeField] public List<SpecieBehaviorTrigger> DefaultBehaviors = default;
    [Header("Modify values and thresholds for testing")]
    [SerializeField] private List<Need> NeedEditorTesting = default;
    [Header("For reference only")]
    [SerializeField] private List<MovementData> AnimalsMovementData = default;

    private Dictionary<string, Need> needs = new Dictionary<string, Need>();
    private Dictionary<Need, Dictionary<NeedCondition, SpecieBehaviorTrigger>> needBehaviors = new Dictionary<Need, Dictionary<NeedCondition, SpecieBehaviorTrigger>>();

    private Vector3 origin = Vector3.zero;
    private GrowthCalculator GrowthCalculator = new GrowthCalculator();
    private float TimeSinceUpdate = 0f;
    private PoolingSystem PoolingSystem = default;
    private int prePopulationCount = default;
    private PopulationBehaviorManager PopulationBehaviorManager = default;

    private void Awake()
    {
        this.PopulationBehaviorManager = this.GetComponent<PopulationBehaviorManager>();
        this.PoolingSystem = this.GetComponent<PoolingSystem>();
        if (this.species != null)
        {
            this.SetupNeeds();
        }
    }

    private void Start()
    {
        int i=0;
        foreach(SpecieBehaviorTrigger behaviorPattern in this.DefaultBehaviors)
        {
            this.PopulationBehaviorManager.ActiveBehaviors.Add("default" + i, behaviorPattern);
            i++;
        }
    }

    /// <summary>
    /// Initialize the population as the given species at the given origin after runtime.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    ///  TODO population instantiation should likely come from an populationdata object with more fields
    public void InitializeNewPopulation(AnimalSpecies species, Vector3 origin, int populationSize)
    {
        this.species = species;
        this.origin = origin;
        this.transform.position = origin;

        this.PoolingSystem.AddPooledObjects(5, this.AnimalPrefab);
        for (int i = 0; i < populationSize; i++)
        {
            GameObject newAnimal = Instantiate(this.AnimalPrefab, this.transform);
            this.AnimalPopulation.Add(newAnimal);
            // PopulationManager will explicitly initialize a new population's animal at the very end
            this.AnimalPopulation[i].SetActive(true);
        }
        this.SetupNeeds();
    }

    private void SetupNeeds()
    {
        this.needs = this.Species.SetupNeeds();
        this.needBehaviors = this.Species.SetupBehaviors(this.needs);
        this.PopulationBehaviorManager.InitializeBehaviors(this.needs);
        this.NeedEditorTesting = new List<Need>();
        foreach (KeyValuePair<string, Need> need in this.needs)
        {
            this.NeedEditorTesting.Add(need.Value);
        }
    }

    private void Update()
    {
        this.HandleGrowth();
    }

    private void HandleGrowth()
    {
        float rate = this.GrowthCalculator.GrowthRate;
        if (rate == 0) return;
        if (this.TimeSinceUpdate > rate)
        {
            this.TimeSinceUpdate = 0;
            switch (this.GrowthCalculator.GrowthStatus)
            {
                case GrowthStatus.increasing:
                    this.AddAnimal();
                    break;
                case GrowthStatus.decreasing:
                    this.RemoveAnimal(1);
                    break;
                default:
                    break;
            }
        }
        this.TimeSinceUpdate += Time.deltaTime;
    }

    /// <summary>
    /// Grabs the updated accessible area and then resets the behavior for all of the animals.
    /// </summary>
    /// Could improve by checking what shape the accessible area is in
    public void UpdateAccessibleArea(List<Vector3Int> accessibleLocations, AnimalPathfinding.Grid grid)
    {
        this.AccessibleLocations = accessibleLocations;
        this.grid = grid;
        if (this.AccessibleLocations.Count < 6)
        {
            Debug.Log("Issue with accessibility area");
            this.PauseAnimals();
        }
        else
        {
            this.UnpauseAnimals();
        }
    }

    public void PauseAnimals()
    {
        foreach(GameObject animal in this.AnimalPopulation)
        {
            animal.GetComponent<MovementController>().IsPaused = true;
        }
    }

    public void UnpauseAnimals()
    {
        foreach(GameObject animal in this.AnimalPopulation)
        {
            animal.GetComponent<MovementController>().IsPaused = false;
        }
    }

    public void InitializeExistingAnimals()
    {
        foreach (GameObject animal in this.AnimalPopulation)
        {
            if (animal.activeSelf)
            {
                MovementData data = new MovementData();
                this.AnimalsMovementData.Add(data);
                animal.GetComponent<Animal>().Initialize(this, data);
            }
        }
        this.prePopulationCount = this.AnimalPopulation.Count;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var name = collision.gameObject.name;
    }

    public void AddAnimal()
    {
        MovementData data = new MovementData();
        this.AnimalsMovementData.Add(data);
        GameObject newAnimal = this.PoolingSystem.GetPooledObject(this.AnimalPopulation);
        if (newAnimal == null)
        {
            this.PoolingSystem.AddPooledObjects(5, this.AnimalPrefab);
            newAnimal = this.PoolingSystem.GetPooledObject(this.AnimalPopulation);
        }
        newAnimal.GetComponent<Animal>().Initialize(this, data);
    }

    // removes last animal in list and last behavior
    public void RemoveAnimal(int count)
    {
        this.AnimalsMovementData.RemoveAt(this.AnimalsMovementData.Count - 1);
        this.AnimalPopulation[this.AnimalPopulation.Count - 1].SetActive(false);
        this.AnimalPopulation.RemoveAt(this.AnimalPopulation.Count - 1);
        this.PoolingSystem.ReturnObjectToPool(this.AnimalPopulation[this.AnimalPopulation.Count - 1]);
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(string need, float value)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        this.needs[need].UpdateNeedValue(value);
        this.FilterBehaviors(need, this.needs[need].GetCondition(value));
        //Debug.Log($"The { species.SpeciesName } population { need } need has new value: {this.needs[need].NeedValue}");
    }

    /// <summary>
    /// Get the value of the given need.
    /// </summary>
    /// <param name="need">The need to get the value of</param>
    /// <returns></returns>
    public float GetNeedValue(string need)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        return this.needs[need].NeedValue;
    }

    /// <summary>
    /// Gets need conditions for each need based on the current values and sends them along with the need's severity to the growth formula system.
    /// </summary>
    public void UpdateGrowthConditions()
    {
        if (this.Species != null) this.GrowthCalculator.CalculateGrowth(this);
        //Debug.Log("Growth Status: " + this.GrowthCalculator.GrowthStatus + ", Growth Rate: " + this.GrowthCalculator.GrowthRate);
    }

    /// <summary>
    /// Updates the needs behaviors based on the need's current condition
    /// </summary>
    public void FilterBehaviors(string need, NeedCondition needCondition)
    {
        if (this.PopulationBehaviorManager.ActiveBehaviors.ContainsKey(need))
        {
            this.PopulationBehaviorManager.ActiveBehaviors[need] = this.needBehaviors[this.needs[need]][needCondition];
        }
    }

    // Ensure there are enough behavior data scripts mapped to the population size
    void OnValidate()
    {
        while (this.AnimalsMovementData.Count < this.AnimalPopulation.Count)
        {
            this.AnimalsMovementData.Add(new MovementData());
        }
        while (this.AnimalsMovementData.Count > this.AnimalPopulation.Count)
        {
            this.AnimalsMovementData.RemoveAt(this.AnimalsMovementData.Count - 1);
        }
        if (this.GrowthCalculator != null)
        {
            this.UpdateNeeds();
            this.UpdateGrowthConditions();
        }
    }

    private void UpdateNeeds()
    {
        int i=0;
        foreach (KeyValuePair<string, Need> need in this.needs)
        {
            if (this.NeedEditorTesting[i].NeedName.Equals(need.Key))
            {
                this.NeedEditorTesting[i] = need.Value;
            }
            i++;
        }
        if (this.NeedEditorTesting != null)
        {
            foreach (Need need in this.NeedEditorTesting)
            {
                this.needs[need.NeedName] = need;
            }
        }
    }

    public Dictionary<string, Need> GetNeedValues()
    {
        return this.Needs;
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    public bool GetAccessibilityStatus()
    {
        return this.HasAccessibilityChanged;
    }

    public void UpdatePopulationStateForChecking()
    {
        this.HasAccessibilityChanged = false;
        this.prePopulationCount = this.Count;
    }
}
