using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO create QueueChangeClass which gets invoked by Initialize and nextDay to calculate any changes
public class TimeSystem : MonoBehaviour
{
    [SerializeField] ReserveDraft ReserveDraft = default;
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    [SerializeField] PopulationManager PopulationManager = default;


    public void nextDay()
    {
        this.ReserveDraft.loadDraft();
        // Recalculates need system values and should updates all populations needs
        this.NeedSystemManager.UpdateSystems();
        this.PopulationManager.UpdateAccessibleLocations();
        // Calculate each populations growth and 
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.UpdateGrowthConditions();
        }
    }
}
