using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public abstract class Need
{
    public string NeedName => needName;
    public int Severity => severity;
    public NeedType NeedType => GetNeedType();
    public Sprite Sprite => sprite;
    public float NeedValue => this.needValue;
    public bool IsPreferred => isPreferred;
    public List<NeedBehavior> Behaviors => this.conditions;

    [SerializeField] private string needName = default;
    [SerializeField] private float needValue = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int severity = 1;
    [SerializeField] bool isPreferred = default;
    [SerializeField] private List<NeedBehavior> conditions = default;
    [SerializeField] private List<float> thresholds = default;
    [SerializeField] private Sprite sprite = default;

    protected abstract NeedType GetNeedType();

    protected Need(NeedConstructData needConstructData)
    {
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

    public float GetThresholdForFirstGoodCondition() {
        for (int i = 1; i < conditions.Count; i++) {
            if (conditions[i].Condition == NeedCondition.Good) {
                return thresholds[i - 1];
            }
        }
        return 0.01f;
    }

    // TODO what is this doing
    /// <summary>
    /// 
    /// </summary>
    /// <param name="needCondition"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public float GetThreshold(NeedCondition needCondition, int occurrence = 0, bool top = true)
    {
        int count = 0;
        foreach(NeedBehavior needBehavior in this.conditions)
        {
            if (needBehavior.Condition.Equals(needCondition))
            {
                count++;
            }
        }
        if (count == 0)
        {
            throw new System.ArgumentException($"Tried to access {needCondition.ToString()} condition in {needName} need, but the need does not have the condition.");
        }

        // Get the number of occurrences of the specified need condition in the need's list of need conditions
        int numOccurrences = count;
        int sign = System.Math.Sign(occurrence);
        int occurrenceIndex = sign < 0 ? numOccurrences + occurrence : occurrence;
        occurrenceIndex = Mathf.Clamp(occurrenceIndex, 0, numOccurrences);
        List<int> conditionIndices = new List<int>();
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].Condition == needCondition)
            {
                conditionIndices.Add(i);
            }
        }
        int thresholdIndex = top ? conditionIndices[occurrenceIndex] : conditionIndices[occurrenceIndex] - 1;
        if (thresholdIndex < 0 || thresholdIndex >= thresholds.Count)
        {
            return float.PositiveInfinity * System.Math.Sign(thresholdIndex);
        }
        else
        {
            return thresholds[thresholdIndex];
        }
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
