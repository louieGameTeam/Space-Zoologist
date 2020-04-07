using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ReserveStore : MonoBehaviour
{
    [Tooltip("Attached to grid GameObject")]
    [SerializeField] private GameObject SelectableItemPrefab = default;
    [Expandable] public List<ScriptableObject> StoreItemReferences = default;
    [Tooltip("Under Store Scroll View GameObject")]
    [SerializeField] GameObject StoreContent = default;
    [SerializeField] PlayerController playerController = default;
    // Can be used to remove items 
    private List<GameObject> AvailableItems = new List<GameObject>();

    public void InitializeStore()
    {
        if (this.AvailableItems.Count == 0)
        {
            foreach (ScriptableObject storeItem in this.StoreItemReferences)
            {
                InitializeItem(storeItem);
            }
        }
    }

    public void InitializeItem(ScriptableObject storeItem)
    {
        GameObject newItem = Instantiate(this.SelectableItemPrefab, this.StoreContent.transform);
        SelectableItem selectableItem = newItem.GetComponent<SelectableItem>();
        selectableItem.Initialize(storeItem, this.playerController.OnItemSelectedEvent);
        this.AvailableItems.Add(newItem);
    }

    // Can be modified to remove items or have more functionality easily
    public bool BuyItem(ref float playerFunds, GameObject itemToBuy, int numCopies)
    {
        if (itemToBuy != null)
        {
            float totalCost = itemToBuy.GetComponent<SelectableItem>().ItemInfo.ItemCost * numCopies;
            // Check if the player has enough funds
            if (totalCost <= playerFunds)
            {
                playerFunds -= totalCost;
                Debug.Log("Item purchased");
                return true;
            }
            else
            {
                Debug.Log("Insufficient funds");
                return false;
            }
        }
        else
        {
            this.CancelPurchase();
            return false;
        }
    }

    // Will be used to handle selling
    public void CancelPurchase()
    {
        Debug.Log("Purchase cancelled");
    }
}
