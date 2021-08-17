using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Each NeedType holds a list of unique needs
public enum NeedType { Terrain, Atmosphere, Density, FoodSource, Liquid, Species, Temperature, Symbiosis };
public enum NeedCondition { Bad, Neutral, Good }

[System.Serializable]
public class TerrainNeedConstructData : NeedConstructData
{
    public TerrainNeedConstructData(string name, List<string> conditions, List<float> thresholds) 
        : base(name, conditions, thresholds)
    {
        
    }
}

[System.Serializable]
public class FoodNeedConstructData : NeedConstructData
{
    public FoodNeedConstructData(string name, List<string> conditions, List<float> thresholds) 
        : base(name, conditions, thresholds)
    {
        
    }
}

[System.Serializable]
public class LiquidNeedConstructData : NeedConstructData
{
    public LiquidNeedConstructData(string name, List<string> conditions, List<float> thresholds) 
        : base(name, conditions, thresholds)
    {
        
    }
}

/// <summary>
/// A data object that holds the information to create a Need object.
/// </summary>
[System.Serializable]
public abstract class NeedConstructData
{
    public string NeedName => needName;
    public List<NeedBehavior> Conditions => conditions;
    public List<float> Thresholds => thresholds;
    public bool IsPreferred => isPreferred;

    [SerializeField] private string needName = default;
    [SerializeField] private bool isPreferred = false;
    [SerializeField] private List<NeedBehavior> conditions = default;
    [SerializeField] private List<float> thresholds = default;

    public NeedConstructData(string name, List<string> conditions, List<float> thresholds)
    {
        this.conditions = new List<NeedBehavior>();
        this.thresholds = new List<float>();
        this.needName = name;
        foreach(string condition in conditions)
        {
            if (condition.Equals("Good", StringComparison.OrdinalIgnoreCase))
            {
                this.conditions.Add(new NeedBehavior(NeedCondition.Good));
            }
            if (condition.Equals("Neutral", StringComparison.OrdinalIgnoreCase))
            {
                this.conditions.Add(new NeedBehavior(NeedCondition.Neutral));
            }
            if (condition.Equals("Bad", StringComparison.OrdinalIgnoreCase))
            {
                this.conditions.Add(new NeedBehavior(NeedCondition.Bad));
            }
        }
        this.thresholds = thresholds;
    }
}