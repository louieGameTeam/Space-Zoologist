using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registers the availability 
/// of different needed items
/// </summary>
/// <remarks>
/// The need availability will contain needs for a species
/// that it may not actually use, such as inedible food or
/// undrinkable water. The NeedRatingFactory needs to check 
/// these conditions when rating a need based on the availability
/// of other items.
/// 
/// The need availability may have items with duplicate ItemID's
/// due to the way that drinking water is handled. Each drinking
/// water item has the ID of the first water in the ItemRegistry.
/// But it may also have a non drinking water need with the 
/// ID of the first water in the ItemRegistry if a species can
/// traverse water as a terrain need.
/// </remarks>
[Serializable]
public class NeedAvailability
{
    #region Public Properties
    public NeedAvailabilityItem[] Items => items;
    #endregion

    #region Private Fields
    [SerializeField]
    [Tooltip("List of the items that are available")]
    private NeedAvailabilityItem[] items;
    #endregion

    #region Constructors
    public NeedAvailability(params NeedAvailabilityItem[] items)
    {
        List<NeedAvailabilityItem> newItems = new List<NeedAvailabilityItem>();
        Dictionary<ItemID, NeedAvailabilityItem> needMap = new Dictionary<ItemID, NeedAvailabilityItem>();
        IEnumerable<NeedAvailabilityItem> nonDrinkingWater = items.Where(item => !item.IsDrinkingWater);

        // Go through the list of all non drinking water and add them to the dictionary
        foreach (NeedAvailabilityItem item in nonDrinkingWater)
        {
            // If the map doesn't have the key then add it
            if (!needMap.ContainsKey(item.ID))
            {
                needMap[item.ID] = item;
            }
            // If the map does have the key then change the value
            // to a new availability item with the amounts available added together
            else
            {
                NeedAvailabilityItem previous = needMap[item.ID];
                needMap[item.ID] = new NeedAvailabilityItem(item.ID, item.AmountAvailable + previous.AmountAvailable);
            }
        }

        // Add all the values from the dictionary to the list
        newItems.AddRange(needMap.Select(kvp => kvp.Value));

        // Next we'll go through the drinking water and collapse needs with the same liquid composition
        IEnumerable<NeedAvailabilityItem> drinkingWater = items.Where(item => item.IsDrinkingWater);
        Dictionary<float[], NeedAvailabilityItem> liquidMap = new Dictionary<float[], NeedAvailabilityItem>(new LiquidCompositionComparer());

        foreach (NeedAvailabilityItem item in drinkingWater)
        {
            // If the map does not have the key then set its value
            if (!liquidMap.ContainsKey(item.WaterComposition))
            {
                liquidMap[item.WaterComposition] = item;
            }
            // If the map already has the key then collapse this item
            // with the one that is already in the dictionary
            else
            {
                NeedAvailabilityItem previous = liquidMap[item.WaterComposition];
                liquidMap[item.WaterComposition] = new NeedAvailabilityItem(item.ID, 
                    item.AmountAvailable + previous.AmountAvailable,
                    item.WaterComposition);
            }
        }

        // Add the items to the list
        newItems.AddRange(liquidMap.Select(kvp => kvp.Value));

        // Finally, set the local array to the list
        this.items = newItems.ToArray();
    }
    #endregion

    #region Find Methods
    public NeedAvailabilityItem FindWithItem(ItemID id)
    {
        return Find(item => item.ID == id);
    }
    public NeedAvailabilityItem Find(Predicate<NeedAvailabilityItem> predicate)
    {
        int index = Array.FindIndex(items, predicate);
        if (index >= 0) return items[index];
        else return null;
    }
    #endregion

    #region Find All Methods
    public NeedAvailabilityItem[] FindAllWater()
    {
        return FindAll(item => item.IsDrinkingWater);
    }
    public NeedAvailabilityItem[] FindAll(Predicate<NeedAvailabilityItem> predicate)
    {
        return Array.FindAll(items, predicate);
    }
    #endregion
}
