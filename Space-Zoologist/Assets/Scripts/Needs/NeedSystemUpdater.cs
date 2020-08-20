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

    [SerializeField] ReservePartitionManager ReservePartitionManager = default;
    [SerializeField] GridSystem GridSystem = default;

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
            population.UnpauseAnimals();
        }
    }

    // TODO what should be done when a population is split?
    // if the population location is no longer on accessible area?
    public void UpdateAccessibleLocations()
    {
        //ReservePartitionManager.UpdateAccessMap();
        foreach (Population population in PopulationManager.Populations)
        {
            population.UpdateAccessibleArea(ReservePartitionManager.GetLocationsWithAccess(population), 
            GridSystem.GetGridWithAccess(population));
        }
    }

    // Temp update
    private void Update()
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
