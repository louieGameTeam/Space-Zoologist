using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Prefab should hold a reference to the scriptable object and handle buy event
/// </summary>
public class StoreItem : MonoBehaviour
{
    private ReserveStore reserveStore = default;
    public StoreItemSO itemInfo { get; set; }

    public void InitializeItem(StoreItemSO storeItem, GameObject newStoreItem, ReserveStore _reserveStore)
    {
        this.itemInfo = storeItem;
        this.reserveStore = _reserveStore;
        newStoreItem.transform.GetChild(0).GetComponent<Text>().text = this.itemInfo.ItemName;
        newStoreItem.transform.GetChild(1).GetComponent<Text>().text = this.itemInfo.ItemCost.ToString();
        newStoreItem.transform.GetChild(2).GetComponent<Text>().text = this.itemInfo.ItemDescription;
        newStoreItem.transform.GetChild(3).GetComponent<Image>().sprite = this.itemInfo.Sprite;
    }
  
    public void Buy()
    {
        this.reserveStore.TryToBuyItem(this.itemInfo.ItemName);
    }
}
