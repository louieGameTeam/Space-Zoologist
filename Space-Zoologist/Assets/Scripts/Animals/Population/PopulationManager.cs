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
    [SerializeField] private LevelDataReference LevelDataReference = default;
    [SerializeField] private GameObject PopulationPrefab = default;
    // What is this doing?
    //private Dictionary<string, SpeciesNeedSystem> speciesNeedSystems = new Dictionary<string, SpeciesNeedSystem>();

    public void Initialize()
    {
        GameObject[] populations = GameObject.FindGameObjectsWithTag("Population");
        foreach (GameObject population in populations)
        {
            this.ExistingPopulations.Add(population.GetComponent<Population>());
        }
        this.UnclearFunction();

        foreach (Population population in this.ExistingPopulations)
        {
            this.SetupExistingPopulation(population);
        }
    }

    private void UnclearFunction()
    {
        // TODO what is this doing?
        Dictionary<string, AnimalSpecies> animalSpeciesMapping = new Dictionary<string, AnimalSpecies>();
        if (LevelDataReference.LevelData != null)
        {
            // Fill string to AnimalSpecies Dictionary
            foreach (AnimalSpecies species in LevelDataReference.LevelData.AnimalSpecies)
            {
                animalSpeciesMapping.Add(species.SpeciesName, species);
            }
        }
        // foreach (NeedSystem system in NeedSystemManager.ins.Systems.Values)
        // {
        //     if (animalSpeciesMapping.ContainsKey(system.NeedName))
        //     {
        //         speciesNeedSystems.Add(system.NeedName, (SpeciesNeedSystem)system);
        //     }
        // }
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
        population.InitializeNewPopulation(species, position, count, NeedSystemManager);
        this.ExistingPopulations.Add(population);
        this.HandlePopulationRegistration(population);
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
        }
        else
        {
            CreatePopulation(species, count, position);
        }
    }

    // TODO determine what else needs to happen with an existing population
    private void SetupExistingPopulation(Population population)
    {
        population.InitializePopulationData(NeedSystemManager);
        population.InitializeExistingAnimals();
        this.HandlePopulationRegistration(population);
    }

    // TODO figure out a better name for what this function is doing: registering the population with various lists and systems
    private void HandlePopulationRegistration(Population population)
    {
        ReservePartitionManager.ins.AddPopulation(population);
        // speciesNeedSystems[population.Species.SpeciesName].AddPopulation(population);
        NeedSystemManager.RegisterWithNeedSystems(population);
    }
}
