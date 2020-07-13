using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateNeedsSystems : MonoBehaviour
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

    public void UnPauseAllAnimals()
    {
        foreach (Population population in this.AllAnimals.Populations)
        {
            foreach (GameObject animal in population.AnimalPopulation)
            {
                animal.GetComponent<MovementController>().IsPaused = false;
            }
        }
    }

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
