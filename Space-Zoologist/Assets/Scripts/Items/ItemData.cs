using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    #region Public Properties
    public ItemName Name => name;
    public Sprite Icon => icon;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Name used to identify the item")]
    private ItemName name;
    [SerializeField]
    [Tooltip("Icon for this item")]
    private Sprite icon;
    #endregion
}
