using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager of all the population instances
/// </summary>
public class PopulationManager : MonoBehaviour
{
    // FindTag population to populate
    public List<Population> Populations => ExistingPopulations;
    private List<Population> ExistingPopulations = new List<Population>();
    [SerializeField] private NeedSystemManager NeedSystemManager = default;
    [SerializeField] private BehaviorPatternUpdater BehaviorPatternUpdater = default;
    [SerializeField] private GameObject PopulationPrefab = default;
    [SerializeField] private ReservePartitionManager ReservePartitionManager = default;
    [SerializeField] public GridSystem GridSystem = default;
    [SerializeField] private List<PopulationBehavior> GenericBehaviors = default;
    [SerializeField] private LevelIO levelIO = default;
    [SerializeField] public SpeciesReferenceData speciesReferenceData = default;

    public void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.PopulationExtinct, this.RemovePopulation);
    }

    public void Initialize()
    {
        SerializedPopulation[] serializedPopulations = this.levelIO.presetMap.serializedPopulations;
        serializedPopulations = serializedPopulations ?? new SerializedPopulation[0];
        for (int i = 0; i < serializedPopulations.Length; i++)
        {
            Vector3[] pos = SerializationUtils.ParseVector3(serializedPopulations[i].population.coords);
            AnimalSpecies species = this.LoadSpecies(serializedPopulations[i].population.name);
            Population pop = null;
            foreach (Vector3 position in pos)
            {
                pop = UpdatePopulation(species, position);
            }
            pop.LoadGrowthRate(serializedPopulations[i].populationIncreaseRate);
        }
    }

    private AnimalSpecies LoadSpecies(string name)
    {
        if (this.speciesReferenceData.AnimalSpecies.ContainsKey(name))
        {
            return this.speciesReferenceData.AnimalSpecies[name];
        }
        Debug.LogError("No animal match the name '" + name + "' can be found in the species list. Did you attach the AnimalSpecies ScriptableObjects to the Population Manager?");
        return null;
    }

    /// <summary>
    /// Create a new population of the given species at the given position.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="position">The origin point of the population</param>
    public Population CreatePopulation(AnimalSpecies species, Vector3 position, Vector3[] positions = null)
    {
        // Create population
        GameObject newPopulationGameObject = Instantiate(this.PopulationPrefab, position, Quaternion.identity, this.transform);
        newPopulationGameObject.name = species.SpeciesName;
        Population population = newPopulationGameObject.GetComponent<Population>();
        this.ExistingPopulations.Add(population);
        // Initialize the basic population data, register the population, then initialize the animals and their behaviors
        population.GetComponent<PopulationBehaviorManager>().tempBehaviors = CopyBehaviors();
        population.InitializeNewPopulation(species, position);
        this.HandlePopulationRegistration(population);
        population.InitializeExistingAnimals();
        EventManager.Instance.InvokeEvent(EventType.NewPopulation, population);
        return population;
    }

    // TODO determine better way to setup behaviors
    private List<PopulationBehavior> CopyBehaviors()
    {
        List<PopulationBehavior> copiedBehaviors = new List<PopulationBehavior>();
        // Have to copy behavior scriptable objects for them to work on multiple populations
        foreach (PopulationBehavior behavior in GenericBehaviors)
        {
            List<BehaviorPattern> patterns = new List<BehaviorPattern>();
            foreach (BehaviorPattern pattern in behavior.behaviorPatterns)
            {
                patterns.Add(Instantiate(pattern, BehaviorPatternUpdater.gameObject.transform));
            }
            PopulationBehavior newBehavior = Instantiate(behavior);
            newBehavior.behaviorPatterns = patterns;
            copiedBehaviors.Add(newBehavior);
        }
        return copiedBehaviors;
    }

    private void RemovePopulation()
    {
        if (!ExistingPopulations.Contains((Population)EventManager.Instance.EventData))
        {
            return;
        }
        this.ExistingPopulations.Remove((Population)EventManager.Instance.EventData);
    }

    /// <summary>
    /// Creates a population if needed, then adds a new animal to the population
    /// </summary>
    /// <param name="species">The species of the animals to be added</param>
    /// <param name="count">The number of animals to add</param>
    /// <param name="position">The position to add them</param>
    public Population UpdatePopulation(AnimalSpecies species, Vector3 position)
    {
        Population population = DoesPopulationExist(species, position);
        if (population == null)
        {
            population = CreatePopulation(species, position);
        }
        population.AddAnimal(position);
        return population;
    }

    private Population DoesPopulationExist(AnimalSpecies species, Vector3 position)
    {
        List<Population> localPopulations = ReservePartitionManager.GetPopulationsWithAccessTo(position);
        foreach (Population preexistingPopulation in localPopulations)
        {
            if (preexistingPopulation.Species.SpeciesName.Equals(species.SpeciesName))
            {
                return preexistingPopulation;
            }
        }
        return null;
    }

    // Registers the population with all of the systems that care about it
    private void HandlePopulationRegistration(Population population)
    {
        this.ReservePartitionManager.AddPopulation(population);
        population.UpdateAccessibleArea(this.ReservePartitionManager.GetLocationsWithAccess(population),
        this.GridSystem.GetGridWithAccess(population));
        this.NeedSystemManager.RegisterWithNeedSystems(population);
        this.BehaviorPatternUpdater.RegisterPopulation(population);
    }

    public void UdateAllPopulationRegistration()
    {
        foreach (Population population in this.ExistingPopulations)
        {
            HandlePopulationRegistration(population);
        }
    }

    public void UdateAllPopulationStateForChecking()
    {
        foreach (Population population in this.ExistingPopulations)
        {
            population.UpdatePopulationStateForChecking();
        }
    }

    public void UpdateAllGrowthConditions()
    {
        foreach(Population population in this.ExistingPopulations)
        {
            population.UpdateGrowthConditions();
        }
    }

    // Creates new populations if population becomes split or updates population's map
    public void UpdateAccessibleLocations()
    {
        ReservePartitionManager.UpdateAccessMap();
        // combines populations
        for (int i=1; i<this.Populations.Count; i++)
        {
            if (this.Populations[i - 1].Species.Equals(this.Populations[i].Species) && ReservePartitionManager.CanAccessPopulation(this.Populations[i - 1], this.Populations[i]))
            {
                for (int j = this.Populations[i - 1].AnimalPopulation.Count - 1; j >= 0; j--)
                {
                    GameObject animal = this.Populations[i - 1].AnimalPopulation[j];
                    this.Populations[i].AddAnimal(animal.transform.position);
                }
                RemovePopulation(this.Populations[i - 1]);
            }
        }
        List<Population> currentPopulations = new List<Population>();
        foreach(Population population in this.Populations)
        {
            currentPopulations.Add(population);
        }
        foreach (Population population in currentPopulations)
        {
            // Debug.Log("Accessible map updated for " + population.name);
            List<Vector3Int> accessibleLocations = ReservePartitionManager.GetLocationsWithAccess(population);
            AnimalPathfinding.Grid grid = GridSystem.GetGridWithAccess(population);
            // checks for animals cut off from population
            for (int i = population.AnimalPopulation.Count - 1; i >= 0; i--)
            {
                GameObject animal = population.AnimalPopulation[i];
                if (!accessibleLocations.Contains(grid.grid.WorldToCell(animal.transform.position)))
                {
                    UpdatePopulation(population.Species, animal.transform.position);
                    population.RemoveAnimal(animal);
                }
            }
            if (accessibleLocations.Count == 0 || population.AnimalPopulation.Count == 0)
            {
                RemovePopulation(population);
            }
            else
            {
                population.UpdateAccessibleArea(accessibleLocations, grid);
            }
            
        }
    }

    public void RemovePopulation(Population population)
    {
        Debug.Log("Removing " + population);
        population.RemoveAll();
        this.Populations.Remove(population);
        NeedSystemManager.UnregisterWithNeedSystems(population);
        ReservePartitionManager.RemovePopulation(population);
    }

    public List<Population> GetPopulationsBySpecies(AnimalSpecies animalSpecies)
    {
        List<Population> populations = new List<Population>();

        foreach(Population population in this.ExistingPopulations)
        {
            if (population.species == animalSpecies)
            {
                populations.Add(population);
            }
        }

        return populations;
    }
}
