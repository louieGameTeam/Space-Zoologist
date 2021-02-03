using System.Collections.Generic;
using UnityEngine;

public enum GrowthStatus {declining, stagnate, growing}

/// <summary>
/// Determines the rate and status of growth for each population based off of their most severe need that isn't being met
/// </summary>
public class GrowthCalculator
{
    public GrowthStatus GrowthStatus { get; private set; }
    public float GrowthRate { get; private set; }
    public Dictionary<string, int> needTimers = new Dictionary<string, int>();
    public int deadAnimals = 0;

    public GrowthCalculator()
    {
        this.GrowthRate = 0;
        this.GrowthStatus = GrowthStatus.growing;
    }

    public void setupNeedTimer(string need, int severity)
    {
        needTimers.Add(need, severity);
    }

    /*
        reset need timer if good, countdown need timer if bad, if reach 0, animal removed on next day and countdown starts over
    */
    public void CalculateGrowth(Population population)
    {
        deadAnimals = 0;
        GrowthStatus = GrowthStatus.growing;
        foreach(KeyValuePair<string, Need> need in population.Needs)
        {
            switch(need.Value.GetCondition(need.Value.NeedValue))
            {
                case NeedCondition.Bad:
                    GrowthStatus = GrowthStatus.declining;
                    needTimers[need.Key]--;
                    if (needTimers[need.Key] == 0)
                    {
                        deadAnimals++;
                        needTimers[need.Key] = need.Value.Severity;
                    }
                    break;
                case NeedCondition.Good:
                    needTimers[need.Key] = need.Value.Severity;
                    break;
                default:
                    Debug.LogError("Needs incorrectly setup");
                    break;
            }
        }
    }
}
