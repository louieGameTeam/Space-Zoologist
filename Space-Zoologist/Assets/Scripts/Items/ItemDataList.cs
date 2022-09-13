using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataList
{
    #region Public Properties
    public ItemData[] Items => items;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of item datas")]
    private ItemData[] items = null;
    #endregion
}
