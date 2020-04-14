using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Species : ScriptableObject
{
    // Getters
    public string SpeciesName => speciesName;
    public float Dominance => dominance;
    public float GrowthFactor => growthFactor;
    public List<SpeciesNeed> Needs => needs;
    public float Size => size;

    // Values
    [SerializeField] private string speciesName = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float dominance = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;
    [SerializeField] private List<SpeciesNeed> needs = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    public List<TileType> accessibleTerrain;


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
