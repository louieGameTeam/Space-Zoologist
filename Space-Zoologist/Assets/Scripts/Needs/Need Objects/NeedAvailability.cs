using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registers the availability 
/// of different needed items
/// </summary>
public class NeedAvailability
{
    #region Public Properties
    public NeedAvailabilityItem[] Items => items;
    #endregion

    #region Private Fields
    private NeedAvailabilityItem[] items;
    #endregion

    #region Constructors
    public NeedAvailability(params NeedAvailabilityItem[] items)
    {
        this.items = new NeedAvailabilityItem[items.Length];
        items.CopyTo(this.items, 0);
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
