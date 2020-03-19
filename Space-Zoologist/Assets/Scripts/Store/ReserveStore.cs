using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// TODO: figure out if there's a cleaner way to handle popup
// - it causes disconnect between event handling so the item selected has to be saved in a reference for when the actual buy button is clicked
// - and a null check is needed to handle cases where different items are selected
public class ReserveStore : MonoBehaviour
{
    // Can't make serialized fields readonly >:(
    [SerializeField] private GameObject UsableItem = default;
    [SerializeField] private GameObject StoreItemPopup = default;
    [Expandable] public List<StoreItemSO> StoreItemReferences = default;
    private List<GameObject> AvailableItems = new List<GameObject>();
    public GameObject PlayerInventory = default;
    private PlayerInventory playerInventory = default;
    private readonly ItemSelectedEvent OnItemSelected = new ItemSelectedEvent();
    private GameObject ItemSelected = default;

    public void Start()
    {
        OnItemSelected.AddListener(ConfirmPurchase);
        this.playerInventory = this.PlayerInventory.GetComponent<PlayerInventory>();  
    }

    public void InitializeStore()
    {
        foreach(StoreItemSO storeItem in this.StoreItemReferences)
        {
            AddItemToStore(storeItem);
        }
    }

    public void AddItemToStore(StoreItemSO storeItem)
    {
        GameObject newStoreItem = Instantiate(this.UsableItem, this.transform);
        UsableItem usableItem = newStoreItem.GetComponent<UsableItem>();
        usableItem.InitializeItem(storeItem, this.OnItemSelected);
        this.AvailableItems.Add(newStoreItem);
    }

    public void ConfirmPurchase(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        this.StoreItemPopup.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(BuyItem);
        this.StoreItemPopup.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ClosePopup);
        this.StoreItemPopup.SetActive(true);
    }

    public void BuyItem()
    {
        // If another item is selected, ClosePopup ensure the previous ItemSelected isn't bought
        if (this.ItemSelected != null)
        {
            // Find the item selected and then check if the player has enough funds
            UsableItem storeItem = this.ItemSelected.GetComponent<UsableItem>();
            if (storeItem.itemInfo.ItemCost <= this.playerInventory.PlayerFunds)
            {
                this.AvailableItems.Remove(this.ItemSelected);
                this.playerInventory.PlayerFunds -= storeItem.itemInfo.ItemCost;
                this.playerInventory.AddItem(storeItem.itemInfo);
                TestDisplayFunds();
                // Note: can't move item location to inventory because of how content displays work with GameObject locations in hierarchy
                this.ItemSelected.SetActive(false);
                ClosePopup();
            }
            else
            {
                Debug.Log("Insufficient Funds");
            }
        }
    }

    public void ClosePopup()
    {
        this.ItemSelected = null;
        this.StoreItemPopup.SetActive(false);
    }

    public void TestDisplayFunds()
    {
        GameObject.Find("PlayerFunds").GetComponent<Text>().text = "PlayerFunds: " + this.playerInventory.PlayerFunds;
    }
}
