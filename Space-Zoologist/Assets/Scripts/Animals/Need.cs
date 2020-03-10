using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenuAttribute(fileName = "Need", menuName = "AnimalPopulations/NewNeed")]
public class Need : ScriptableObject
{
    public enum NeedCondition { Bad = -1, Neutral = 0, Good = 1 }
    // Need a way to distinguish specific needs with generic needtypes
    [SerializeField] public string NeedType = default;
    [SerializeField] public string NeedName = default;
    [SerializeField] private List<float> thresholds = default;
    [SerializeField] private List<NeedCondition> conditions = default;
    [SerializeField] public float NeedSeverity = default;
    public NeedCondition needCondition { get; set; }

    public void OnValidate()
    {
        while (conditions.Count < thresholds.Count + 1)
        {
            conditions.Add(NeedCondition.Bad);
        }
        while (conditions.Count> thresholds.Count + 1)
        {
            conditions.RemoveAt(conditions.Count - 1);
        }

        for(var i = 0; i < thresholds.Count - 1; i++)
        {
            if (thresholds[i + 1] <= thresholds[i])
            {
                thresholds[i + 1] = thresholds[i] + 1;
            }
        }
        
    }

    public void UpdateValue(float value)
    {
        this.needCondition = DetermineNeedCondition(value);
    }

    private NeedCondition DetermineNeedCondition(float value)
    {
        NeedCondition needCondition = NeedCondition.Neutral;
        // Below or above threshold.
        if (value <= this.thresholds[0])
        {
            needCondition = this.conditions[0];
        }
        else if (value >= this.thresholds[this.thresholds.Count - 1])
        {
            needCondition = this.conditions[this.thresholds.Count - 1];
        }
        // Between threshold values.
        for (int i = 1; i < this.thresholds.Count - 2; i++)
        {
            if ((value >= this.thresholds[i]) && (value < this.thresholds[i + 1]))
            {
                needCondition = this.conditions[i];
            }
        }
        return needCondition;
    }

}
