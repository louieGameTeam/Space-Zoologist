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
            foreach (GameObject animal in population.AnimalPopulation)
            {
                animal.GetComponent<MovementController>().IsPaused = true;
            }
        }
    }

    public void UnpauseAllAnimals()
    {
        foreach (Population population in PopulationManager.Populations)
        {
            foreach (GameObject animal in population.AnimalPopulation)
            {
                animal.GetComponent<MovementController>().IsPaused = false;
            }
        }
    }

    // TODO what should be done when a population is split?
    // if the population location is no longer on accessible area?
    public void UpdateAccessibleLocations()
    {

        ReservePartitionManager.UpdateAccessMap();
        foreach (Population population in PopulationManager.Populations)
        {
            population.UpdateAccessibleArea();
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

        this.needSystemsTester.Update();
    }
}
