using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the availability of all needs for all populations and food sources
/// </summary>
public class NeedAvailabilityCache
{
    #region Public Properties
    public IReadOnlyDictionary<Population, NeedAvailability> PopulationNeedAvailabilities => populationNeedAvailabilities;
    public IReadOnlyDictionary<FoodSource, NeedAvailability> FoodSourceNeedAvailabilities => foodSourceNeedAvailabilities;
    #endregion

    #region Private Fields
    private Dictionary<Population, NeedAvailability> populationNeedAvailabilities;
    private Dictionary<FoodSource, NeedAvailability> foodSourceNeedAvailabilities = new Dictionary<FoodSource, NeedAvailability>();
    #endregion

    #region Public Methods
    /// <summary>
    /// Rebuild the need availabilities for all populations
    /// </summary>
    /// <remarks>
    /// Since populations compete for food and terrain, all of their 
    /// availabilities must be rebuilt in one operation. The codebase 
    /// does not currently support insertions or updates of existing availability
    /// </remarks>
    public void RebuildAllAvailabilities(ReservePartitionManager rpm)
    {
        populationNeedAvailabilities = NeedAvailabilityBuilder.BuildDistribution(rpm);
    }

    /// <summary>
    /// Rebuild the need availabilities for all food sources
    /// </summary>
    public void RebuildAllAvailabilities(List<FoodSource> foodSources)
    {
        // Get a list of all food sources
        foodSourceNeedAvailabilities.Clear();

        // Create the need availability for each food
        foreach (FoodSource food in foodSources)
        {
            RebuildAvailability(food);
        }
    }

    /// <summary>
    /// Rebuild the availability associated with this food source
    /// </summary>
    /// <remarks>
    /// Since food sources do not compete for terrain or water,
    /// they can be added to the cache individually without clearing 
    /// and rebuilding the whole food source cache
    /// </remarks>
    /// <param name="food"></param>
    public void RebuildAvailability(FoodSource food)
    {
        foodSourceNeedAvailabilities[food] = NeedAvailabilityBuilder.Build(food);
    }

    /// <summary>
    /// Remove a food source from the cache
    /// </summary>
    /// <remarks>
    /// Since food sources do not compete for terrain or water,
    /// they can be removed from the cache individually without clearing 
    /// and rebuilding the whole food source cache
    /// </remarks>
    /// <param name="food"></param>
    /// <returns>True if the food was removed and false if it was not</returns>
    public bool RemoveAvailability(FoodSource food)
    {
        if (HasAvailability(food))
        {
            foodSourceNeedAvailabilities.Remove(food);
            return true;
        }
        else return false;
    }
    
    public bool HasAvailability(Population population)
    {
        return populationNeedAvailabilities.ContainsKey(population);
    }
    public bool HasAvailability(FoodSource food)
    {
        return foodSourceNeedAvailabilities.ContainsKey(food);
    }

    public NeedAvailability GetAvailability(Population population)
    {
        if (HasAvailability(population))
        {
            return populationNeedAvailabilities[population];
        }
        else throw new KeyNotFoundException(
            $"No need availability exists for population '{population}'");
    }
    public NeedAvailability GetAvailability(FoodSource food)
    {
        if (HasAvailability(food))
        {
            return foodSourceNeedAvailabilities[food];
        }
        else throw new KeyNotFoundException(
            $"No need availability exists for food source '{food}'");

    }
    #endregion
}
