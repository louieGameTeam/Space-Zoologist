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
    /// <param name="availability">Needs available to that population</param>
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
        float friendRating = FriendRating(
            population.Species.Needs,
            availability,
            population.Species.FriendsRequired.x * population.Count,
            population.Species.FriendsRequired.y * population.Count);
        float treeRating = TreeRating(
            population.Species.Needs,
            availability,
            population.Species.TreesRequired * population.Count);

        // Return the new need rating object
        return new NeedRating(predatorCount, foodRating, terrainRating, waterRating, friendRating, treeRating);
    }
    /// <summary>
    /// Build the need rating for a single food source
    /// </summary>
    /// <param name="foodSource"></param>
    /// <param name="availability"></param>
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
        return new NeedRating(0, float.NaN, terrainRating, waterRating, float.NaN, float.NaN);
    }
    #endregion

    #region Private Methods
    private static int CountPredators(NeedRegistry needs, NeedAvailability availability)
    {
        NeedData[] predatorNeeds = needs.FindPredatorNeeds();
        float predatorCount = 0;

        foreach (NeedData need in predatorNeeds)
        {
            // Find a predator with the same id as the need
            NeedAvailabilityItem predator = availability.FindWithItem(need.ID);

            // If we found an available predator then 
            // add the number available to the local variable
            if (predator != null)
            {
                predatorCount += predator.AmountAvailable;
            }
        }

        return (int)predatorCount;
    }
    private static float FoodRating(NeedRegistry needs, NeedAvailability availability, int minFoodNeeded, int maxFoodConsumed)
    {
        return SimplePreferenceNeedRating(
            needs.FindFoodNeeds(),
            availability.FindFoodItems(),
            minFoodNeeded,
            maxFoodConsumed,
            true);
    }
    private static float TerrainRating(NeedRegistry needs, NeedAvailability availability, int terrainTilesNeeded)
    {
        return SimplePreferenceNeedRating(
            needs.FindTerrainNeeds(),
            availability.FindTerrainItems(),
            terrainTilesNeeded,
            terrainTilesNeeded,
            true);
    }
    private static float TreeRating(NeedRegistry needs, NeedAvailability availability, int treesNeeded)
    {
        return SimplePreferenceNeedRating(
            needs.FindTreeNeeds(),
            availability.FindTreeItems(),
            treesNeeded,
            treesNeeded,
            false);
    }
    private static float FriendRating(NeedRegistry needs, NeedAvailability availability, int minFriendsNeeded, int maxFriendsNeeded)
    {
        return SimplePreferenceNeedRating(
            needs.FindFriendNeeds(),
            availability.FindFriendItems(),
            minFriendsNeeded,
            maxFriendsNeeded,
            false);
    }
    private static float WaterRating(NeedRegistry needs, NeedAvailability availability, int waterTilesNeeded)
    {
        // Select only water that is drinkable
        IEnumerable<NeedAvailabilityItem> drinkableWater = availability
            .FindWaterItems()
            .Where(item => needs.WaterIsDrinkable(item.WaterContent.Contents));

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
                    .Sum(item => item.WaterContent.Contents[0] * item.AmountAvailable);
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
        NeedAvailabilityItem[] items,
        int minNeeded,
        int maxUsed,
        bool useAmount)
    {
        // Check if some needs were passed in
        if (needs.Length > 0)
        {
            // Start at using none
            float preferredUsed = 0;
            float survivableUsed = 0;

            // Go through each need in the needs
            foreach (NeedData need in needs)
            {
                // Find an item with the same id as the need
                NeedAvailabilityItem applicableItem = Array
                    .Find(items, item => item.ID == need.ID);

                if (applicableItem != null)
                {
                    // Get total available based on whether to use the item count or amount available
                    float totalAvailable = useAmount ? applicableItem.AmountAvailable : applicableItem.ItemCount;

                    // If the need is preferred then add to preferred used
                    if (need.Preferred)
                    {
                        preferredUsed += totalAvailable;
                    }
                    // If the need is not preferred the add to survivable used
                    else survivableUsed += totalAvailable;
                }

            }

            // Cannot use more than what is needed
            preferredUsed = Mathf.Min(preferredUsed, maxUsed);
            survivableUsed = Mathf.Min(survivableUsed, maxUsed - preferredUsed);
            float totalUsed = preferredUsed + survivableUsed;

            // If we used the amound we needed, then boost the rating
            if (totalUsed >= minNeeded)
            {
                int magnitude = maxUsed - minNeeded;

                // Boost amounts used to boost the rating
                float towardsMaxBoost = 0f;
                float preferenceBoost = preferredUsed / totalUsed;

                // If there is a difference between min-max used,
                // then half the boost comes from how close to the max is used
                // and the other half of the boost comes from how much of 
                // the preferred item is used
                if (magnitude > 0)
                {
                    towardsMaxBoost = 0.5f * (totalUsed / maxUsed);
                    preferenceBoost *= 0.5f;
                }

                return 1 + towardsMaxBoost + preferenceBoost;
            }
            // If we did not use the amount we needed
            // then the rating is the proportion that we needed
            else return (float)totalUsed / minNeeded;
        }
        // If there were no needs then return the number for no need rating "NaN"
        else return float.NaN;
    }

    #endregion
}
