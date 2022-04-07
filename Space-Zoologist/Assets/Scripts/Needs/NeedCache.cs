﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the cache for the availability of needs 
/// and ratings of different populations and food source
/// </summary>
public class NeedCache
{
    #region Public Properties
    public NeedAvailabilityCache Availability { get; private set; } = new NeedAvailabilityCache();
    public NeedRatingCache Ratings { get; private set; } = new NeedRatingCache();
    #endregion

    #region Public Methods
    public void Rebuild()
    {
        // Rebuild the food source first, because population depends on their output,
        // which depends on their ratings
        RebuildFoodCache();

        // Rebuild the populations now that foods are taken care of
        RebuildPopulationCache();
    }
    public void RebuildFoodCache()
    {
        Availability.RebuildFoodAvailabilities();
        Ratings.RebuildFoodRatings(Availability);
    }
    public void RebuildPopulationCache()
    {
        Availability.RebuildPopulationAvailabilities();
        Ratings.RebuildPopulationRatings(Availability);
    }
    /// <summary>
    /// Rebuild the cache associated with a single food source
    /// </summary>
    /// <param name="food"></param>
    public void Rebuild(FoodSource food)
    {
        Availability.RebuildAvailability(food);
        Ratings.RebuildRating(food, Availability.GetAvailability(food));
    }
    public bool HasCache(Population population)
    {
        return Availability.HasAvailability(population) && Ratings.HasRating(population);
    }
    public bool HasCache(FoodSource food)
    {
        return Availability.HasAvailability(food) && Ratings.HasRating(food);
    }
    #endregion
}
