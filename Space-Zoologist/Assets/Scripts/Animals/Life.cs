using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The abstract class both Population and FoodSource will inherit from
/// </summary>
abstract public class Life: MonoBehaviour
{
    public string SpeciesName { get; set; }
    public Dictionary<string, float> NeedsValues => needsValues;

    protected Dictionary<string, float> needsValues = new Dictionary<string, float>();

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    abstract public void UpdateNeed(string need, float value);
}
