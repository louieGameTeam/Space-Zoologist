using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player's request for additional resources
/// they can use to sustain the enclosure
/// </summary>
[System.Serializable]
public class ResourceRequest
{
    #region Public Properties
    public ItemID ItemAddressed
    {
        get => itemAddressed;
        set => itemAddressed = value;
    }
    public NeedType NeedAddressed
    {
        get
        {
            // If the item requested is food, the need type addressed is food
            if (ItemRequested.Category == ItemRegistry.Category.Food) return NeedType.FoodSource;
            // If it's not food we assume it is a tile. Check if it is a water tile
            else if (ItemRequested.Data.Name.Get(ItemName.Type.English).Contains("Water")) return NeedType.Liquid;
            // If it is not water it must be a tile to address terrain needs
            else return NeedType.Terrain;
        }
    }
    public int QuantityRequested
    {
        get => quantityRequested;
        set => quantityRequested = value;
    }
    public ItemID ItemRequested
    {
        get => itemRequested;
        set => itemRequested = value;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Item that the player is trying to address by making the resource request")]
    private ItemID itemAddressed;
    [SerializeField]
    [Tooltip("Quantity of the resource requested")]
    private int quantityRequested;
    [SerializeField]
    [Tooltip("The item that is requested")]
    private ItemID itemRequested;
    #endregion
}
