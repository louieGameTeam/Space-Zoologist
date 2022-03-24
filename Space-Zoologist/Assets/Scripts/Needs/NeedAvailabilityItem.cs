using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An available item that a species needs
/// </summary>
[Serializable]
public class NeedAvailabilityItem
{
    #region Public Properties
    public ItemID ID => id;
    public int ItemCount => itemCount;
    public float AmountAvailable => amountAvailable;
    public object MetaData => metaData;
    public bool IsDrinkingWater => id.IsWater &&
        metaData != null &&
        metaData is LiquidBodyContent;
    public LiquidBodyContent WaterContent
    {
        get
        {
            if (IsDrinkingWater) return metaData as LiquidBodyContent;
            else throw new InvalidOperationException(
                "Cannot obtain the water composition for this item " +
                "because it is not a drinking water need");
        }
    }
    #endregion

    #region Private Fields
    [SerializeField]
    [Tooltip("ID of the item that is available")]
    private ItemID id;
    [SerializeField]
    [Tooltip("Count the number of this item that is available. " +
        "This is distinct from the amount available in the case of food, " +
        "where the count is the number of food sources whereas " +
        "the amount available is the total fruit output")]
    private int itemCount;
    [SerializeField]
    [Tooltip("Amount of the item that is available")]
    private float amountAvailable;
    private object metaData;
    #endregion

    #region Constructors
    public NeedAvailabilityItem(ItemID id, int itemCount, float amountAvailable)
        : this(id, itemCount, amountAvailable, null) { }
    public NeedAvailabilityItem(ItemID id, int itemCount, float amountAvailable, object metaData)
    {
        this.id = id;
        this.itemCount = itemCount;
        this.amountAvailable = amountAvailable;
        this.metaData = metaData;
    }
    #endregion

    #region Operator Overloads
    public static bool operator ==(NeedAvailabilityItem a, NeedAvailabilityItem b)
    {
        // If a is not null then use the equals override
        if (!ReferenceEquals(a, null)) return a.Equals(b);
        // If a is null and b is null then they are equal
        else if (ReferenceEquals(b, null)) return true;
        // If a is null and b is not null then they are unequal
        else return false;
    }
    public static bool operator !=(NeedAvailabilityItem a, NeedAvailabilityItem b)
    {
        return !(a == b);
    }
    #endregion

    #region Object Overrides
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        else if (obj.GetType() != GetType()) return false;
        else
        {
            NeedAvailabilityItem item = obj as NeedAvailabilityItem;
            return id == item.id && amountAvailable == item.amountAvailable && Equals(metaData, item.metaData);
        }
    }
    public override int GetHashCode()
    {
        int hash = id.GetHashCode() ^ amountAvailable.GetHashCode();

        // If there is metadata then add its hash code
        if (metaData != null)
        {
            return hash ^ metaData.GetHashCode();
        }
        // If there is no metadata do not add its hash
        else return hash;
    }
    #endregion
}
