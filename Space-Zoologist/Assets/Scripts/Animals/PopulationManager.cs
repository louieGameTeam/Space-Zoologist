using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    private List<Population> populations = new List<Population>();
    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private GameObject PopulationPrefab = default;

    /// <summary>
    /// Create a new population of the given species at the given origin.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin point of the population</param>
    public void CreatePopulation(Species species, Vector2Int origin)
    {
        GameObject newPopulation = Instantiate(this.PopulationPrefab, this.gameObject.transform);
        newPopulation.name = species.SpeciesName;
        newPopulation.GetComponent<Population>().Initialize(species, origin);
        foreach (SpeciesNeed need in species.Needs)
        {
            needSystemManager.RegisterPopulation(newPopulation.GetComponent<Population>(), need.Name);
        }
    }
}
