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
    public bool Valid => ItemRegistry.ValidID(this);
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
    #endregion
}
