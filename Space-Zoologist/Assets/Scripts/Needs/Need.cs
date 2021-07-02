using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Need
{
    public string NeedName => needName;
    public int Severity => severity;
    public NeedType NeedType => needType;
    public Sprite Sprite => sprite;
    public float NeedValue => this.needValue;
    public bool IsPreferred => isPreferred;
    public List<NeedBehavior> Behaviors => this.conditions;

    [SerializeField] private NeedType needType = default;
    [SerializeField] private string needName = default;
    [SerializeField] private float needValue = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int severity = 1;
    [SerializeField] bool isPreferred = default;
    [SerializeField] private List<NeedBehavior> conditions = default;
    [SerializeField] private List<float> thresholds = default;
    [SerializeField] private Sprite sprite = default;

    public Need(NeedType needType, NeedConstructData needConstructData)
    {
        this.needType = needType;
        this.needName = needConstructData.NeedName;
        this.severity = needConstructData.Severity;
        this.conditions = needConstructData.Conditions;
        this.thresholds = needConstructData.Thresholds;
        this.conditions = needConstructData.Conditions;
        this.isPreferred = needConstructData.IsPreferred;
    }

    /// <summary>
    /// Returns what condition the need is in based on the given need value.
    /// </summary>
    /// <param name="value">The value to compare to the need thresholds</param>
    /// <returns></returns>
    public NeedCondition GetCondition(float value)
    {
        // If there is only one condition, return it.
        if (conditions.Count == 1) return conditions[0].Condition;
        for (var i = 0; i < this.thresholds.Count; i++)
        {
            if (value + 0.1 < this.thresholds[i])
            {
                return this.conditions[i].Condition;
            }
        }
        return this.conditions[this.thresholds.Count].Condition;
    }
    public NeedBehavior GetBehavior(float value) {
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

    public float GetMaxThreshold()
    {
        return this.thresholds[this.thresholds.Count - 1];
    }

    public float GetMinThreshold()
    {
        return this.thresholds[0];
    }

    public void OnValidate()
    {
        if (conditions.Count == 0)
        {
            conditions.Add(new NeedBehavior(NeedCondition.Good));
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

    public void UpdateNeedValue(float value)
    {
        this.needValue = value;
    }
}
