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
    public bool hasAccessibilityChanged;
    #endregion

    #region Public Constructors
    public SerializablePopulation(Population population)
    {
        count = population.Count;
        prePopulationCount = population.PrePopulationCount;
        foodDominance = population.FoodDominance;
        hasAccessibilityChanged = population.HasAccessibilityChanged;
    }
    #endregion
}
