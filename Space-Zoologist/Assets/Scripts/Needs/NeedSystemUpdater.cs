using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hanldes the update of NS and pausing animal repersentation
/// </summary>
public class NeedSystemUpdater : MonoBehaviour
{
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] NeedSystemsTester needSystemsTester = default;
    [SerializeField] ReservePartitionManager ReservePartitionManager = default;

    public bool isInStore { get; set; }

    public void PauseAllAnimals()
    {
       foreach (Population population in PopulationManager.Populations)
        {
            population.PauseAnimals();
        }
    }

    public void UnpauseAllAnimals()
    {
        foreach (Population population in PopulationManager.Populations)
        {
            if (population.IssueWithAccessibleArea)
            {
                continue;
            }
            population.UnpauseAnimals();
        }
    }

    // TODO what should be done when a population is split?
    // if the population location is no longer on accessible area?
    public void UpdateAccessibleLocations()
    {
        ReservePartitionManager.UpdateAccessMap();
        foreach (Population population in PopulationManager.Populations)
        {
            population.UpdateAccessibleArea(ReservePartitionManager.GetLocationsWithAccess(population),
            ReservePartitionManager.GetGridWithAccess(population));
            foreach (GameObject animal in population.AnimalPopulation)
            {
                animal.GetComponent<Animal>().ResetBehavior();
            }
        }
    }

    // Temp update
    private void FixedUpdate()
    {
        if(!isInStore)
        {
            NeedSystemManager.UpdateSystems();
        }
        else
        {
            // continually pauses all animals in case any are added. Will need a better way to handle this once behavior framework figured out.
            this.PauseAllAnimals();
        }
    }
}
