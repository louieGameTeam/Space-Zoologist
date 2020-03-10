using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// TODO: figure out how popSize should affect growth
// This could be a SO or just a serialable class

[CreateAssetMenuAttribute(fileName = "GrowthCalculator", menuName = "AnimalPopulations/NewGrowthCalculator")]
public class AnimalPopulationGrowth : ScriptableObject
{
    public enum PopGrowthStatus { Decreasing = -1, Stabilized = 0, Increasing = 1 }
    public float GrowthTime { get; set; }
    public PopGrowthStatus GrowthStatus { get; set; }
    private int PopSize;
    // 20 seconds
    [Header("Where growth rate should stop in seconds")]
    public float LowerThreshold = 20f;
    // 20 minutes
    public float UpperThreshold = 7200f;

    /* Setup so each species can respond to a different time threshold:
     * could be altered to have a more tapered decrease or increase
     * 
     */
    public void CalculateGrowth(List<Need> Needs, float growthTime, int popSize)
    {
        this.PopSize = popSize;
        float OverallNeedStatus = 0;
        foreach (Need need in Needs)
        {
            OverallNeedStatus += CalculateGrowthBasedOnNeed(need);
            growthTime += OverallNeedStatus;
        }
        // GrowthTime could use GrowthStatus for calculations.
        DetermineGrowthStatus(OverallNeedStatus);
        DetermineGrowthTime(growthTime);
    }

    private float CalculateGrowthBasedOnNeed(Need need)
    {
        float needWeight = need.NeedSeverity;
        // setup so enum values are -1, 0, or 1
        float needCondition = (float)need.needCondition;
        return needCondition * needWeight;
    }

    // Arbitrary thresholds, could modify.
    private void DetermineGrowthStatus(float overallNeedStatus)
    {
        if (overallNeedStatus > 0)
        {
            this.GrowthStatus = PopGrowthStatus.Increasing;
        }
        else if (overallNeedStatus <= 1 && overallNeedStatus >= -1)
        {
            this.GrowthStatus = PopGrowthStatus.Stabilized;
        }
        else
        {
            this.GrowthStatus = PopGrowthStatus.Decreasing;
        }
    }

    private void DetermineGrowthTime(float growthTime)
    {
        if (growthTime > this.UpperThreshold)
        {
            growthTime = this.UpperThreshold;
        }
        else if (growthTime < this.LowerThreshold)
        {
            growthTime = this.LowerThreshold;
        }
        this.GrowthTime = growthTime;
    }
}
