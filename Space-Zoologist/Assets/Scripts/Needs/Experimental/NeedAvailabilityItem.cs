using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An available item that a species needs
/// </summary>
[System.Serializable]
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
    [SerializeField]
    [Tooltip("ID of the item that is available")]
    private ItemID id;
    [SerializeField]
    [Tooltip("Amount of the item that is available")]
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

            // If both are drinking water 
            if (IsDrinkingWater && item.IsDrinkingWater)
            {
                return id == item.id &&
                    amountAvailable == item.AmountAvailable &&
                    WaterComposition.SequenceEqual(item.WaterComposition);
            }
            else if (!IsDrinkingWater && !item.IsDrinkingWater)
            {
                return id == item.ID && amountAvailable == item.AmountAvailable;
            }
            else return false;
        }
    }
    public override int GetHashCode()
    {
        if (IsDrinkingWater)
        {
            return id.GetHashCode() + amountAvailable.GetHashCode() + WaterComposition.GetHashCode();
        }
        else return id.GetHashCode() + amountAvailable.GetHashCode();
    }
    #endregion
}
