using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class SpeciesNeed : ScriptableObject
{
    [SerializeField] private string needName = default;
    public string NeedName { get => needName; set => needName = value; }
    [Range(1.0f, 10.0f)]
    [SerializeField] private float severity = default;
    public float Severity { get => severity; set => severity = value; }
    [SerializeField] private List<float> thresholds = default;
    [SerializeField] private List<NeedCondition> conditions = default;

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

    public NeedCondition GetCondition(float value)
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
