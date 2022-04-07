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
    public bool isPaused;
    public bool hasAccessibilityChanged;
    #endregion

    #region Public Constructors
    public SerializablePopulation(Population population)
    {
        count = population.Count;
        prePopulationCount = population.PrePopulationCount;
        foodDominance = population.FoodDominance;
        origin = population.Origin;
        isPaused = population.IsPaused;
        hasAccessibilityChanged = population.HasAccessibilityChanged;
    }
    #endregion
}
