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

    #region Constructors
    public NeedRatingCache() : this(null) { }
    public NeedRatingCache(NeedAvailabilityCache cache)
    {
        RebuildCache(cache);
    }
    #endregion

    #region Public Methods
    public void RebuildCache(NeedAvailabilityCache cache = null)
    {
        // If no cache is given then build a new one
        if (cache == null)
        {
            cache = new NeedAvailabilityCache();
        }

        // Get a list of all populations
        List<Population> populations = GameManager.Instance.m_populationManager.Populations;
        populationRatings.Clear();
        
        // Add the rating for every population
        foreach (Population population in populations)
        {
            NeedAvailability availability = cache.GetAvailability(population);
            NeedRating rating = NeedRatingBuilder.Build(population, availability);
            populationRatings.Add(population, rating);
        }

        // Get a list of all food sources
        List<FoodSource> foodSources = GameManager.Instance.m_foodSourceManager.FoodSources;
        foodSourceRatings.Clear();

        // Add the rating for every food source
        foreach (FoodSource food in foodSources)
        {
            NeedAvailability availability = cache.GetAvailability(food);
            NeedRating rating = NeedRatingBuilder.Build(food, availability);
            foodSourceRatings.Add(food, rating);
        }
    }
    public NeedRating GetRating(Population population)
    {
        if (populationRatings.ContainsKey(population))
        {
            return populationRatings[population];
        }
        else throw new System.ArgumentException(
            $"No need rating associated with population '{population}'");
    }
    public NeedRating GetRating(FoodSource food)
    {
        if (foodSourceRatings.ContainsKey(food))
        {
            return foodSourceRatings[food];
        }
        else throw new System.ArgumentException(
            $"No need rating associated with food source '{food}'");
    }
    #endregion
}
