using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Species : ScriptableObject
{
    [SerializeField] private string speciesName = default;
    public string SpeciesName { get { return speciesName; } }
    [Range(0.0f, 10.0f)]
    [SerializeField] private float dominance = default;
    public float Dominance { get => dominance; }
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;
    public float GrowthFactor { get => growthFactor; }
    [SerializeField] private List<SpeciesNeed> needs = default;
    public List<SpeciesNeed> Needs { get => needs; }
    [SerializeField] private Sprite sprite = default;
    public Sprite Sprite { get => sprite; }

    public NeedCondition GetNeedCondition(NeedType needType, float status)
    {
        foreach(SpeciesNeed need in needs)
        {
            if (need.Type == needType)
            {
                return need.GetCondition(status);
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }

    public float GetNeedSeverity(NeedType needType)
    {
        foreach (SpeciesNeed need in needs)
        {
            if (need.Type == needType)
            {
                return need.Severity;
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }
}
