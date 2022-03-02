using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemID
{
    #region Public Properties
    public ItemRegistry.Category Category => category;
    public int Index => index;
    public ItemData Data => ItemRegistry.Get(this);
    public bool IsValid => ItemRegistry.ValidID(this);
    public bool IsWater => Data.Name.AnyNameContains("Water");
    // Used as the index into the liquidbody contents
    public int WaterIndex
    {
        get
        {
            if (IsWater)
            {
                ItemID firstWater = ItemRegistry.FindAnyNameContains("Water");
                return index - firstWater.index;
            }
            else throw new System.InvalidOperationException(
                "Cannot get the water index of an item id " +
                $"for which the '{nameof(IsWater)}' property " +
                $"does not return 'true'");
        }
    }
    public static ItemID Invalid => new ItemID(0, -1);
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Category to select from in the item registry")]
    private ItemRegistry.Category category;
    [SerializeField]
    [Tooltip("Index of the item in the selected category")]
    private int index;
    #endregion

    #region Constructors
    public ItemID(ItemRegistry.Category category, int index)
    {
        this.category = category;
        this.index = index;
    }
    #endregion

    #region Factories
    public static ItemID FromWaterIndex(int waterIndex)
    {
        ItemID firstWater = ItemRegistry.FindAnyNameContains("Water");
        ItemID newID = new ItemID(firstWater.Category, firstWater.index + waterIndex);

        if (newID.IsValid) return newID;
        else throw new System.ArgumentException(
            $"Water index '{waterIndex}' resulted in id '{newID}', " +
            $"which is not a valid item id. Please use a different water index " +
            $"or adjust the {nameof(ItemRegistry)}");
    }
    public static ItemID FreshWater() => FromWaterIndex(0);
    public static ItemID SaltWater() => FromWaterIndex(1);
    public static ItemID StagnantWater() => FromWaterIndex(2);
    #endregion

    #region Operators
    public static bool operator ==(ItemID a, ItemID b) => a.category == b.category && a.index == b.index;
    public static bool operator !=(ItemID a, ItemID b) => !(a == b);
    #endregion

    #region Overrides
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        else if (obj.GetType() == GetType()) return this == (ItemID)obj;
        else return false;
    }
    public override int GetHashCode()
    {
        return category.GetHashCode() + index.GetHashCode();
    }
    public override string ToString()
    {
        string str = $"ItemID ({category}, {index})";
        if (IsValid) str += $" ({Data.Name})";
        return str;
    }
    #endregion
}
