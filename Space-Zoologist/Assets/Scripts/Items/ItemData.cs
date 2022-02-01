using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    #region Public Properties
    public ItemName Name => name;
    public Sprite Icon => icon;
    public Item ShopItem => shopItem;
    public ScriptableObject Species => species;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Name used to identify the item")]
    [EditArrayWrapperOnEnum("names", typeof(ItemName.Type))]
    private ItemName name;
    [SerializeField]
    [Tooltip("Icon for this item")]
    private Sprite icon;
    [SerializeField]
    [Tooltip("Reference to the scriptable object used to buy this item in the shop")]
    private Item shopItem;
    [SerializeField]
    [Tooltip("Reference to the scriptable object that holds species information for this item")]
    private ScriptableObject species;
    #endregion

    #region Private Fields
    // Used internally by the item registry to hide
    // the species object for items that do not have a species object
    [SerializeField]
    [HideInInspector]
    private bool hasSpecies = true;
    #endregion
}
