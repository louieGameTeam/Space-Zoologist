using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Holds information about item and handles ItemSelection
/// </summary>
public class ItemSelectedEvent : UnityEvent<GameObject> { }
public class UsableItem : MonoBehaviour
{
    public StoreItemSO itemInfo { get; set; }
    private ItemSelectedEvent DoSomethingWithItem = new ItemSelectedEvent();

    public void InitializeItem(StoreItemSO storeItem, ItemSelectedEvent action)
    {
        this.itemInfo = storeItem;
        this.DoSomethingWithItem = action;
        this.gameObject.transform.GetChild(0).GetComponent<Text>().text = this.itemInfo.ItemName;
        this.gameObject.transform.GetChild(1).GetComponent<Text>().text = this.itemInfo.ItemCost.ToString();
        this.gameObject.transform.GetChild(2).GetComponent<Text>().text = this.itemInfo.ItemDescription;
        this.gameObject.transform.GetChild(3).GetComponent<Image>().sprite = this.itemInfo.Sprite;
    }
  
    public void ItemSelected()
    {
        this.DoSomethingWithItem.Invoke(this.gameObject);
    }
}
