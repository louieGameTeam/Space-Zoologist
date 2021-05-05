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
    [Header("Attach All ScriptableObjects of Species Available on This Level Here")]
    [SerializeField] private AnimalSpecies[] Species = default;

    //private SpeciesNeedSystem speciesNeedSystem = null;
    //private SymbiosisNeedSystem symbiosisNeedSystem = null;

    public void Initialize()
    {
        //TODO load from save
        this.speciesNeedSystem = (SpeciesNeedSystem)NeedSystemManager.Systems[NeedType.Species];
        this.symbiosisNeedSystem = (SymbiosisNeedSystem)NeedSystemManager.Systems[NeedType.Symbiosis];
        

        //Old loading, load gameobjects from the scene
/*        GameObject[] populations = GameObject.FindGameObjectsWithTag("Population");
        foreach (GameObject population in populations)
        {
            this.ExistingPopulations.Add(population.GetComponent<Population>());
        }

        foreach (Population population in this.ExistingPopulations)
        {
            this.SetupExistingPopulation(population);
        }
        this.NeedSystemManager.UpdateAllSystems();*/
    }
    private AnimalSpecies LoadSpecies(string name)
    {
        foreach (AnimalSpecies animalSpecies in Species)
        {
            if (animalSpecies.name.Equals(name))
            {
                return animalSpecies;
            }
        }
        Debug.LogError("No animal match the name '" + name + "' can be found in the species list. Did you attach the AnimalSpecies ScriptableObjects to the Population Manager?");
        return null;
    }
    public SerializedPopulation[] Serialize()
    {
        GameObject[] populations = GameObject.FindGameObjectsWithTag("Population");
        SerializedPopulation[] serializedPopulations = new SerializedPopulation[populations.Length];
        for (int i = 0; i < populations.Length; i++)
        {
            AnimalSpecies animalSpecies = populations[i].GetComponent<Population>().species;
            GameObject[] animals = new GameObject[populations[i].transform.childCount];
            for (int j = 0; j < populations[i].transform.childCount; j++)
            {
                animals[i] = populations[i].transform.GetChild(j).gameObject;
            }
            serializedPopulations[i] = new SerializedPopulation(animalSpecies, animals);
        }
        return serializedPopulations;
    }
    public void Parse(SerializedPopulation[] serializedPopulations)
    {
        if (serializedPopulations == null)
        {
            Debug.LogWarning("No population found in save");
            return;
        }
        for (int i=0; i < serializedPopulations.Length;i++)
        {
            Vector3[] pos = SerializationUtils.ParseVector3(serializedPopulations[i].population.coords);
            this.CreatePopulation(this.LoadSpecies(serializedPopulations[i].population.name), serializedPopulations.Length, pos[0], pos);
        }
    }
    /// <summary>
    /// Create a new population of the given species at the given position.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="position">The origin point of the population</param>
    /// Can be simplified since count will always equal to positions
    public void CreatePopulation(AnimalSpecies species, int count, Vector3 position, Vector3[] positions = null)
    {
        // Create population
        GameObject newPopulationGameObject = Instantiate(this.PopulationPrefab, position, Quaternion.identity, this.transform);
        newPopulationGameObject.name = species.SpeciesName;
        Population population = newPopulationGameObject.GetComponent<Population>();
        this.ExistingPopulations.Add(population);
        // Initialize the basic population data, register the population, then initialize the animals and their behaviors
        population.InitializeNewPopulation(species, position, count, positions);
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
        EventManager.Instance.InvokeEvent(EventType.NewPopulation, population);
    }

    // Registers the population with all all of the systems that care about it
    private void HandlePopulationRegistration(Population population)
    {
        this.ReservePartitionManager.AddPopulation(population);
        population.UpdateAccessibleArea(this.ReservePartitionManager.GetLocationsWithAccess(population),
        this.GridSystem.GetGridWithAccess(population));
        this.GridSystem.HighlightHomeLocations();
        this.NeedSystemManager.RegisterWithNeedSystems(population);
        this.BehaviorPatternUpdater.RegisterPopulation(population);
        //this.speciesNeedSystem.AddPopulation(population);
        //this.symbiosisNeedSystem.AddPopulation(population);
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
