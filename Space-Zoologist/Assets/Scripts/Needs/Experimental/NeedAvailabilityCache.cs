using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the availability of all needs for all populations and food sources
/// </summary>
public class NeedAvailabilityCache
{
    #region Private Fields
    private Dictionary<Population, NeedAvailability> populationNeedAvailabilities;
    private Dictionary<FoodSource, NeedAvailability> foodSourceNeedAvailabilities = new Dictionary<FoodSource, NeedAvailability>();
    #endregion

    #region Constructors
    public NeedAvailabilityCache()
    {
        RebuildCache();
    }
    #endregion

    #region Public Methods
    public void RebuildCache()
    {
        // Build population distribution
        populationNeedAvailabilities = NeedAvailabilityFactory.BuildDistribution();

        // Get a list of all food sources
        foodSourceNeedAvailabilities.Clear();
        List<FoodSource> foodSources = GameManager.Instance.m_foodSourceManager.FoodSources;

        // Create the need availability for each food
        foreach (FoodSource food in foodSources)
        {
            NeedAvailability availability = NeedAvailabilityFactory.Build(food);
            foodSourceNeedAvailabilities[food] = availability;
        }
    }
    public NeedAvailability GetAvailability(Population population)
    {
        if (populationNeedAvailabilities.ContainsKey(population))
        {
            return populationNeedAvailabilities[population];
        }
        else throw new System.ArgumentException(
            $"No need availability exists for population '{population}'");
    }
    public NeedAvailability GetAvailability(FoodSource food)
    {
        if (foodSourceNeedAvailabilities.ContainsKey(food))
        {
            return foodSourceNeedAvailabilities[food];
        }
        else throw new System.ArgumentException(
            $"No need availability exists for food source '{food}'");

    }
    #endregion
}
