using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A runtime instance of a species.
/// </summary>
public class Population : MonoBehaviour
{
    [SerializeField] private AnimalSpecies species = default;
    public AnimalSpecies Species { get => species; }
    private Dictionary<string, float> needsValues = new Dictionary<string, float>();
    public Dictionary<string, float> NeedsValues => needsValues;
    public int Count { get; private set; }
    private Vector2Int origin = Vector2Int.zero;

    private void Awake()
    {
        this.Initialize(species, Vector2Int.RoundToInt((Vector2) transform.position));
    }

    /// <summary>
    /// Initialize the population as the given species at the given origin.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    public void Initialize(AnimalSpecies species, Vector2Int origin)
    {
        this.species = species;
        this.origin = origin;

        this.transform.position = GridUtils.Vector2IntToVector3Int(origin);

        foreach(Need need in species.Needs)
        {
            needsValues.Add(need.NeedName, 0);
        }
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(string need, float value)
    {
        Debug.Assert(needsValues.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        needsValues[need] = value;
        // Debug.Log($"The { species.SpeciesName } population { need } need has new value: {NeedsValues[need]}");
    }

    /// <summary>
    /// Get the value of the given need.
    /// </summary>
    /// <param name="need">The need to get the value of</param>
    /// <returns></returns>
    public float GetNeedValue(string need)
    {
        Debug.Assert(needsValues.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        return needsValues[need];
    }

    // TODO: Implement
    /// <summary>
    /// Gets need conditions for each need based on the current values and sends them along with the need's severity to the growth formula system.
    /// </summary>
    public void UpdateGrowthConditions()
    {
        throw new System.NotImplementedException();
    }
}

