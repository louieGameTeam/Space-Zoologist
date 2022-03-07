using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Abstract class that all NeedSystems will inherit from. Every need system will have a list of consumers (Life) 
/// that have the need that the need system is in charge of, and keeps this need up to date for all of its 
/// consumers.
/// </summary>
abstract public class NeedSystem
{
    #region Legacy Code
    public NeedType NeedType { get; private set; }
    public bool IsDirty => this.isDirty;

    // Dirty flag is on to force intial update
    protected bool isDirty = true;
    protected HashSet<Life> Consumers = new HashSet<Life>();

    public NeedSystem(NeedType needType)
    {
        NeedType = needType;
    }

    /// <summary>
    /// Mark this system dirty
    /// </summary>
    /// <remarks>Any one can mark a system dirty, but only the system can unmark itself</remarks>
    virtual public void MarkAsDirty()
    {
        this.isDirty = true;
    }

    virtual public void AddConsumer(Life life)
    {
        this.isDirty = true;
        this.Consumers.Add(life);
    }

    virtual public bool RemoveConsumer(Life life)
    {
        this.isDirty = true;
        return this.Consumers.Remove(life);
    }

    // Check the evnoirmental state of the consumers, currently only terrain
    virtual public bool CheckState()
    {
        foreach (Life consumer in this.Consumers)
        {
            if (consumer.GetAccessibilityStatus())
            {
                return true;
            }
        }

        return false;
    }

    abstract public void UpdateSystem();
    #endregion

    #region Experimental Methods
    public static float WaterRating(NeedRegistry needs, NeedAvailability availability, int waterTilesNeeded)
    {
        // Number of water tiles that the species can drink from
        int drinkableWaterUsed = availability
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
    public static float FoodRating(NeedRegistry needs, NeedAvailability availability, int minFoodNeeded, int maxFoodConsumed)
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
    public static float TerrainRating(NeedRegistry needs, NeedAvailability availability, int terrainTilesNeeded)
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
    private static float SimplePreferenceNeedRating(
        NeedData[] needs, 
        NeedAvailability availability, 
        int minNeeded, 
        int maxUsed, 
        Func<NeedAvailabilityItem, NeedData, bool> itemMatch)
    {
        // Start at using none
        int preferredUsed = 0;
        int survivableUsed = 0;

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
        int totalUsed = preferredUsed + survivableUsed;

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