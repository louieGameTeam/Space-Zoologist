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

    private void Start()
    {
        populations.AddRange(FindObjectsOfType<Population>());
        foreach (Population population in populations)
        {
            needSystemManager.RegisterPopulationNeeds(population);
        }
    }

    /// <summary>
    /// Create a new population of the given species at the given origin.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin point of the population</param>
    public void CreatePopulation(AnimalSpecies species, int count, Vector3 position)
    {
        GameObject newPopulationGameObject = Instantiate(populationGameObject, position, Quaternion.identity, this.transform);
        newPopulationGameObject.name = species.SpeciesName;
        Population population = newPopulationGameObject.GetComponent<Population>();
        population.Initialize(species, position, count);
        this.populations.Add(population);
        needSystemManager.RegisterPopulationNeeds(population);
        rpm.AddPopulation(population);
    }

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
