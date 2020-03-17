using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Prefab should hold a reference to the scriptable object and handle buy event
/// </summary>
public class UsableItem : MonoBehaviour
{
    public StoreItemSO itemInfo { get; set; }
    public bool itemSelected { get; set; }

    public void InitializeItem(StoreItemSO storeItem, GameObject newStoreItem)
    {
        this.itemInfo = storeItem;
        newStoreItem.transform.GetChild(0).GetComponent<Text>().text = this.itemInfo.ItemName;
        newStoreItem.transform.GetChild(1).GetComponent<Text>().text = this.itemInfo.ItemCost.ToString();
        newStoreItem.transform.GetChild(2).GetComponent<Text>().text = this.itemInfo.ItemDescription;
        newStoreItem.transform.GetChild(3).GetComponent<Image>().sprite = this.itemInfo.Sprite;
    }
  
    public void ItemSelected()
    {
        this.itemSelected = true;
    }
}
