using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO create QueueChangeClass which gets invoked by Initialize and nextDay to calculate any changes
public class TimeSystem : MonoBehaviour
{
    //[SerializeField] PopulationManager PopulationManager = default;
    //[SerializeField] Inspector Inspector = default;
    //[SerializeField] Text CurrentDayText = default;
    //[SerializeField] BuildBufferManager buildBufferManager = default;
    //[SerializeField] int maxDay = 20;
    //private int currentDay = 1;
    //public bool LessThanMaxDay => currentDay <= maxDay;

    //private void Start()
    //{
    //    UpdateDayText(currentDay);
    //}

    //public void nextDay()
    //{
    //    this.buildBufferManager.CountDown();
    //    this.PopulationManager.UpdateAccessibleLocations();
    //    this.PopulationManager.UdateAllPopulationRegistration();
    //    this.NeedSystemManager.UpdateAllSystems();
    //    this.PopulationManager.UpdateAllGrowthConditions();
    //    bool populaitonDidGrow = false;
    //    for (int i=PopulationManager.Populations.Count - 1; i>= 0; i--)
    //    {
    //        if (PopulationManager.Populations[i].HandleGrowth())
    //        {
    //            populaitonDidGrow = true;
    //        }
    //    }
    //    if (populaitonDidGrow)
    //    {
    //        this.NeedSystemManager.UpdateAllSystems();
    //        this.PopulationManager.UpdateAllGrowthConditions();
    //    }
    //    this.Inspector.UpdateCurrentDisplay();
    //    UpdateDayText(++currentDay);
    //    EventManager.Instance.InvokeEvent(EventType.NextDay, this);
    //}

    //private void UpdateDayText(int day)
    //{
    //    CurrentDayText.text = "" + day;
    //    if (maxDay > 0)
    //    {
    //        CurrentDayText.text += " / " + maxDay;
    //    }
    //}
}
