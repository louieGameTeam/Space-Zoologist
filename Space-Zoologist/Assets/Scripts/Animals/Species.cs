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
    [SerializeField] private List<SpeciesNeed> _needs = default;
    public List<SpeciesNeed> Needs { get => _needs; }
    [SerializeField] private Sprite sprite = default;
    public Sprite Sprite { get => sprite; private set => sprite = value; }

    public NeedCondition GetNeedCondition(string needName, float status)
    {
        foreach(SpeciesNeed need in _needs)
        {
            if (need.NeedName == needName)
            {
                return need.GetCondition(status);
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }

    public float GetNeedSeverity(string needName)
    {
        foreach (SpeciesNeed need in _needs)
        {
            if (need.NeedName == needName)
            {
                return need.Severity;
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }
}
