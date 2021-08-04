﻿using System.Collections;
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
    [SerializeField] BuildBufferManager buildBufferManager = default;
    [SerializeField] int maxDay = 20;
    private int currentDay = 1;

    private void Start()
    {
        UpdateDayText(currentDay);
    }

    public void nextDay()
    {
        this.buildBufferManager.CountDown();
        this.PopulationManager.UpdateAccessibleLocations();
        this.PopulationManager.UdateAllPopulationRegistration();
        this.NeedSystemManager.UpdateAllSystems();
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.HandleGrowth();
        }
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.UpdateGrowthConditions();
        }
        this.Inspector.UpdateCurrentDisplay();
        UpdateDayText(++currentDay);

        // Fire next day event.
        EventManager.Instance.InvokeEvent(EventType.OnNextDay, null);
    }

    private void UpdateDayText(int day)
    {
        CurrentDayText.text = "" + day;
        if (maxDay > 0)
        {
            CurrentDayText.text += " / " + maxDay;
        }
    }

    // PUBLIC GETTER
    public int CurrentDay
    {
        get { return currentDay; }
    }
}
