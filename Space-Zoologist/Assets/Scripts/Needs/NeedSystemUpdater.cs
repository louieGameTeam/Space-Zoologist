using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedSystemUpdater : MonoBehaviour
{
    [SerializeField] PopulationManager AllAnimals = default;

    public void PauseAllAnimals()
    {
        foreach (Population population in this.AllAnimals.Populations)
        {
            foreach (GameObject animal in population.AnimalPopulation)
            {
                animal.GetComponent<MovementController>().IsPaused = true;
            }
        }
    }

    public void UnpauseAllAnimals()
    {
        foreach (Population population in this.AllAnimals.Populations)
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
        ReservePartitionManager.ins.UpdateAccessMap();
        foreach (Population population in this.AllAnimals.Populations)
        {
            population.UpdateAccessibleArea();
            foreach (GameObject animal in population.AnimalPopulation)
            {
                animal.GetComponent<Animal>().ResetBehavior();
            }
        }
    }
}
