using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private float playerFunds = default;
    public float PlayerFunds { get => playerFunds; set => playerFunds = value; }
    private List<GameObject> UsableItems = new List<GameObject>();

    // TODO: instantiate item in view? or move item from store?
    public void AddItem(GameObject item)
    {
        this.UsableItems.Add(item);
    }

    public void SellItem()
    {
        foreach (GameObject item in this.UsableItems)
        {
            // Find the item selected and then add some lesser amount to player funds
            UsableItem itemToRemove = item.GetComponent<UsableItem>();
            if (itemToRemove.itemSelected)
            {
                itemToRemove.itemSelected = false;
                float something = 0.5f;
                this.playerFunds += itemToRemove.itemInfo.ItemCost * something;
                item.SetActive(false);
                this.UsableItems.Remove(item);
            }
        }
    }

    public void DisplayItems()
    {
        string items = "PlayerFunds: " + playerFunds.ToString();
        foreach (GameObject ownedItem in this.UsableItems)
        {
            items += " " + ownedItem.GetComponent<UsableItem>().itemInfo.ItemName;
        }
        this.GetComponent<Text>().text = items;
    }
}
