using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    private List<Population> populations = new List<Population>();
    [SerializeField] private NeedSystemManager needSystemManager = default;

    private void Start()
    {
        // Add any populations that existed at start time.
        this.populations.AddRange(FindObjectsOfType<Population>());

    }

    /// <summary>
    /// Create a new population of the given species at the given origin.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin point of the population</param>
    public void CreatePopulation(Species species, Vector2Int origin)
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.parent = this.transform;
        gameObject.name = species.SpeciesName;
        gameObject.AddComponent<Population>();
        Population population = gameObject.GetComponent<Population>();
        population.Initialize(species, origin);
    }
}
