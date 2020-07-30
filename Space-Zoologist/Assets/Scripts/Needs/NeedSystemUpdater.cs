using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hanldes the update of NS and pausing animal repersentation
/// </summary>
public class NeedSystemUpdater : MonoBehaviour
{
    public bool isInStore { get; set; }

    // Singleton
    public static NeedSystemUpdater ins;

    private NeedSystemsTester NSTester = default;  

    private void Awake()
    {
        isInStore = false;
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }
    }

    private void Start()
    {
        this.NSTester = FindObjectOfType<NeedSystemsTester>();
    }

    public void PauseAllAnimals()
    {
       foreach (Population population in PopulationManager.ins.Populations)
        {
            foreach (GameObject animal in population.AnimalPopulation)
            {
                animal.GetComponent<MovementController>().IsPaused = true;
            }
        }
    }

    public void UnpauseAllAnimals()
    {
        foreach (Population population in PopulationManager.ins.Populations)
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
        // TODO: temply commented out
        //ReservePartitionManager.ins.UpdateAccessMap();
        //foreach (Population population in PopulationManager.ins.Populations)
        //{
        //    population.UpdateAccessibleArea();
        //    foreach (GameObject animal in population.AnimalPopulation)
        //    {
        //        animal.GetComponent<Animal>().ResetBehavior();
        //    }
        //}
    }

    // Temp update
    private void FixedUpdate()
    {
        if(!isInStore)
        {
            NeedSystemManager.ins.UpdateSystems();
        }

        this.NSTester.Update();
    }
}
