using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager of all the population instances
/// </summary>
public class PopulationManager : MonoBehaviour
{
    // FindObjectOfType<Population> to populate
    private List<Population> ExistingPopulations = new List<Population>();
    public List<Population> Populations => ExistingPopulations;
    [SerializeField] public NeedSystemManager NeedSystemManager = default;
    [SerializeField] public LevelDataReference LevelDataReference = default;
    [SerializeField] private GameObject PopulationPrefab = default;
    [SerializeField] public bool AutomotonTesting = false;
    [SerializeField] private ReservePartitionManager ReservePartitionManager = default;
    [SerializeField] private GridSystem GridSystem = default;

    private SpeciesNeedSystem speciesNeedSystem = null;

    public void Initialize()
    {
        GameObject[] populations = GameObject.FindGameObjectsWithTag("Population");
        this.speciesNeedSystem = (SpeciesNeedSystem)NeedSystemManager.Systems[NeedType.Species];

        foreach (GameObject population in populations)
        {
            this.ExistingPopulations.Add(population.GetComponent<Population>());
        }

        foreach (Population population in this.ExistingPopulations)
        {
            this.SetupExistingPopulation(population);
        }
    }

    /// <summary>
    /// Create a new population of the given species at the given position.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="position">The origin point of the population</param>
    public void CreatePopulation(AnimalSpecies species, int count, Vector3 position)
    {
        // Create population
        GameObject newPopulationGameObject = Instantiate(this.PopulationPrefab, position, Quaternion.identity, this.transform);
        newPopulationGameObject.name = species.SpeciesName;
        Population population = newPopulationGameObject.GetComponent<Population>();
        this.ExistingPopulations.Add(population);
        // Initialize the basic population data, register the population, then initialize the specific population data, then initialize the animals
        population.InitializeNewPopulation(species, position, count, NeedSystemManager, ReservePartitionManager);
        population.InitializePopulationData(NeedSystemManager, ReservePartitionManager);
        this.HandlePopulationRegistration(population);
        population.InitializeExistingAnimals();
    }

    /// <summary>
    /// Add animals to the accessible area containing the given position. If there is already a population, add the animals to it, else create a new population.
    /// </summary>
    /// <param name="species">The species of the animals to be added</param>
    /// <param name="count">The number of animals to add</param>
    /// <param name="position">The position to add them</param>
    public void AddAnimals(AnimalSpecies species, int count, Vector3 position)
    {
        // If a population of the species already exists in this area, just combine with it, otherwise, make a new one
        List<Population> localPopulations = ReservePartitionManager.GetPopulationsWithAccessTo(position);
        Population preexistingPopulation = localPopulations.Find(p => p.Species == species);
        if (preexistingPopulation)
        {
            preexistingPopulation.AddAnimal(new BehaviorsData());
        }
        else
        {
            CreatePopulation(species, count, position);
        }
    }

    // register the existing population, initialize it's specific data, then initialize the animals
    private void SetupExistingPopulation(Population population)
    {
        this.HandlePopulationRegistration(population);
        population.InitializePopulationData(NeedSystemManager, ReservePartitionManager);
        population.InitializeExistingAnimals();
    }

    // Registers the population with all all of the systems that care about it
    private void HandlePopulationRegistration(Population population)
    {
        this.ReservePartitionManager.AddPopulation(population);
        population.UpdateAccessibleArea(ReservePartitionManager.GetLocationsWithAccess(population), 
        GridSystem.GetGridWithAccess(population));
        this.speciesNeedSystem.AddPopulation(population);
        NeedSystemManager.RegisterWithNeedSystems(population);
    }

    public void ResetAccessibilityStatus()
    {
        foreach (Population population in this.ExistingPopulations)
        {
            ReservePartitionManager.PopulationAccessbilityStatus[population] = false;
        }
    }
}
