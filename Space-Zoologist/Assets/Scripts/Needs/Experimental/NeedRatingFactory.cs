using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to build a need rating for a species
/// </summary>
public static class NeedRatingFactory
{
    #region Public Methods
    public static NeedRating Build(Population population, NeedAvailability availability = null)
    {
        // If no availability is given then build it from scratch
        if (availability == null)
        {
            Dictionary<Population, NeedAvailability> distribution = NeedAvailabilityFactory.BuildDistribution();
            availability = distribution[population];
        }

        // Compute each rating
        float foodRating = FoodRating(
            population.Species.Needs, 
            availability, 
            population.Species.MinFoodRequired * population.Count, 
            population.Species.MaxFoodRequired * population.Count);
        float terrainRating = TerrainRating(
            population.Species.Needs, 
            availability, 
            population.Species.TerrainTilesRequired * population.Count);
        float waterRating = WaterRating(
            population.Species.Needs,
            availability,
            population.Species.WaterTilesRequired * population.Count);

        // Return the new need rating object
        return new NeedRating(foodRating, terrainRating, waterRating);
    }
    public static NeedRating Build(FoodSource foodSource, NeedAvailability availability = null)
    {
        // If no availability is given then build it from scratch
        if (availability == null)
        {
            availability = NeedAvailabilityFactory.Build(foodSource);
        }

        // Compute each rating
        float terrainRating = TerrainRating(
            foodSource.Species.Needs,
            availability,
            foodSource.Species.TerrainTilesNeeded);
        float waterRating = WaterRating(
            foodSource.Species.Needs,
            availability,
            foodSource.Species.WaterTilesRequired);

        // Return the new need rating object
        return new NeedRating(-1f, terrainRating, waterRating);
    }
    #endregion

    #region Private Methods
    private static float FoodRating(NeedRegistry needs, NeedAvailability availability, int minFoodNeeded, int maxFoodConsumed)
    {
        // Get all food needs
        NeedData[] foodNeeds = needs.FindFoodNeeds();

        // Get the amount of foods consumed
        return SimplePreferenceNeedRating(
            foodNeeds,
            availability,
            minFoodNeeded,
            maxFoodConsumed,
            (item, need) => item.ID == need.ID);
    }
    private static float TerrainRating(NeedRegistry needs, NeedAvailability availability, int terrainTilesNeeded)
    {
        // Get all terrain needs
        NeedData[] terrainNeeds = needs.FindTerrainNeeds();

        // Return a simple preference need rating
        return SimplePreferenceNeedRating(
            terrainNeeds,
            availability,
            terrainTilesNeeded,
            terrainTilesNeeded,
            (item, need) => item.ID == need.ID && !item.IsDrinkingWater);
    }
    private static float WaterRating(NeedRegistry needs, NeedAvailability availability, int waterTilesNeeded)
    {
        // Number of water tiles that the species can drink from
        float drinkableWaterUsed = availability
            .FindAllWater()
            .Where(item => needs.WaterIsDrinkable(item.WaterComposition))
            .Sum(item => item.AmountAvailable);
        // Drinkable water used will not exceed the amount actually needed
        drinkableWaterUsed = Mathf.Min(drinkableWaterUsed, waterTilesNeeded);

        if (drinkableWaterUsed >= waterTilesNeeded)
        {
            // This computation will have to be changed later,
            // because I don't understand how to "boost" the rating yet
            return 2f;
        }
        else return (float)drinkableWaterUsed / waterTilesNeeded;
    }
    private static float SimplePreferenceNeedRating(
        NeedData[] needs,
        NeedAvailability availability,
        int minNeeded,
        int maxUsed,
        Func<NeedAvailabilityItem, NeedData, bool> itemMatch)
    {
        // Start at using none
        float preferredUsed = 0;
        float survivableUsed = 0;

        // Go through each need in the terrain needs
        foreach (NeedData need in needs)
        {
            NeedAvailabilityItem availabilityItem = availability
                .Find(item => itemMatch.Invoke(item, need));

            // If that item is available,
            // then increase tiles occupied by the amount available
            if (availabilityItem != null)
            {
                if (need.Preferred)
                {
                    preferredUsed += availabilityItem.AmountAvailable;
                }
                else survivableUsed += availabilityItem.AmountAvailable;
            }
        }

        // Cannot use more than what is needed
        preferredUsed = Mathf.Min(preferredUsed, maxUsed);
        survivableUsed = Mathf.Min(survivableUsed, maxUsed - preferredUsed);
        float totalUsed = preferredUsed + survivableUsed;

        // If we used the amound we needed, then boost the rating
        // by the amount used that is preferred
        if (totalUsed >= minNeeded)
        {
            return 1 + ((float)preferredUsed / totalUsed);
        }
        // If we did not use the amount we needed
        // then the rating is the proportion that we needed
        else return (float)totalUsed / minNeeded;
    }

    #endregion
}
