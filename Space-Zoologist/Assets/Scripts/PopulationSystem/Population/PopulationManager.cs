using System.Linq;
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
    private BehaviorPatternUpdater BehaviorPatternUpdater => GameManager.Instance.m_behaviorPatternUpdater;
    [SerializeField] private GameObject PopulationPrefab = default;

    public void Initialize()
    {
        SerializedPopulation[] serializedPopulations = GameManager.Instance.PresetMap.serializedPopulations;
        serializedPopulations = serializedPopulations ?? new SerializedPopulation[0];
        for (int i = 0; i < serializedPopulations.Length; i++)
        {
            Vector3[] pos = SerializationUtils.ParseVector3(serializedPopulations[i].population.coords);
            AnimalSpecies species = this.LoadSpecies(serializedPopulations[i].population.name);
            Population pop = null;
            foreach (Vector3 position in pos)
            {
                pop = SpawnAnimal(species, position);
            }
        }

        EventManager.Instance.SubscribeToEvent(EventType.PopulationExtinct, this.RemovePopulation);
        EventManager.Instance.SubscribeToEvent(EventType.StoreToggled, (eventData) => { if (!(bool) eventData) UpdateAccessibleLocations(); });
    }

    private AnimalSpecies LoadSpecies(string name)
    {
        ItemID id = ItemRegistry.FindHasName(name);
        if (GameManager.Instance.AnimalSpecies.ContainsKey(id))
        {
            return GameManager.Instance.AnimalSpecies[id];
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
        newPopulationGameObject.name = species.ID.Data.Name.Get(ItemName.Type.Serialized);
        Population population = newPopulationGameObject.GetComponent<Population>();
        this.ExistingPopulations.Add(population);
        // Initialize the basic population data, register the population, then initialize the animals and their behaviors
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

    private void RemovePopulation(object eventData)
    {
        if (!ExistingPopulations.Contains((Population)eventData))
        {
            return;
        }
        this.ExistingPopulations.Remove((Population)eventData);
    }

    /// <summary>
    /// Creates a population if needed, then adds a new animal to the population
    /// </summary>
    /// <param name="species">The species of the animals to be added</param>
    /// <param name="count">The number of animals to add</param>
    /// <param name="position">The position to add them</param>
    public Population SpawnAnimal(AnimalSpecies species, Vector3 position)
    {
        Population population = GetPopulation(species, position);
        if (population == null)
        {
            population = CreatePopulation(species, position);
        }
        population.AddAnimal(position);
        return population;
    }

    private Population GetPopulation(AnimalSpecies species, Vector3 position)
    {
        List<Population> localPopulations = GameManager.Instance.m_reservePartitionManager.GetPopulationsWithAccessTo(position);
        foreach (Population preexistingPopulation in localPopulations)
        {
            if (preexistingPopulation.Species.ID == species.ID)
            {
                return preexistingPopulation;
            }
        }
        return null;
    }

    // Registers the population with all of the systems that care about it
    private void HandlePopulationRegistration(Population population)
    {
        GameManager.Instance.m_reservePartitionManager.AddPopulation(population);
        population.UpdateAccessibleArea(GameManager.Instance.m_reservePartitionManager.GetLocationsWithAccess(population),
        GameManager.Instance.m_tileDataController.GetGridWithAccess(population));
        this.BehaviorPatternUpdater.RegisterPopulation(population);

        // NOTE: does the need cache need to be updated now?
    }

    public void UpdateAllPopulationRegistration()
    {
        foreach (Population population in this.ExistingPopulations)
        {
            HandlePopulationRegistration(population);
        }
    }

    public void UpdateAllPopulationStateForChecking()
    {
        foreach (Population population in this.ExistingPopulations)
        {
            population.UpdatePopulationStateForChecking();
        }
    }

    public void UpdateAccessibleLocations()
    {
        GameManager.Instance.m_reservePartitionManager.UpdateAccessMap();
        SimplifyPopulations();
        List<Population> currentPopulations = new List<Population>();
        currentPopulations = this.Populations.GetRange(0, this.ExistingPopulations.Count);
        foreach (Population population in currentPopulations)
        {
            HandlePopulationSplitting(population);
        }
    }

    // Combines populations that are connected and support the same species
    private void SimplifyPopulations()
    {
        for (int i = 1; i < this.ExistingPopulations.Count; i++)
        {
            if (this.ExistingPopulations[i - 1].Species.Equals(this.ExistingPopulations[i].Species) 
                && GameManager.Instance.m_reservePartitionManager.CanAccessPopulation(this.ExistingPopulations[i - 1], this.ExistingPopulations[i]))
            {
                for (int j = this.ExistingPopulations[i - 1].AnimalPopulation.Count - 1; j >= 0; j--)
                {
                    GameObject animal = this.ExistingPopulations[i - 1].AnimalPopulation[j];
                    this.ExistingPopulations[i].AddAnimal(animal.transform.position);
                }
                RemovePopulation(this.ExistingPopulations[i - 1]);
            }
        }
    }

    private void HandlePopulationSplitting(Population population)
    {
        List<Vector3Int> accessibleLocations = GameManager.Instance.m_reservePartitionManager.GetLocationsWithAccess(population);
        for (int i = population.AnimalPopulation.Count - 1; i >= 0; i--)
        {
            GameObject animal = population.AnimalPopulation[i];
            Vector3Int animalLocation = GameManager.Instance.m_tileDataController.WorldToCell(animal.transform.position);
            if (!accessibleLocations.Contains(animalLocation))
            {
                Debug.Log("Creating new population");
                SpawnAnimal(population.Species, animal.transform.position);
                population.RemoveAnimal(animal);
            }
        }
        if (accessibleLocations.Count == 0 || population.AnimalPopulation.Count == 0)
        {
            RemovePopulation(population);
        }
        else
        {
            AnimalPathfinding.Grid grid = GameManager.Instance.m_tileDataController.GetGridWithAccess(population);
            population.UpdateAccessibleArea(accessibleLocations, grid);
        }
    }

    public void HandleGrowth () {
        foreach (Population population in this.ExistingPopulations) {
            population.HandleGrowth ();
        }
    }

    public void RemovePopulation(Population population)
    {
        Debug.Log("Removing " + population);
        population.RemoveAll();
        Populations.Remove(population);
        GameManager.Instance.m_reservePartitionManager.RemovePopulation(population);
        Destroy(population.gameObject);

        // NOTE: does the need cache need to be updated now?
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
    public int TotalPopulationSize(AnimalSpecies species)
    {
        List<Population> populations = GetPopulationsBySpecies(species);
        return populations.Sum(pop => pop.Count);
    }
}
