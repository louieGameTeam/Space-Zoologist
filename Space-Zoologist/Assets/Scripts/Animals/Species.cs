using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Species : ScriptableObject
{
    // Getters
    public string SpeciesName => speciesName;
    public float Dominance => dominance;
    public float GrowthFactor => growthFactor;
    public List<SpeciesNeed> Needs => needs;
    public float Size => size;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public Sprite Sprite => sprite;

    // Values
    [SerializeField] private string speciesName = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float dominance = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;
    [SerializeField] private List<SpeciesNeed> needs = default;
    [Header("One behavior set for each unique need type")]
    [SerializeField] private List<Behaviors> BehaviorSets = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private Sprite sprite = default;


    /// <summary>
    /// Get the condition of a need given its current value.
    /// </summary>
    /// <param name="NeedName">The need to get the condition of</param>
    /// <param name="value">The value of the need</param>
    /// <returns></returns>
    public NeedCondition GetNeedCondition(NeedName NeedName, float value)
    {
        foreach(SpeciesNeed need in needs)
        {
            if (need.Name == NeedName)
            {
                return need.GetCondition(value);
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }

    /// <summary>
    /// Get the severity of a given need.
    /// </summary>
    /// <param name="NeedName">The need to get the severity of</param>
    /// <returns></returns>
    public float GetNeedSeverity(NeedName NeedName)
    {
        foreach (SpeciesNeed need in needs)
        {
            if (need.Name == NeedName)
            {
                return need.Severity;
            }
        }
        throw new System.ArgumentException("needName not found in needs list");
    }

    public void OnValidate()
    {
        HashSet <NeedType> seenTypes = new HashSet<NeedType>();
        int numBehaviorSets = 0;
        // Update seenTypes and numBehaviorSets whenever a new need type is found
        foreach (SpeciesNeed need in this.Needs)
        {
            if (!seenTypes.Contains(need.NType))
            {  
                seenTypes.Add(need.NType);
                numBehaviorSets++;
            }
        }
        // Ensure BehaviorSets.Count is correct
        while (this.BehaviorSets.Count < numBehaviorSets)
        {
            this.BehaviorSets.Add(new Behaviors());
        }
        while (this.BehaviorSets.Count > numBehaviorSets)
        {
            this.BehaviorSets.RemoveAt(this.BehaviorSets.Count - 1);
        }
        int i = 0;
        // Give each behavior set the unique need type name. Prevents changing names
        foreach (NeedType needType in seenTypes)
        {
            this.BehaviorSets[i].BehaviorSet = needType;
            i++;
        }
    }
}
