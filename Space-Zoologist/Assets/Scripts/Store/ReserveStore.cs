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
    public GameObject ItemToBuy { get; set; }
    public PlayerInventory playerInventory = default;

    public void InitializeStore()
    {
        int xLocation = 200, yLocation = 400;
        foreach(StoreItemSO storeItem in this.StoreItemReferences)
        {
            AddItemToStore(storeItem, xLocation, yLocation);
            xLocation += 300;
        }
    }

    public void AddItemToStore(StoreItemSO storeItem, int xLocation, int yLocation)
    {
        Vector3 ItemPosition = new Vector3(xLocation, yLocation, this.transform.position.z);      
        GameObject newStoreItem = Instantiate(this.StoreItemPrefab, ItemPosition, Quaternion.identity, this.transform);
        newStoreItem.GetComponent<StoreItem>().InitializeItem(storeItem, newStoreItem, this);
        this.AvailableItems.Add(newStoreItem);
    }

    // Should the price checking be done in the store or in each store item?
    // TODO: if item has same name, first reference will be removed
    public void TryToBuyItem(string itemToBuy)
    {
        foreach(GameObject availableItem in this.AvailableItems)
        {
            // Find the item and then check if the player has enough funds
            StoreItem storeItem = availableItem.GetComponent<StoreItem>();
            if (System.String.Equals(storeItem.itemInfo.ItemName, itemToBuy))
            {
                if (storeItem.itemInfo.ItemCost <= this.playerInventory.PlayerFunds)
                {
                    this.AvailableItems.Remove(availableItem);
                    this.ItemToBuy = availableItem;
                    this.playerInventory.PlayerFunds -= storeItem.itemInfo.ItemCost;
                    break;
                }
                {
                    this.ItemToBuy = null;
                }
            }
        }
    }

    public void ConfirmPurchase()
    {
        if (this.ItemToBuy == null)
        {
            Debug.Log("Insufficient Funds");
        }
        else
        {
            this.playerInventory.AddItem(this.ItemToBuy.GetComponent<StoreItem>());
            this.ItemToBuy.SetActive(false);
            this.ItemToBuy = null;
        }
    }
}
