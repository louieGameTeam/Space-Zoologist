using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    // Getters
    public string SpeciesName => speciesName;
    public float Dominance => dominance;
    public float GrowthFactor => growthFactor;
    public List<Need> Needs => needs;
    public float Size => size;
    public List<TileType> AccessibleTerrain => accessibleTerrain;

    // Values
    [SerializeField] private string speciesName = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private int dominance = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;
    [SerializeField] private List<Need> needs = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;


    /// <summary>
    /// Get the condition of a need given its current value.
    /// </summary>
    /// <param name="needType">The need to get the condition of</param>
    /// <param name="value">The value of the need</param>
    /// <returns></returns>
    public NeedCondition GetNeedCondition(string needType, float value)
    {
        foreach(Need need in needs)
        {
            if (need.NeedName == needType)
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
    public float GetNeedSeverity(string needType)
    {
        foreach (Need need in needs)
        {
            if (need.NeedName == needType)
            {
                return need.Severity;
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }
}
