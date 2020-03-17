using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private float playerFunds = default;
    public float PlayerFunds { get => playerFunds; set => playerFunds = value; }
    private List<StoreItem> OwnedItems = new List<StoreItem>();

    public void AddItem(StoreItem item)
    {
        this.OwnedItems.Add(item);
    }

    public void DisplayItems()
    {
        string items = "PlayerFunds: " + playerFunds.ToString();
        foreach (StoreItem ownedItem in this.OwnedItems)
        {
            items += " " + ownedItem.itemInfo.ItemName;
        }
        this.GetComponent<Text>().text = items;
    }
}
