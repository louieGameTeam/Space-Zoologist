using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A runtime instance of a species.
/// </summary>
public class Population : MonoBehaviour
{
    public Dictionary<string, float> NeedsValues => needsValues;
    public AnimalSpecies Species { get => species; }
    public int Count => count;
    public float Dominance => count * species.Dominance;

    [SerializeField] private AnimalSpecies species = default;
    [SerializeField] private int count = 0;
    private Dictionary<string, float> needsValues = new Dictionary<string, float>();
    private Vector2 origin = Vector2.zero;

    private void Awake()
    {
        if (this.species)
        {
            this.Initialize(species, transform.position, count);
        }
    }

    /// <summary>
    /// Initialize the population as the given species at the given origin.
    /// </summary>
    /// <param name="species">The species of the population</param>
    /// <param name="origin">The origin of the population</param>
    /// <param name="needSystemManager"></param>
    public void Initialize(AnimalSpecies species, Vector2 origin, int count)
    {
        this.species = species;
        this.origin = origin;
        this.count = count;

        this.transform.position = origin;

        foreach (Need need in species.Needs.Values)
        {
            needsValues.Add(need.NeedName, 0);
        }
    }

    /// <summary>
    /// Increases the number of animals in the population by the given count.
    /// </summary>
    /// <param name="count">The number of animals to add to the population</param>
    public void AddAnimals(int count)
    {
        this.count += count;
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

    /// <summary>
    /// Gets need conditions for each need based on the current values and sends them along with the need's severity to the growth formula system.
    /// </summary>
    public void UpdateGrowthConditions()
    {
        throw new System.NotImplementedException();
    }
}

