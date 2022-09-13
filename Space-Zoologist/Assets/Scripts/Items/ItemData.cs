using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    #region Public Properties
    public ItemName Name => name;
    public TileType Tile => tile;
    public Item ShopItem => shopItem;
    public ScriptableObject Species => species;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Name used to identify the item")]
    [EditArrayWrapperOnEnum("names", typeof(ItemName.Type))]
    private ItemName name = null;
    [SerializeField]
    [Tooltip("Type of tile that this item represents on the tile data controller")]
    private TileType tile = TileType.TypesOfTiles;
    [SerializeField]
    [Tooltip("Reference to the scriptable object used to buy this item in the shop")]
    private Item shopItem = null;
    [SerializeField]
    [Tooltip("Reference to the scriptable object that holds species information for this item")]
    private ScriptableObject species = null;
    #endregion

    #region Private Fields
    // Used internally by the item registry to hide
    // some fields in the item data
    [SerializeField]
    [HideInInspector]
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", 
        Justification = "Cannot be readonly since it is a serialized field")]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", 
        Justification = "This attribute is being used in custom editor code")]
    private ItemRegistry.Category categoryFilter;
    #endregion
}
