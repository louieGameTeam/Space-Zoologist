using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Caches the need ratings of all populations and food sources
/// </summary>
public class NeedRatingCache
{
    #region Public Properties
    public IReadOnlyDictionary<Population, NeedRating> PopulationRatings => populationRatings;
    public IReadOnlyDictionary<FoodSource, NeedRating> FoodSourceRatings => foodSourceRatings;
    #endregion

    #region Private Fields
    private Dictionary<Population, NeedRating> populationRatings = new Dictionary<Population, NeedRating>();
    private Dictionary<FoodSource, NeedRating> foodSourceRatings = new Dictionary<FoodSource, NeedRating>();
    #endregion

    #region Public Methods
    /// <summary>
    /// Rebuild the ratings for each population
    /// </summary>
    /// <param name="populations"></param>
    /// <param name="cache">
    /// Cache used to hold the need availabilities 
    /// of all populations
    /// </param>
    public void RebuildPopulationRatings(NeedAvailabilityCache cache)
    {
        // Get a list of all populations
        populationRatings.Clear();

        // Add the rating for every population
        foreach (KeyValuePair<Population, NeedAvailability> kvp in cache.PopulationNeedAvailabilities)
        {
            RebuildRating(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Rebuild the ratings for each food source
    /// </summary>
    /// <param name="foods"></param>
    /// <param name="cache"></param>
    public void RebuildFoodSourceRatings(NeedAvailabilityCache cache)
    {
        // Clear existing ratings
        foodSourceRatings.Clear();

        // Add the rating for every population
        foreach (KeyValuePair<FoodSource, NeedAvailability> kvp in cache.FoodSourceNeedAvailabilities)
        {
            RebuildRating(kvp.Key, kvp.Value);
        }
    }

    /// <summary>
    /// Rebuild the rating for a single population
    /// </summary>
    /// <param name="population"></param>
    /// <param name="availability"></param>
    public void RebuildRating(Population population, NeedAvailability availability)
    {
        populationRatings[population] = NeedRatingBuilder.Build(population, availability);
    }

    /// <summary>
    /// Rebuild the rating for a single food source
    /// </summary>
    /// <param name="food"></param>
    /// <param name="availability"></param>
    public void RebuildRating(FoodSource food, NeedAvailability availability)
    {
        foodSourceRatings[food] = NeedRatingBuilder.Build(food, availability);
    }

    public bool HasRating(Population population)
    {
        return populationRatings.ContainsKey(population);
    }
    public bool HasRating(FoodSource food)
    {
        return foodSourceRatings.ContainsKey(food);
    }
    
    public NeedRating GetRating(Population population)
    {
        if (HasRating(population))
        {
            return populationRatings[population];
        }
        else throw new System.ArgumentException(
            $"No need rating associated with population '{population}'");
    }
    public NeedRating GetRating(FoodSource food)
    {
        if (HasRating(food))
        {
            return foodSourceRatings[food];
        }
        else throw new System.ArgumentException(
            $"No need rating associated with food source '{food}'");
    }
    #endregion
}
