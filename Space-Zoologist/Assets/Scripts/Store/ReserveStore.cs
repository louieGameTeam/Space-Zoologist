using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ReserveStore : MonoBehaviour, ISelectableItem
{
    [SerializeField] GameObject Grid = default;
    private TilePlacementController TilePlacementController = default;
    [SerializeField] private GameObject SelectableItemPrefab = default;
    [Expandable] public List<ScriptableObject> StoreItemReferences = default;
    [SerializeField] GameObject StoreContent = default;
    private List<GameObject> AvailableItems = new List<GameObject>();
    [SerializeField] float PlayerFunds = default;
    [SerializeField] GameObject PlayerFundsDisplay;
    // [SerializeField] private GameObject PlayerInventoryContent = default;
    // private PlayerInventory playerInventory = default;
    private readonly ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();
    private GameObject ItemSelected = default;
    public UnityEvent CloseStore = default;
    private bool BuyingTile = false;
    private float TotalCost = 0;
    private float ItemCost = 0;
    private TerrainTile selectedTile = default;

    public void Start()
    {
        this.OnItemSelectedEvent.AddListener(this.OnItemSelected);
        this.TilePlacementController = this.Grid.GetComponent<TilePlacementController>();
    }

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
        selectableItem.Initialize(storeItem, this.OnItemSelectedEvent);
        this.AvailableItems.Add(newItem);
    }

    public void Update()
    {
        if (this.BuyingTile && this.selectedTile != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TilePlacementController.StartPreview(this.selectedTile);
            }
            if (Input.GetMouseButtonUp(0))
            {
                TilePlacementController.StopPreview(this.selectedTile);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.TilePlacementController.ResetTileCounter();
                BuyItem();
                this.BuyingTile = false;
            }
            this.TotalCost = this.TilePlacementController.NumTilesPlaced;
            this.TestDisplayFunds();
        }
    }

    public void OnItemSelected(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        if (itemSelected.GetComponent<SelectableItem>().ItemInfo.ItemDescription.Equals("landscaping"))
        {
            this.BuyingTile = true;
            this.selectedTile = (TerrainTile)itemSelected.GetComponent<SelectableItem>().OriginalItem;
        }
        this.ItemCost = this.ItemSelected.GetComponent<SelectableItem>().ItemInfo.ItemCost;
        this.CloseStore.Invoke();
        // TODO: when item is selected, start listening for when player lets go in update
    }

    private void BuyItem()
    {
        if (this.ItemSelected != null)
        {
            // Check if the player has enough funds
            if (this.TotalCost <= this.PlayerFunds)
            {
                this.PlayerFunds -= this.TotalCost;
                //this.AvailableItems.Remove(this.ItemSelected);
                // Note: can't move SelectableItem GameObject from StoreContent GameObject to InventoryContent GameObject in hierarchy,
                // so a new SelectableItem GameObject has to be created
                this.ItemSelected.SetActive(false);
            }
            else
            {
                if (this.BuyingTile)
                {
                    Debug.Log("not reverting");
                    this.TilePlacementController.RevertChanges();
                }
            }
        }
    }

    public void TestDisplayFunds()
    {
        if (this.ItemSelected != null)
        {
            this.PlayerFundsDisplay.GetComponent<Text>().text = "Funds: " + this.PlayerFunds +
            "(-" + this.TotalCost + ")";
        }
    }
}
