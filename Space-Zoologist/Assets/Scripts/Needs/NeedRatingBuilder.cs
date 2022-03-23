using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to build a need rating for a species
/// </summary>
public static class NeedRatingBuilder
{
    #region Public Constants
    public const float MaxFreshWater = 0.98f;
    #endregion

    #region Public Methods
    /// <summary>
    /// Build the need rating for a single population
    /// </summary>
    /// <param name="population">Population to rate</param>
    /// <param name="availability">
    /// Needs available to that population.
    /// If 'null' is provided, builds the need availability from scratch
    /// </param>
    /// <returns>The rating for the population's needs</returns>
    public static NeedRating Build(Population population, NeedAvailability availability)
    {
        // Compute each rating
        int predatorCount = CountPredators(
            population.Species.Needs,
            availability);
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
        return new NeedRating(predatorCount, foodRating, terrainRating, waterRating);
    }
    /// <summary>
    /// Build the need rating for a single food source
    /// </summary>
    /// <param name="foodSource"></param>
    /// <param name="availability">
    /// Needs available to that population.
    /// If 'null' is proved, builds the need availability from scratch
    /// </param>
    /// <returns></returns>
    public static NeedRating Build(FoodSource foodSource, NeedAvailability availability)
    {
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
        return new NeedRating(0, -1f, terrainRating, waterRating);
    }
    #endregion

    #region Private Methods
    private static int CountPredators(NeedRegistry needs, NeedAvailability availability)
    {
        NeedData[] predatorNeeds = needs.FindPredatorNeeds();
        float predatorCount = 0;

        foreach (NeedData need in predatorNeeds)
        {
            // Find the availability of predators
            NeedAvailabilityItem predator = availability.FindWithItem(need.ID);

            // If that predator is available then increase the count by the number available
            if (predator != null)
            {
                predatorCount += predator.AmountAvailable;
            }
        }

        return (int)predatorCount;
    }
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
        // Select only water that is drinkable
        IEnumerable<NeedAvailabilityItem> drinkableWater = availability
            .FindAllWater()
            .Where(item => needs.WaterIsDrinkable(item.WaterComposition));

        // Number of water tiles that the species can drink from
        float totalDrinkableWater = drinkableWater.Sum(item => item.AmountAvailable);
        
        // Drinkable water used will not exceed the amount actually needed
        float drinkableWaterUsed = Mathf.Min(totalDrinkableWater, waterTilesNeeded);

        if (drinkableWaterUsed >= waterTilesNeeded)
        {
            // Find the need for fresh water
            NeedData freshWaterNeed = Array.Find(
                needs.FindWaterNeeds(), 
                need => need.ID.WaterIndex == 0);

            // If we have a fresh water need, boost it
            if (freshWaterNeed != null)
            {
                // Average the fresh water from all water sources
                float averageFreshWater = drinkableWater
                    .Sum(item => item.WaterComposition[0] * item.AmountAvailable);
                averageFreshWater /= totalDrinkableWater;

                // Boost the rating by how close it is to the max possible fresh water
                return 1 + (averageFreshWater - freshWaterNeed.Minimum) / (MaxFreshWater - freshWaterNeed.Minimum);
            }
            // If we have no fresh water need,
            // for now just assume a max boost
            else return 2f;
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

        // Go through each need in the needs
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
