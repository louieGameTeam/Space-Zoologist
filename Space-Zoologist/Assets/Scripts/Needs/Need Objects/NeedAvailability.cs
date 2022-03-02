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

    #region Public Methods
    public NeedAvailabilityItem FindItem(ItemID id)
    {
        int index = Array.FindIndex(items, needItem => needItem.ID == id);
        if (index >= 0) return items[index];
        else return null;
    }
    #endregion
}
