using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// TODO figure out how to refactor the MarkNeedsDirty so that NeedSystemManager isn't a dependency
/// <summary>
/// A runtime instance of a population.
/// </summary>
public class Population : MonoBehaviour, Life
{
    [Expandable] public AnimalSpecies species = default;
    public AnimalSpecies Species { get => species; }
    public int Count { get => this.AnimalPopulation.Count; }
    public float Dominance => Count * species.Dominance;

    public Dictionary<string, Need> Needs => needs;
    private Dictionary<string, Need> needs = new Dictionary<string, Need>();
    public AnimalPathfinding.Grid grid { get; private set; }
    // TODO when accessible locations becomes nothing, add a warning so the player can respond.
    public List<Vector3Int>  AccessibleLocations { get; private set; }
    public List<BehaviorScriptName> CurrentBehaviors { get; private set; }
    // TODO remove this and all dependencies once framework in place, add as actual movement

    [Header("Add existing animals")]
    [SerializeField] public List<GameObject> AnimalPopulation = default;
    [SerializeField] private GameObject AnimalPrefab = default;
    [Header("Updated through OnValidate")]
    [SerializeField] private List<BehaviorsData> AnimalsBehaviorData = default;
    [Header("Modify values and thresholds for testing")]
    // Initialized when InitializePopulationDataCalled and updates the Need Dictionary OnValidate
    [SerializeField] private List<Need> NeedEditorTesting = default;

    private Vector3 origin = Vector3.zero;
    private GrowthCalculator GrowthCalculator = default;
    private NeedSystemManager NeedSystemManager = default;
    public float TimeSinceUpdate = 0f;

    private void Awake()
    {
        this.CurrentBehaviors = new List<BehaviorScriptName>();
        this.GrowthCalculator = new GrowthCalculator();
    }

    /// <summary>
    /// Initialize the population as the given species at the given origin after runtime.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    ///  TODO population instantiation should likely come from an populationdata object with more fields
    public void InitializeNewPopulation(AnimalSpecies species, Vector3 origin, int populationSize, NeedSystemManager needSystemManager)
    {
        this.NeedSystemManager = needSystemManager;
        this.species = species;
        this.origin = origin;
        this.transform.position = origin;
        // - the pods will contain populations which we'll likely want to specify certain aspects of
        for (int i=0; i<populationSize; i++)
        {
            this.AnimalsBehaviorData.Add(new BehaviorsData());
            this.AddAnimal(this.AnimalsBehaviorData[i]);
        }
        this.InitializePopulationData(needSystemManager);
    }

    // Can be initialized at runtime if the species is defined or later when a pod is used
    /// <summary>
    /// Adds populations to rpm, gets accessible locations, gets behaviors scripts, and adds needs
    /// </summary>
    public void InitializePopulationData(NeedSystemManager needSystemManager)
    {
        this.NeedSystemManager = needSystemManager;
        ReservePartitionManager.ins.AddPopulation(this);
        this.UpdateAccessibleArea();
        this.CurrentBehaviors = new List<BehaviorScriptName>();
        foreach (BehaviorScriptTranslation data in this.Species.Behaviors)
        {
            // Debug.Log("Behavior added");
            this.CurrentBehaviors.Add(data.behaviorScriptName);
        }
        this.needs = this.Species.SetupNeeds();
        this.SetupNeedTesting();
    }

    private void SetupNeedTesting()
    {
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
                    this.TestAddAnimal();
                    break;
                case GrowthStatus.decreasing:
                    this.TestRemoveAnimal();
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
    public void UpdateAccessibleArea()
    {
        this.AccessibleLocations = ReservePartitionManager.ins.GetLocationsWithAccess(this);
        this.grid = ReservePartitionManager.ins.GetGridWithAccess(this, TilemapUtil.ins.largestMap);
    }

    public void InitializeExistingAnimals()
    {
        int i = 0;
        foreach (GameObject animal in this.AnimalPopulation)
        {
            if (animal.activeSelf)
            {
                animal.GetComponent<Animal>().Initialize(this, this.AnimalsBehaviorData[i]);
                i++;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var name = collision.gameObject.name;
    }

    public void AddAnimal(BehaviorsData data)
    {
        GameObject newAnimal = Instantiate(this.AnimalPrefab, this.gameObject.transform);
        newAnimal.GetComponent<Animal>().Initialize(this, data);
        AnimalPopulation.Add(newAnimal);
        this.MarkNeedsDirty();
    }

    private void MarkNeedsDirty()
    {
        // Making the NS of this pop's need dirty (Density, FoodSource and Species)
        foreach (string needName in this.needs.Keys)
        {
            if (!Enum.IsDefined(typeof(AtmoshpereComponent), needName) && !Enum.IsDefined(typeof(TileType), needName))
            {
                this.NeedSystemManager.Systems[needName].MarkAsDirty();
            }
        }
        // Debug.Log("Current Need Systems: ");
        // foreach(KeyValuePair<string, NeedSystem> needSystem in NeedSystemManager.ins.Systems)
        // {
        //     Debug.Log(needSystem.Key);
        // }
        // Debug.Log("Attempting to add: " + this.species.SpeciesName);
        this.NeedSystemManager.Systems[this.species.SpeciesName].MarkAsDirty();
    }

    // TODO remove using PoolingSystem
    public void RemoveAnimal(int count)
    {
        this.MarkNeedsDirty();
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(string need, float value)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        this.needs[need].Value = value;
        // Debug.Log($"The { species.SpeciesName } population { need } need has new value: {NeedsValues[need]}");
    }

    /// <summary>
    /// Get the value of the given need.
    /// </summary>
    /// <param name="need">The need to get the value of</param>
    /// <returns></returns>
    public float GetNeedValue(string need)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        return this.needs[need].Value;
    }

    /// <summary>
    /// Gets need conditions for each need based on the current values and sends them along with the need's severity to the growth formula system.
    /// </summary>
    public void UpdateGrowthConditions()
    {
        if (this.Species != null) this.GrowthCalculator.CalculateGrowth(this);
        Debug.Log("Growth Status: " + this.GrowthCalculator.GrowthStatus + ", Growth Rate: " + this.GrowthCalculator.GrowthRate);
    }

    private void TestAddAnimal()
    {
        this.AnimalsBehaviorData.Add(new BehaviorsData());
        GameObject newAnimal = Instantiate(this.AnimalPrefab, this.gameObject.transform);
        newAnimal.GetComponent<Animal>().Initialize(this, this.AnimalsBehaviorData[this.AnimalsBehaviorData.Count - 1]);
        AnimalPopulation.Add(newAnimal);
    }

    public void TestRemoveAnimal()
    {
        Destroy(this.AnimalPopulation[this.AnimalPopulation.Count - 1]);
        this.AnimalPopulation.RemoveAt(this.AnimalPopulation.Count - 1);
    }

    // TODO setup filter for adding/removing behaviors from this.CurrentBehaviors according to populations condition
    /// <summary>
    /// Adds and removes behaviors based on each need's current condition and severity.
    /// Multiple behaviors can also be added for increased representation.
    /// </summary>
    public void FilterBehaviors()
    {
        throw new System.NotImplementedException();
    }

    // Ensure there are enough behavior data scripts mapped to the population size
    void OnValidate()
    {
        while (this.AnimalsBehaviorData.Count < this.AnimalPopulation.Count)
        {
            this.AnimalsBehaviorData.Add(new BehaviorsData());
        }
        while (this.AnimalsBehaviorData.Count > this.AnimalPopulation.Count)
        {
            this.AnimalsBehaviorData.RemoveAt(this.AnimalsBehaviorData.Count - 1);
        }
        if (this.GrowthCalculator != null)
        {
            this.UpdateNeeds();
            this.UpdateGrowthConditions();
        }
    }

    private void UpdateNeeds()
    {
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
}

