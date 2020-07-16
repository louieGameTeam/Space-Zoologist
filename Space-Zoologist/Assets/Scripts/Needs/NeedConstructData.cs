using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Each NeedType holds a list of unique needs
public enum NeedType { Terrain, Atmosphere, Density, Food, Liquid, Species };
public enum NeedCondition { Bad, Neutral, Good }

[System.Serializable]
public class NeedTypeConstructData
{

    public NeedTypeConstructData(NeedType needType)
    {
        this.needType = needType;
        needs = new List<NeedConstructData>();
    }


    public NeedType NeedType => needType;
    public List<NeedConstructData> Needs => needs;

    [SerializeField] private NeedType needType = default;
    [SerializeField] List<NeedConstructData> needs = default;
}

/// <summary>
/// A data object that holds the information to create a Need object.
/// </summary>
[System.Serializable]
public class NeedConstructData
{
    public string NeedName => needName;
    public int Severity => severity;
    public List<NeedCondition> Conditions => conditions;
    public List<float> Thresholds => thresholds;

    [SerializeField] private string needName = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int severity = 1;
    [SerializeField] private List<NeedCondition> conditions = default;
    [SerializeField] private List<float> thresholds = default;

    public NeedConstructData(string name, int severity, List<string> conditions, List<float> thresholds)
    {
        this.needName = name;
        this.severity = severity;
        foreach(string condition in conditions)
        {
            if (NeedCondition.Good == (NeedCondition)Enum.Parse(typeof(NeedCondition), condition))
            {
                this.conditions.Add(NeedCondition.Good);
            }
            if (NeedCondition.Neutral == (NeedCondition)Enum.Parse(typeof(NeedCondition), condition))
            {
                this.conditions.Add(NeedCondition.Neutral);
            }
            if (NeedCondition.Bad == (NeedCondition)Enum.Parse(typeof(NeedCondition), condition))
            {
                this.conditions.Add(NeedCondition.Bad);
            }
        }
        this.thresholds = thresholds;
    }
}