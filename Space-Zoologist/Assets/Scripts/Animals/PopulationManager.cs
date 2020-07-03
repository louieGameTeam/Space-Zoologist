using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public List<Population> Populations => populations;

    private List<Population> populations = new List<Population>();
    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private GameObject populationGameObject = default;
    [SerializeField] private ReservePartitionManager rpm = default;
    //[SerializeField] private PopulationDensitySystem populationDensitySystem = default;

    [SerializeField] private LevelData levelData = default;

    private Dictionary<AnimalSpecies, SpeciesNeedSystem> speciesNeedSystems = new Dictionary<AnimalSpecies, SpeciesNeedSystem>();

    private void Awake()
    {
        // Add new FoodSourceNeedSystem
        foreach (AnimalSpecies animalSpecies in levelData.AnimalSpecies)
        {
            SpeciesNeedSystem speciesNeedSystem = new SpeciesNeedSystem(animalSpecies.SpeciesName, rpm);
            speciesNeedSystems.Add(animalSpecies, speciesNeedSystem);
        }
    }

    private void Start()
    {
        populations.AddRange(FindObjectsOfType<Population>());

        // Add SpeicesNeedSystems to NeedSystemManager
        foreach (SpeciesNeedSystem speciesNeedSystem in speciesNeedSystems.Values)
        {
            needSystemManager.AddSystem(speciesNeedSystem);
        }

        // Register with manager
        foreach (Population population in populations)
        {
            needSystemManager.RegisterWithNeedSystems(population);
        }
    }

    /// <summary>
    /// Create a new population of the given species at the given position.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="position">The origin point of the population</param>
    public void CreatePopulation(AnimalSpecies species, int count, Vector3 position)
    {
        GameObject newPopulationGameObject = Instantiate(populationGameObject, position, Quaternion.identity, this.transform);
        newPopulationGameObject.name = species.SpeciesName;
        Population population = newPopulationGameObject.GetComponent<Population>();
        population.Initialize(species, position, count);
        this.populations.Add(population);
        rpm.AddPopulation(population);
        speciesNeedSystems[population.Species].AddPopulation(population);

        // Register with NeedSystemManager
        needSystemManager.RegisterWithNeedSystems(population);
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

        List<Population> localPopulations = rpm.GetPopulationsWithAccessTo(position);
        Population preexistingPopulation = localPopulations.Find(p => p.Species == species);
        if (preexistingPopulation)
        {
            preexistingPopulation.AddAnimals(count);
        } // TODO: update systems related to population count change ^
        else
        {
            CreatePopulation(species, count, position);
        } // TODO: update systems related to new population
    }
}
