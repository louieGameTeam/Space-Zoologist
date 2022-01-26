using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializablePopulation
{
    #region Public Fields
    public int count;
    public int prePopulationCount;
    public float foodDominance;
    public Vector3 origin;
    public List<SerializableNeed> needs;
    public SerializableGrowthCalculator growthCalculator;
    public bool isPaused;
    public bool hasAccessibilityChanged;
    #endregion

    #region Public Constructors
    public SerializablePopulation(Population population)
    {
        // Set the first few fields
        count = population.Count;
        prePopulationCount = population.PrePopulationCount;
        foodDominance = population.FoodDominance;
        origin = population.Origin;

        // Create a list with all the entries in the needs dictionary
        IEnumerable<SerializableNeed> entries = population.Needs.Values
            .Select(need => new SerializableNeed(need));
        needs = new List<SerializableNeed>(entries);

        // Set the remaining fields
        growthCalculator = new SerializableGrowthCalculator(population.GrowthCalculator);
        isPaused = population.IsPaused;
        hasAccessibilityChanged = population.HasAccessibilityChanged;
    }
    #endregion
}
