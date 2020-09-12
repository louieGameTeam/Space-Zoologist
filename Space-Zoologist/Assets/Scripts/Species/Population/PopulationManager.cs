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
    [SerializeField] private GridSystem GridSystem = default;

    private SpeciesNeedSystem speciesNeedSystem = null;
    private SymbiosisNeedSystem symbiosisNeedSystem = null;

    public void Initialize()
    {
        GameObject[] populations = GameObject.FindGameObjectsWithTag("Population");
        this.speciesNeedSystem = (SpeciesNeedSystem)NeedSystemManager.Systems[NeedType.Species];
        this.symbiosisNeedSystem = (SymbiosisNeedSystem)NeedSystemManager.Systems[NeedType.Symbiosis];

        foreach (GameObject population in populations)
        {
            this.ExistingPopulations.Add(population.GetComponent<Population>());
        }

        foreach (Population population in this.ExistingPopulations)
        {
            this.SetupExistingPopulation(population);
        }
        this.NeedSystemManager.UpdateAllSystems();
    }

    public void PauseAllAnimals()
    {
        foreach (Population population in this.Populations)
        {
            population.PauseAnimalsMovementController();
        }
    }

    public void UnpauseAllAnimals()
    {
        foreach (Population population in this.Populations)
        {
            population.UnpauseAnimalsMovementController();
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
        // Initialize the basic population data, register the population, then initialize the animals and their behaviors
        population.InitializeNewPopulation(species, position, count);
        this.HandlePopulationRegistration(population);
        population.InitializeExistingAnimals();

        EventManager.Instance.InvokeEvent(EventType.NewPopulation, population);
    }

    /// <summary>
    /// Add animals to the accessible area containing the given position. If there is already a population, add the animals to it, else create a new population.
    /// </summary>
    /// <param name="species">The species of the animals to be added</param>
    /// <param name="count">The number of animals to add</param>
    /// <param name="position">The position to add them</param>
    public void UpdatePopulation(AnimalSpecies species, int count, Vector3 position)
    {
        // If a population of the species already exists in this area, just combine with it, otherwise, make a new one
        List<Population> localPopulations = ReservePartitionManager.GetPopulationsWithAccessTo(position);
        foreach(Population preexistingPopulation in localPopulations)
        {
            if (preexistingPopulation.Species.SpeciesName.Equals(species.SpeciesName))
            {
                preexistingPopulation.AddAnimal();
                return;
            }
        }
        CreatePopulation(species, count, position);
    }

    // register the existing population then initialize the animals
    private void SetupExistingPopulation(Population population)
    {
        this.HandlePopulationRegistration(population);
        this.GridSystem.UnhighlightHomeLocations();
        population.InitializeExistingAnimals();
    }

    // Registers the population with all all of the systems that care about it
    private void HandlePopulationRegistration(Population population)
    {
        this.ReservePartitionManager.AddPopulation(population);
        population.UpdateAccessibleArea(this.ReservePartitionManager.GetLocationsWithAccess(population),
        this.GridSystem.GetGridWithAccess(population));
        this.GridSystem.HighlightHomeLocations();
        this.speciesNeedSystem.AddPopulation(population);
        this.NeedSystemManager.RegisterWithNeedSystems(population);
        this.BehaviorPatternUpdater.RegisterPopulation(population);
        this.symbiosisNeedSystem.AddPopulation(population);
    }

    public void UdateAllPopulationStateForChecking()
    {
        foreach (Population population in this.ExistingPopulations)
        {
            population.UpdatePopulationStateForChecking();
        }
    }

    public void UpdateAccessibleLocations()
    {
        this.NeedSystemManager.UpdateAccessMap();
        foreach (Population population in this.Populations)
        {
            population.UpdateAccessibleArea(ReservePartitionManager.GetLocationsWithAccess(population),
            GridSystem.GetGridWithAccess(population));
        }
    }
}
