using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Species : ScriptableObject
{
    // Getters
    public string SpeciesName { get { return speciesName; } }
    public float Dominance { get => dominance; }
    public float GrowthFactor { get => growthFactor; }
    public List<SpeciesNeed> Needs { get => needs; }
    public Sprite Sprite { get => sprite; }

    public float Size;
    public List<TileType> accessibleTerrain;

    // Values
    [SerializeField] private string speciesName = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float dominance = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;
    [SerializeField] private List<SpeciesNeed> needs = default;
    [SerializeField] private Sprite sprite = default;

    /// <summary>
    /// Get the condition of a need given its current value.
    /// </summary>
    /// <param name="needType">The need to get the condition of</param>
    /// <param name="value">The value of the need</param>
    /// <returns></returns>
    public NeedCondition GetNeedCondition(NeedType needType, float value)
    {
        foreach(SpeciesNeed need in needs)
        {
            if (need.Type == needType)
            {
                return need.GetCondition(value);
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }

    /// <summary>
    /// Get the severity of a given need.
    /// </summary>
    /// <param name="needType">The need to get the severity of</param>
    /// <returns></returns>
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
