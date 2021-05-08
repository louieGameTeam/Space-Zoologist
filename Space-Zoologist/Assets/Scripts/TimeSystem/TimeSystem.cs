using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO create QueueChangeClass which gets invoked by Initialize and nextDay to calculate any changes
public class TimeSystem : MonoBehaviour
{
    [SerializeField] ReserveDraft ReserveDraft = default;
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] Inspector Inspector = default;
    [SerializeField] Text CurrentDayText = default;
    private int currentDay = 1;

    private void Start()
    {
        UpdateDayText(currentDay);
    }

    public void nextDay()
    {
        Debug.Log("---NEXT DAY---");
        this.ReserveDraft.loadDraft();
        // Recalculates need system values and should updates all populations needs
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.HandleGrowth();
        }
        this.NeedSystemManager.UpdateAllSystems();
        this.PopulationManager.UpdateAccessibleLocations();
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.UpdateGrowthConditions();
        }
        this.Inspector.UpdateCurrentDisplay();
        UpdateDayText(++currentDay);
    }

    private void UpdateDayText(int day)
    {
        CurrentDayText.text = "Day " + day;
    }
}
