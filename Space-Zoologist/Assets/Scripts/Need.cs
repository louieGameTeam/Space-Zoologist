using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NeedCondition { Bad, Neutral, Good }

[CreateAssetMenu]
public class Need : ScriptableObject
{
    public string NeedName => needName;
    public int Severity => severity;

    [SerializeField] private string needName = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int severity = 1;
    [SerializeField] private List<NeedCondition> conditions = default;
    [SerializeField] private List<float> thresholds = default;

    /// <summary>
    /// Returns what condition the need is in based on the given need value.
    /// </summary>
    /// <param name="value">The value to compare to the need thresholds</param>
    /// <returns></returns>
    public NeedCondition GetCondition(float value)
    {
        // If there is only one condition, return it.
        if (conditions.Count == 1) return conditions[0];

        for (var i = 0; i < this.thresholds.Count; i++)
        {
            if (value < this.thresholds[i])
            {
                return this.conditions[i];
            }
        }

        return this.conditions[this.thresholds.Count];
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
}
