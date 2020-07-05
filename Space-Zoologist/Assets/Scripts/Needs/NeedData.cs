using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: need to find a way to make this more non-tech-friendly
public enum NeedType { SpaceMaple, Density, Dirt, Sand, GasX, Strot };
public enum NeedCondition { Bad, Neutral, Good }


/// <summary>
/// A data object that holds the information to create a Need object.
/// </summary>
/// <remarks>
/// 
/// </remarks>
[System.Serializable]
public class NeedData
{
    public string NeedName => needName.ToString();
    public int Severity => severity;
    public List<NeedCondition> Conditions => conditions;
    public List<float> Thresholds => thresholds;

    [SerializeField] private NeedType needName = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int severity = 1;
    [SerializeField] private List<NeedCondition> conditions = default;
    [SerializeField] private List<float> thresholds = default;
}
