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

    public Dictionary<string, float> NeedsValues => needsValues;
    protected Dictionary<string, float> needsValues = new Dictionary<string, float>();

    [SerializeField] private AnimalSpecies species = default;
    [SerializeField] private GameObject AnimalPrefab = default;
    // Defined at runtime or added when a pod is used
    [Header("Add existing animals")]
    [SerializeField] public List<GameObject> AnimalPopulation = default;
    // Data can be accessed via scripts by going through the animals themselves. This is for editing in editor
    [Header("Updated through OnValidate")]
    [SerializeField] private List<BehaviorsData> AnimalsBehaviorData = default;

    // updated in FilterBehaviors function
    public List<BehaviorScriptName> CurrentBehaviors { get; private set; }
    private Dictionary<string, float> Needs = new Dictionary<string, float>();
    private Vector3 origin = Vector3.zero;

    // 2d array based off of accessible locations for a populations pathfinding
    public AnimalPathfinding.Grid grid { get; private set; }
    // TODO when accessible locations becomes nothing, add a warning so the player can respond.
    public List<Vector3Int>  AccessibleLocations { get; private set; }

    private void Awake()
    {
        this.CurrentBehaviors = new List<BehaviorScriptName>();
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
        // - the pods will contain populations which we'll likely want to specify certain aspects of
        this.InitializePopulationData();
        for (int i=0; i<populationSize; i++)
        {
            this.AnimalsBehaviorData.Add(new BehaviorsData());
            this.AddAnimal(this.AnimalsBehaviorData[i]);
        }

        //foreach (Need need in species.Needs.Values)
        //{
        //    needsValues.Add(need.NeedName, 0);
        //    Debug.Log($"Add {need.NeedName} NeedValue to {this.species.SpeciesName}");
        //}
    }

    // Can be initialized at runtime if the species is defined or later when a pod is used
    /// <summary>
    /// Adds populations to rpm, gets accessible locations, gets behaviors scripts, and adds needs
    /// </summary>
    public void InitializePopulationData()
    {
        ReservePartitionManager.ins.AddPopulation(this);
        this.UpdateAccessibleArea();
        this.CurrentBehaviors = new List<BehaviorScriptName>();
        foreach (BehaviorScriptTranslation data in this.Species.Behaviors)
        {
            // Debug.Log("Behavior added");
            this.CurrentBehaviors.Add(data.behaviorScriptName);
        }
        foreach (KeyValuePair<string, Need> need in Species.Needs)
        {
            Needs.Add(need.Value.NeedName, 0);
            needsValues.Add(need.Value.NeedName, 0);
            //Debug.Log($"Add {need.Value.NeedName} NeedValue to {this.species.SpeciesName}");
        }
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

        this.MarkNSDirtyDueToAddCount();
    }

    private void MarkNSDirtyDueToAddCount()
    {
        // Making the NS of this pop's need dirty (Density, FoodSource and Species)
        foreach (string needName in this.needsValues.Keys)
        {
            if (!Enum.IsDefined(typeof(AtmoshpereComponent), needName) && !Enum.IsDefined(typeof(TileType), needName))
            {
                NeedSystemManager.ins.Systems[needName].MarkAsDirty();
            }
        }

        // Mark SpeciesNS of this pop type dirty
        NeedSystemManager.ins.Systems[this.species.SpeciesName].MarkAsDirty();
    }

    public void RemoveAniaml(int count)
    {
        // TODO: remove animal

        foreach (string needName in this.needsValues.Keys)
        {
            if (!Enum.IsDefined(typeof(AtmoshpereComponent), needName) && !Enum.IsDefined(typeof(TileType), needName))
            {
                NeedSystemManager.ins.Systems[needName].MarkAsDirty();
            }
        }
        NeedSystemManager.ins.Systems[this.species.SpeciesName].MarkAsDirty();
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(string need, float value)
    {
        Debug.Assert(needsValues.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        needsValues[need] = value;
        // Debug.Log($"The { species.SpeciesName } population { need } need has new value: {NeedsValues[need]}");
    }

    /// <summary>
    /// Get the value of the given need.
    /// </summary>
    /// <param name="need">The need to get the value of</param>
    /// <returns></returns>
    public float GetNeedValue(string need)
    {
        Debug.Assert(needsValues.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        return needsValues[need];
    }

    /// <summary>
    /// Gets need conditions for each need based on the current values and sends them along with the need's severity to the growth formula system.
    /// </summary>
    public void UpdateGrowthConditions()
    {
        throw new System.NotImplementedException();
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
    }

    public Dictionary<string, float> GetNeedValues()
    {
        return this.NeedsValues;
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }
}

