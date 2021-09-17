using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemID
{
    #region Public Properties
    public ItemRegistry.Category Category => category;
    public int Index => index;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Category to select from in the item registry")]
    private ItemRegistry.Category category;
    [SerializeField]
    [Tooltip("Index of the item in the selected category")]
    private int index;
    #endregion
}
