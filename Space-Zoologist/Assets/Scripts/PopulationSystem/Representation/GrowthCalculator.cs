using System.Collections.Generic;
using UnityEngine;

public enum GrowthStatus {declining, stagnate, growing}

/// <summary>
/// Determines the rate and status of growth for each population based off of their most severe need that isn't being met
/// </summary>
public class GrowthCalculator
{
    public GrowthStatus GrowthStatus { get; private set; }
    public Dictionary<NeedType, NeedCondition> NeedTracker = new Dictionary<NeedType, NeedCondition>();
    public int GrowthCountdown = 0;
    public int DecayCountdown = 0;
    Population Population = default;

    public GrowthCalculator(Population population)
    {
        this.Population = population;
        this.GrowthCountdown = population.Species.GrowthRate;
        this.DecayCountdown = population.Species.DecayRate;
        this.GrowthStatus = GrowthStatus.stagnate;
    }

    public void setupNeedTracker(NeedType need)
    {
        if (NeedTracker.ContainsKey(need))
        {
            return;
        }
        NeedTracker.Add(need, NeedCondition.Bad);
    }

    /*
        1. if any needs of a type are good, then need is satisfied
        2. reset NeedTracker and udpate growth status
    */
    public void CalculateGrowth()
    {
        this.GrowthStatus = GrowthStatus.growing;
        // 1.
        foreach (KeyValuePair<string, Need> need in Population.Needs)
        {
            if (NeedTracker.ContainsKey(need.Value.NeedType))
            {
                NeedCondition needCondition = need.Value.GetCondition(need.Value.NeedValue);
                if (needCondition.Equals(NeedCondition.Good))
                {
                    Debug.Log(need.Value.NeedName + " is being met");
                    NeedTracker[need.Value.NeedType] = NeedCondition.Good;
                }
            }
        }
        // 2.
        Dictionary<NeedType, NeedCondition> resetNeedTracker = new Dictionary<NeedType, NeedCondition>(NeedTracker);
        foreach (KeyValuePair<NeedType, NeedCondition> need in NeedTracker)
        {
            if (NeedTracker[need.Key].Equals(NeedCondition.Bad))
            {
                GrowthStatus = GrowthStatus.declining;
            }
            resetNeedTracker[need.Key] = NeedCondition.Bad;
        }
        NeedTracker = resetNeedTracker;
    }

    //private void HandleBadCondition(KeyValuePair<string, Need> need)
    //{
    //    GrowthStatus = GrowthStatus.declining;
    //    this.GrowthCountdown = Population.Species.GrowthRate;
    //    needTimers[need.Key]--;
    //}

    //private void HandleGoodCondition(KeyValuePair<string, Need> need)
    //{
    //    needTimers[need.Key] = need.Value.Severity;
    //}

    //public int NumAnimalsToRemove()
    //{
    //    int numAnimals = 0;
    //    var needTimerCopy = new Dictionary<string, int>(this.needTimers);
    //    foreach (KeyValuePair<string, int> needTimer in needTimerCopy)
    //    {
    //        if (needTimer.Value == 0)
    //        {
    //            numAnimals++;
    //            needTimers[needTimer.Key] = Population.Needs[needTimer.Key].Severity;
    //        }
    //    }
    //    return numAnimals;
    //}

    public bool ReadyForDecay()
    {
        this.DecayCountdown--;
        if (this.DecayCountdown == 0)
        {
            this.DecayCountdown = Population.Species.GrowthRate;
            return true;
        }
        return false;
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
