using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates and sells store items and manages which one's are active
/// </summary>
public class StoreItemManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] private GameObject StoreItemPrefab = default;
    [SerializeField] private GameObject StoreContent = default;
    // made public so Expandable property works
    [Expandable] public List<StoreItemSO> StoreItemReferences = default;
    [SerializeField] private  PlayerInfo playerInformation = default;
    private List<GameObject> AvailableItems { set; get; }
    // Use to setup HUD events and player interaction events
    [Header("Store Item Controller and Sprite Follow Cursor")]
    public ItemSelectedEvent ItemSelectedEvent = new ItemSelectedEvent();

    public void Start()
    {
        this.AvailableItems = new List<GameObject>();
        foreach (StoreItemSO storeItem in this.StoreItemReferences)
        {    
            AddItem(storeItem);
        }
    }

    /// <summary>
    /// Creates a new store item based off of the store item prefab
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(StoreItemSO itemData)
    {
        GameObject newStoreItem = Instantiate(this.StoreItemPrefab, this.StoreContent.transform);
        newStoreItem.GetComponent<ItemData>().StoreItemData = itemData;
        newStoreItem.GetComponent<Image>().sprite = itemData.Sprite;
        this.SetupItemSelectedHandler(newStoreItem, this.ItemSelectedEvent);
        this.AvailableItems.Add(newStoreItem);
        newStoreItem.SetActive(false);
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent itemSelected)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(this.ItemSelectedEvent);
    }

    /// <summary>
    /// Updates playerFunds through reference if purchase is successful, otherwise returns false
    /// </summary>
    /// <param name="playerFunds"></param>
    /// <param name="itemToBuy"></param>
    /// <param name="numCopies"></param>
    /// <returns></returns>
    public bool BuyItem(ItemData itemToBuy, int numCopies)
    {
        bool isPurchaseSuccessful = false;
        if (itemToBuy != null)
        {
            float totalCost = itemToBuy.StoreItemData.ItemCost * numCopies;
            // Check if the player has enough funds
            if (totalCost > 0 && totalCost <= this.playerInformation.Funds)
            {
                this.playerInformation.Funds -= totalCost;
                isPurchaseSuccessful = true;
            }
            else
            {
                isPurchaseSuccessful = false;
            }
        }
        return isPurchaseSuccessful;
    }

    public void DisplayCategory(string category)
    {
        foreach (GameObject storeItem in this.AvailableItems)
        {
            if (storeItem.GetComponent<ItemData>().StoreItemData.StoreItemCategory.Equals(category))
            {
                storeItem.SetActive(true);
            }
            else 
            {
                storeItem.SetActive(false);
            }
        }
    }
}
