using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A runtime instance of a species.
/// </summary>
public class Population : MonoBehaviour
{
    // TODO refactor into static utitlity
    public Tilemap TilemapReference { get; set; }
    // Defined at runtime or added when a pod is used based off pods
    [SerializeField] public List<GameObject> animalPopulation = default;

    [SerializeField] private GameObject AnimalPrefab = default;
    [SerializeField] private Species species = default;

    // Data can be accessed by going through the animals themselves
    [SerializeField] private List<BehaviorsData> AnimalsBehaviorData = default;
    
    public List<BehaviorScriptName> CurrentBehaviors { get; set; }
    public Species Species { get => species; }
    public string SpeciesName { get => Species.SpeciesName; }
    private Dictionary<NeedName, float> Needs = new Dictionary<NeedName, float>();
    private Vector2Int origin = Vector2Int.zero;

    private List<GameObject> AnimalObjects = default;
    public AnimalPathfinding Pathfinder { get; private set; }
    public List<Vector3Int> AccessibleLocations { get; set; }

    private void Awake()
    {
        this.CurrentBehaviors = new List<BehaviorScriptName>();
    }

    private void Start()
    {
        this.TilemapReference = GameObject.Find("Background").GetComponent<Tilemap>();
        this.Pathfinder = this.gameObject.GetComponent<AnimalPathfinding>();
        this.InitializeExisitingAnimals();
    }

    private void InitializeExisitingAnimals()
    {
        int i = 0;
        foreach (GameObject animal in this.animalPopulation)
        {
            this.AddComponentByName(Species.Behaviors, animal);
            animal.GetComponent<Animal>().Initialize(this, this.AnimalsBehaviorData[i]);
            i++;
        }
    }

    /// <summary>
    /// Initialize the population as the given species at the given origin after runtime.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    public void InitializeNewPopulation(Species species, Vector2Int origin, int populationSize)
    {
        this.species = species;
        this.origin = origin;
        for (int i=0; i<populationSize; i++)
        {
            this.InstantiateAnimal(this.AnimalsBehaviorData[i]);
        }
        // TODO population instantiation should likely come from an populationdata object which should already have this defined
        // - the pods will contain populations which we'll likely want to specify certain aspects of
        while (this.AnimalsBehaviorData.Count < this.animalPopulation.Count)
        {
            this.AnimalsBehaviorData.Add(new BehaviorsData());
        }
        this.InitializePopulationData();
    }

    // Can be initialized at runtime if the species is defined or later when a pod is used
    /// <summary>
    /// Adds populations to rpm, gets accessible locations, gets behaviors scripts, and adds needs
    /// </summary>
    public void InitializePopulationData()
    {
        ReservePartitionManager.ins.AddPopulation(this);
        this.transform.position = GridUtils.Vector2IntToVector3Int(origin);
        this.AccessibleLocations = ReservePartitionManager.ins.GetLocationWithAccess(this);
        this.CurrentBehaviors = new List<BehaviorScriptName>();
        foreach (BehaviorScriptTranslation data in this.Species.Behaviors)
        {
            this.CurrentBehaviors.Add(data.behaviorScriptName);
        }
        foreach (SpeciesNeed need in Species.Needs)
        {
            Needs.Add(need.Name, 0);
        }
    }

    public void InstantiateAnimal(BehaviorsData data)
    {
        GameObject newAnimal = Instantiate(this.AnimalPrefab, this.gameObject.transform);
        this.AddComponentByName(Species.Behaviors, newAnimal);
        newAnimal.GetComponent<Animal>().Initialize(this, data);
        animalPopulation.Add(newAnimal);
    }

    // No way to dynamially add scripts by name, have to use if statements and add to this everytime new component is defined
    private void AddComponentByName(List<BehaviorScriptTranslation> components, GameObject newAnimal)
    {
        foreach (BehaviorScriptTranslation component in components)
        {
            switch(component.behaviorScriptName)
            {
                case BehaviorScriptName.RandomMovement:
                    newAnimal.AddComponent<RandomMovement>();
                    break;
                case BehaviorScriptName.Idle:
                    newAnimal.AddComponent<Idle>();
                    break;
                case BehaviorScriptName.Eating:
                    newAnimal.AddComponent<Eating>();
                    break;
                default:
                    Debug.Log("No component with the type found");
                    break;
            }
        }
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(NeedName need, float value)
    {
        if (Needs.ContainsKey(need))
        {
            Needs[need] = value;
            // UpdateGrowthConditions();
        }
        else
        {
            Debug.Log("Need not found");
        }
    }

    /// <summary>
    /// Get the value of the given need.
    /// </summary>
    /// <param name="need">The need to get the value of</param>
    /// <returns></returns>
    public float GetNeedValue(NeedName need)
    {
        if (!Needs.ContainsKey(need))
        {
            Debug.Log($"Tried to access nonexistent need '{need}' in a { SpeciesName } population");
            return 0;
        }

        return Needs[need];
    }

    // TODO: Implement
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

    // TODO refactor into static utitlity
    public Vector3Int WorldToCell(Vector3 position)
    {
        return this.TilemapReference.WorldToCell(position);
    }

    // Ensure there are enough behavior data scripts mapped to the population size
    void OnValidate()
    {
        while (this.AnimalsBehaviorData.Count < this.animalPopulation.Count)
        {
            this.AnimalsBehaviorData.Add(new BehaviorsData());
        }
        while (this.AnimalsBehaviorData.Count > this.animalPopulation.Count)
        {
            this.AnimalsBehaviorData.RemoveAt(this.AnimalsBehaviorData.Count - 1);
        }
    }
}

