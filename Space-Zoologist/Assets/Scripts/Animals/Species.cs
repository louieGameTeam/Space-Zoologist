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
    [Header("Behavior displayed when need isn't being met")]
    [SerializeField] private List<Behavior> NeedBehaviorSet = default;
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
        // Calculate number of behaviors based off of unique need types
        HashSet <NeedType> uniqueTypes = new HashSet<NeedType>();
        int numBehaviors = 0;
        foreach (SpeciesNeed need in this.Needs)
        {
            if (!uniqueTypes.Contains(need.NType))
            {  
                uniqueTypes.Add(need.NType);
                numBehaviors++;
            }
        }

        // Ensure there is a behavior for each needType
        while (this.NeedBehaviorSet.Count < numBehaviors)
        {
            this.NeedBehaviorSet.Add(new Behavior());
        }
        while (this.NeedBehaviorSet.Count > numBehaviors)
        {
            this.NeedBehaviorSet.RemoveAt(this.NeedBehaviorSet.Count - 1);
        }
        int i = 0;
        // Give each behavior set the unique need type name. Prevents changing names
        foreach (NeedType needType in uniqueTypes)
        {
            this.NeedBehaviorSet[i].NeedType = needType;
            i++;
        }
        
        // Ensure there are no duplicate behaviors when behaviors are being chosen
        HashSet<string> usedBehaviors = new HashSet<string>(); 
        foreach (Behavior behavior in this.NeedBehaviorSet)
        {
            if (!usedBehaviors.Contains(behavior.behavior.ToString()))
            {
                usedBehaviors.Add(behavior.behavior.ToString());
            }
            else
            {
                behavior.behavior = AnimalBehavior.None;
            }
        }
    }
}
