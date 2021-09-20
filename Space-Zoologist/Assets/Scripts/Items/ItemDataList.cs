using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataList
{
    #region Public Properties
    public ItemData[] List => list;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of item datas")]
    private ItemData[] list;
    #endregion
}
