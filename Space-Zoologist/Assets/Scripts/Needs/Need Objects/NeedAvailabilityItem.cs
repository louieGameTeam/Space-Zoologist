using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An available item that a species needs
/// </summary>
public class NeedAvailabilityItem
{
    #region Public Properties
    public ItemID ID => id;
    public int AmountAvailable => amountAvailable;
    public object MetaData => metaData;
    public bool IsDrinkingWater => id.IsWater &&
        metaData != null &&
        metaData is float[];
    public float[] WaterComposition
    {
        get
        {
            if (IsDrinkingWater) return metaData as float[];
            else throw new System.InvalidOperationException(
                "Cannot obtain the water composition for this item " +
                "because it is not a drinking water need");
        }
    }
    #endregion

    #region Private Fields
    private ItemID id;
    private int amountAvailable;
    private object metaData;
    #endregion

    #region Constructors
    public NeedAvailabilityItem(ItemID id, int amountAvailable)
        : this(id, amountAvailable, null) { }
    public NeedAvailabilityItem(ItemID id, int amountAvailable, object metaData)
    {
        this.id = id;
        this.amountAvailable = amountAvailable;
        this.metaData = metaData;
    }
    #endregion
}
