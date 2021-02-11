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
    public int GrowthCountdown = 0;
    Population Population = default;

    public GrowthCalculator(Population population)
    {
        this.Population = population;
        this.GrowthCountdown = population.Species.GrowthRate;
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
    public void CalculateGrowth()
    {
        this.GrowthStatus = GrowthStatus.growing;
        foreach (KeyValuePair<string, Need> need in Population.Needs)
        {
            switch(need.Value.GetCondition(need.Value.NeedValue))
            {
                case NeedCondition.Bad:
                    HandleBadCondition(need);
                    break;
                case NeedCondition.Good:
                    HandleGoodCondition(need);
                    break;
                default:
                    Debug.LogError("Needs incorrectly setup");
                    break;
            }
        }
    }

    private void HandleBadCondition(KeyValuePair<string, Need> need)
    {
        GrowthStatus = GrowthStatus.declining;
        this.GrowthCountdown = Population.Species.GrowthRate;
        needTimers[need.Key]--;
    }

    private void HandleGoodCondition(KeyValuePair<string, Need> need)
    {
        needTimers[need.Key] = need.Value.Severity;
    }

    public int NumAnimalsToRemove()
    {
        int numAnimals = 0;
        var needTimerCopy = new Dictionary<string, int>(this.needTimers);
        foreach (KeyValuePair<string, int> needTimer in needTimerCopy)
        {
            if (needTimer.Value == 0)
            {
                numAnimals++;
                needTimers[needTimer.Key] = Population.Needs[needTimer.Key].Severity;
            }
        }
        return numAnimals;
    }

    public bool ReadyForGrowth()
    {
        this.GrowthCountdown--;
        if (this.GrowthCountdown == 0)
        {
            this.GrowthCountdown = Population.Species.GrowthRate;
            return true;
        }
        return false;
    }
}
