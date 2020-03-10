using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenuAttribute(fileName = "Need", menuName = "AnimalPopulations/NewNeed")]
public class Need : ScriptableObject
{
    public enum NeedCondition { Bad = -1, Neutral = 0, Good = 1 }
    [SerializeField] public string NeedType = default;
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
        // If it's completely below or above the range value, something should probably happen?
        //if (value <= this.thresholds[0])
        //{
        //    // UpdatedValue = something;
        //}
        //else if (value >= this.thresholds[this.thresholds.Count - 1])
        //{
        //    // UpdatedValue = something;
        //}
        this.needCondition = DetermineNeedCondition(value);
    }

    private NeedCondition DetermineNeedCondition(float value)
    {
        NeedCondition needCondition = NeedCondition.Neutral;
        for (int i = 0; i < this.thresholds.Count - 1; i++)
        {
            if ((value >= this.thresholds[i]) && (value < this.thresholds[i + 1]))
            {
                needCondition = this.conditions[i];
            }
        }
        return needCondition;
    }

}
