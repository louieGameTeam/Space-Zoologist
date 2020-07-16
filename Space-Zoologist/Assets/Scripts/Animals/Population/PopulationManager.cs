using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The manager of all the population instances
/// </summary>
public class PopulationManager : MonoBehaviour
{
    // Singleton
    public static PopulationManager ins;

    // FindObjectOfType<Population> to populate
    private List<Population> ExistingPopulations = new List<Population>();
    public List<Population> Populations => ExistingPopulations;

    [SerializeField] private GameObject PopulationPrefab = default;
    [SerializeField] private LevelData levelData = default;

    private Dictionary<string, SpeciesNeedSystem> speciesNeedSystems = new Dictionary<string, SpeciesNeedSystem>();
    // AnimalSpecies to string name
    private Dictionary<string, AnimalSpecies> animalSpecies = new Dictionary<string, AnimalSpecies>();

    private void Awake()
    {
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }

        // Add new FoodSourceNeedSystem
        if (levelData != null)
        {
            // Fill string to AnimalSpecies Dictionary
            foreach (AnimalSpecies species in levelData.AnimalSpecies)
            {
                animalSpecies.Add(species.SpeciesName, species);
            }
        }
    }

    private void Start()
    {
        ExistingPopulations.AddRange(FindObjectsOfType<Population>());

        // Get the SpeicesNeedSystems from NeedSystemManager
        foreach (NeedSystem system in NeedSystemManager.ins.Systems.Values)
        {
            if (animalSpecies.ContainsKey(system.NeedName))
            {
                speciesNeedSystems.Add(system.NeedName, (SpeciesNeedSystem)system);
            }
        }

        // Register with manager
        //foreach (Population population in ExistingPopulations)
        //{
        //    NeedSystemManager.ins.RegisterWithNeedSystems(population);
        //    Debug.Log($"Registering {population.Species.SpeciesName} with NS at Start");
        //}

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
        Debug.Log("Population created");
        GameObject newPopulationGameObject = Instantiate(this.PopulationPrefab, position, Quaternion.identity, this.transform);
        newPopulationGameObject.name = species.SpeciesName;
        Population population = newPopulationGameObject.GetComponent<Population>();
        population.InitializeNewPopulation(species, position, count);
        this.ExistingPopulations.Add(population);
        ReservePartitionManager.ins.AddPopulation(population);
        speciesNeedSystems[population.Species.SpeciesName].AddPopulation(population);

        // Register with NeedSystemManager
        NeedSystemManager.ins.RegisterWithNeedSystems(population);
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

        List<Population> localPopulations = ReservePartitionManager.ins.GetPopulationsWithAccessTo(position);
        Population preexistingPopulation = localPopulations.Find(p => p.Species == species);
        if (preexistingPopulation)
        {
            Debug.Log("Preexisting population");
            preexistingPopulation.AddAnimal(new BehaviorsData());
        } // TODO: update systems related to population count change ^
        else
        {
            CreatePopulation(species, count, position);
        } // TODO: update systems related to new population
    }

    // TODO how to register with need system
    public void SetupExistingPopulation(Population population)
    {
        population.InitializePopulationData();
        population.InitializeExistingAnimals();

        // Register with NS
        NeedSystemManager.ins.RegisterWithNeedSystems(population);
        Debug.Log($"Registering {population.Species.SpeciesName} with NS at Start");
    }
}
