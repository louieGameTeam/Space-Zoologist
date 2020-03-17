using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserveStore : MonoBehaviour
{
    [SerializeField] public GameObject StoreItemPrefab = default;
    // Can't make serialized fields readonly >:(
    [Expandable] public List<StoreItemSO> StoreItemReferences = default;
    private List<GameObject> AvailableItems = new List<GameObject>();
    public PlayerInventory playerInventory = default;

    public void InitializeStore()
    {
        foreach(StoreItemSO storeItem in this.StoreItemReferences)
        {
            AddItemToStore(storeItem);
        }
    }

    public void AddItemToStore(StoreItemSO storeItem)
    {
        GameObject newStoreItem = Instantiate(this.StoreItemPrefab, this.transform);
        newStoreItem.GetComponent<UsableItem>().InitializeItem(storeItem, newStoreItem);
        this.AvailableItems.Add(newStoreItem);
    }

    // TODO: figure out how to handle removed items
    public void BuyItem()
    {
        foreach(GameObject availableItem in this.AvailableItems)
        {
            // Find the item selected and then check if the player has enough funds
            UsableItem storeItem = availableItem.GetComponent<UsableItem>();
            if (storeItem.itemSelected)
            {
                storeItem.itemSelected = false;
                if (storeItem.itemInfo.ItemCost <= this.playerInventory.PlayerFunds)
                {
                    this.AvailableItems.Remove(availableItem);
                    this.playerInventory.PlayerFunds -= storeItem.itemInfo.ItemCost;
                    this.playerInventory.AddItem(availableItem);
                    availableItem.SetActive(false);
                    break;
                }
                else
                {
                    Debug.Log("Insufficient Funds");
                }
            }
            else
            {
                Debug.Log("Item not selected");
            }
        }
    }
}
