using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    private List<Population> populations = new List<Population>();
    [SerializeField] private NeedSystemManager needSystemManager = default;

    // For demo
    public int popListSize { get => this.populations.Count; }
    // end

    private void Start()
    {
        // Add any populations that existed at start time.
        this.populations.AddRange(FindObjectsOfType<Population>());

        foreach (Population pop in this.populations)
        {
            foreach (SpeciesNeed need in pop.Species.Needs)
            {
                this.needSystemManager.UpdateSystem(need.Type);
            }
        }
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
        gameObject.GetComponent<Population>().Initialize(species, origin, needSystemManager);

        // Manually add new pop to list
        this.populations.Add(gameObject.GetComponent<Population>());
        foreach (SpeciesNeed need in gameObject.GetComponent<Population>().Species.Needs)
        {
            this.needSystemManager.UpdateSystem(need.Type);
        }
    }
}
