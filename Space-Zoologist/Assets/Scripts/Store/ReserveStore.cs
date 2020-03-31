using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ReserveStore : MonoBehaviour, ISelectableItem
{
    [SerializeField] private GameObject SelectableItemPrefab = default;
    [SerializeField] private GameObject StoreItemPopupPrefab = default;
    [Expandable] public List<ScriptableObject> StoreItemReferences = default;
    private List<GameObject> AvailableItems = new List<GameObject>();
    [SerializeField] private GameObject PlayerInventoryContent = default;
    private PlayerInventory playerInventory = default;
    private readonly ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();
    private GameObject ItemSelected = default;

    public void Start()
    {
        this.OnItemSelectedEvent.AddListener(this.OnItemSelected);
        // Prefabs can't hold a reference to any specific scene objects so events have to be added by code
        this.StoreItemPopupPrefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(BuyItem);
        this.StoreItemPopupPrefab.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ClosePopup);
        this.playerInventory = this.PlayerInventoryContent.GetComponent<PlayerInventory>();  
    }

    public void InitializeStore()
    {
        foreach(ScriptableObject storeItem in this.StoreItemReferences)
        {
            InitializeItem(storeItem);
        }
    }

    public void InitializeItem(ScriptableObject storeItem)
    {
        GameObject newItem = Instantiate(this.SelectableItemPrefab, this.transform);
        SelectableItem selectableItem = newItem.GetComponent<SelectableItem>();
        selectableItem.Initialize(storeItem, this.OnItemSelectedEvent);
        this.AvailableItems.Add(newItem);
    }

    // TODO: figure out if there's a better way to handle disconnect between item selected and a popup
    // - is there a clean way to pass the itemSelected between each event call instead of saving in the ItemSelected instance field
    public void OnItemSelected(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        this.StoreItemPopupPrefab.SetActive(true);
    }

    private void BuyItem()
    {
        // If another item is selected, ClosePopup ensures the previous ItemSelected isn't bought
        if (this.ItemSelected != null)
        {
            // Get the ItemInfo and then check if the player has enough funds
            SelectableItem storeItem = this.ItemSelected.GetComponent<SelectableItem>();
            if (storeItem.ItemInfo.ItemCost <= this.playerInventory.PlayerFunds)
            {
                this.AvailableItems.Remove(this.ItemSelected);
                this.playerInventory.PlayerFunds -= storeItem.ItemInfo.ItemCost;
                this.playerInventory.InitializeItem(storeItem.ItemInfo);
                // Note: can't move SelectableItem GameObject from StoreContent GameObject to InventoryContent GameObject in hierarchy so need to create a new
                // so a new SelectableItem GameObject has to be created
                this.ItemSelected.SetActive(false);
            }
            else
            {
                Debug.Log("Insufficient Funds");
            }
        }
        ClosePopup();
    }

    private void ClosePopup()
    {
        this.ItemSelected = null;
        this.StoreItemPopupPrefab.SetActive(false);
        TestDisplayFunds();
    }

    public void TestDisplayFunds()
    {
        GameObject.Find("PlayerFunds").GetComponent<Text>().text = "Funds: " + this.playerInventory.PlayerFunds;
    }
}
