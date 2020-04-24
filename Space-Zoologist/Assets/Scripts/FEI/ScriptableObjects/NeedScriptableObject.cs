using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum PlantNeedType { Terrain, RLiquid, YLiquid, BLiquid, GasX, GasY, GasZ, Temperature }

/// <summary>
/// General Need object imported from the PopGrowth branch.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Food/Need", order = 1)]
public class NeedScriptableObject : ScriptableObject, IComparable<NeedScriptableObject>
{
	[SerializeField] private PlantNeedType type = default;
	public PlantNeedType Type { get => type; }
	[Range(1.0f, 10.0f)]
	[SerializeField] private int severity = 1;
	public int Severity { get => severity; }
    //public enum NeedCondition { Bad, Neutral, Good } as defined in SpeciesNeed.cs
    [SerializeField] private List<NeedCondition> conditions = default;
	[SerializeField] private List<float> thresholds = default;

    /// <summary>
    /// Compares a value with the condition thresholds and returns the associated condition.
    /// </summary>
    /// <param name="value">The value to compare to the need thresholds</param>
    /// <returns></returns>
    public NeedCondition GetCondition(float value)
    {
        // If there is only one condition, return it.
        if (conditions.Count == 1) return conditions[0];
        
        NeedCondition needCondition = NeedCondition.Bad;

        // Below or above threshold.
        if (value <= thresholds[0])
        {
            needCondition = conditions[0];
        }
        else if (value >= thresholds[thresholds.Count - 1])
        {
            needCondition = conditions[thresholds.Count];
        }
        // Between threshold values.
        for (int i = 0; i < thresholds.Count - 1; i++)
        {
            if ((value >= thresholds[i]) && (value < thresholds[i + 1]))
            {
                needCondition = conditions[i+1];
            }
        }
        return needCondition;
    }

    public void OnValidate()
    {
        if (conditions.Count == 0)
        {
            conditions.Add(NeedCondition.Good);
        }

        while (conditions.Count < thresholds.Count + 1)
        {
            thresholds.RemoveAt(thresholds.Count - 1);
        }
        while (conditions.Count > thresholds.Count + 1)
        {
            if (thresholds.Count == 0)
            {
                thresholds.Add(0);
            }
            else
            {
                thresholds.Add(thresholds[thresholds.Count - 1] + 1);
            }
        }

        for (var i = 0; i < thresholds.Count - 1; i++)
        {
            if (thresholds[i + 1] <= thresholds[i])
            {
                thresholds[i + 1] = thresholds[i] + 1;
            }
        }

    }


    //to be sorted in FSO
    public int CompareTo(NeedScriptableObject other) {
        if (type < other.type)
        {
            return -1;
        }
        else if (type > other.type)
        {
            return 1;
        }
        else { //type == other.type
            if (severity < other.severity)
            {
                return -1;
            }
            else if (severity > other.severity)
            {
                return 1;
            }
            else //type and severity are both equal
            {
                return 0;
            }
        }
    }
}
