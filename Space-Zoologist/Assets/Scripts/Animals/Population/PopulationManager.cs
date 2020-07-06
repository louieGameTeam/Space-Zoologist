using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private GameObject PopulationPrefab = default;
    [SerializeField] List<GameObject> Populations = default;

    public void Start()
    {
        foreach (GameObject population in this.Populations)
        {
            if (population.activeSelf)
            {
                this.SetupExistingPopulation(population.GetComponent<Population>());
            }
        }
    }

    /// <summary>
    /// Create a new population of the given species at the given origin.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin point of the population</param>
    /// TODO refactor into a population data object so more aspects can be passed in and defined (e.g., initial conditions or behaviors)
    public void CreatePopulation(Species species, Vector2Int origin, int populationSize)
    {
        GameObject newPopulation = Instantiate(this.PopulationPrefab, this.gameObject.transform);
        newPopulation.name = species.SpeciesName;
        Population newPop = newPopulation.GetComponent<Population>();
        newPop.InitializeNewPopulation(species, origin, populationSize);
        this.Populations.Add(newPopulation);
        foreach (SpeciesNeed need in species.Needs)
        {
            needSystemManager.RegisterPopulation(newPop, need.Name);
        }
    }

    // TODO drag and drop can be avoided with find with tag or object to type
    public void SetupExistingPopulation(Population population)
    {
        population.InitializePopulationData();
        population.InitializeExistingAnimals();
        foreach (SpeciesNeed need in population.Species.Needs)
        {
            needSystemManager.RegisterPopulation(population, need.Name);
        }
    }
}
