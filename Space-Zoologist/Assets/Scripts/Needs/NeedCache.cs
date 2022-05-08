using System.Collections;
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
    public bool PopulationCacheDirty { get; private set; } = true;
    public bool FoodCacheDirty { get; private set; } = true;
    #endregion

    #region Public Methods

    public NeedCache()
    {
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountChange, MarkPopulationCacheDirty);
        EventManager.Instance.SubscribeToEvent(EventType.LiquidChange, MarkFoodCacheDirty);
        EventManager.Instance.SubscribeToEvent(EventType.FoodSourceChange, MarkFoodCacheDirty);
        EventManager.Instance.SubscribeToEvent(EventType.TilemapChange, MarkFoodCacheDirty);
        EventManager.Instance.SubscribeToEvent(EventType.InspectorSelectionChanged, RebuildIfDirty);
    }

    ~NeedCache()
    {
        EventManager.Instance.UnsubscribeToEvent(EventType.PopulationCountChange, MarkPopulationCacheDirty);
        EventManager.Instance.UnsubscribeToEvent(EventType.LiquidChange, MarkFoodCacheDirty);
        EventManager.Instance.UnsubscribeToEvent(EventType.FoodSourceChange, MarkFoodCacheDirty);
        EventManager.Instance.UnsubscribeToEvent(EventType.TilemapChange, MarkFoodCacheDirty);
        EventManager.Instance.UnsubscribeToEvent(EventType.InspectorSelectionChanged, RebuildIfDirty);

    }

    private void MarkPopulationCacheDirty()
    {
        PopulationCacheDirty = true;
    }

    private void MarkFoodCacheDirty()
    {
        FoodCacheDirty = true;
    }

    public void RebuildIfDirty()
    {
        // If food cache is dirty then rebuild both food and population cache
        if (FoodCacheDirty)
        {
            Rebuild();
        }
        // If population cache is dirty then rebuild only that
        else if (PopulationCacheDirty)
        {
            RebuildPopulationCache();
        }
    }

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
        FoodCacheDirty = false;
        EventManager.Instance.InvokeEvent(EventType.FoodCacheRebuilt, null);
    }
    public void RebuildPopulationCache()
    {
        Availability.RebuildPopulationAvailabilities();
        Ratings.RebuildPopulationRatings(Availability);
        PopulationCacheDirty = false;
        EventManager.Instance.InvokeEvent(EventType.PopulationCacheRebuilt, null);
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
