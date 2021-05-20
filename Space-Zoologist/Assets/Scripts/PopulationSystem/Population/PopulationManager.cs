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

    public void Initialize()
    {
        SerializedPopulation[] serializedPopulations = this.levelIO.presetMap.serializedPopulations;
        for (int i = 0; i < serializedPopulations.Length; i++)
        {
            Vector3[] pos = SerializationUtils.ParseVector3(serializedPopulations[i].population.coords);
            AnimalSpecies species = this.LoadSpecies(serializedPopulations[i].population.name);
            foreach (Vector3 position in pos)
            {
                UpdatePopulation(species, position);
            }
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
        population.GetComponent<PopulationBehaviorManager>().tempBehaviors = GenericBehaviors;
        // Initialize the basic population data, register the population, then initialize the animals and their behaviors
        population.InitializeNewPopulation(species, position, positions);
        this.HandlePopulationRegistration(population);
        population.InitializeExistingAnimals();
        EventManager.Instance.InvokeEvent(EventType.NewPopulation, population);
        return population;
    }
    /// <summary>
    /// Creates a population if needed, then adds a new animal to the population
    /// </summary>
    /// <param name="species">The species of the animals to be added</param>
    /// <param name="count">The number of animals to add</param>
    /// <param name="position">The position to add them</param>
    public void UpdatePopulation(AnimalSpecies species, Vector3 position)
    {
        Population population = DoesPopulationExist(species, position);
        if (population == null)
        {
            population = CreatePopulation(species, position);
        }
        population.AddAnimal(position);
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

    public void UpdateAccessibleLocations()
    {
        this.NeedSystemManager.UpdateAccessMap();
        foreach (Population population in this.Populations)
        {
            // Debug.Log("Accessible map updated for " + population.name);
            population.UpdateAccessibleArea(ReservePartitionManager.GetLocationsWithAccess(population),
            GridSystem.GetGridWithAccess(population));
        }
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
