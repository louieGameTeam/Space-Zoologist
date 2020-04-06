using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Replaced tiles don't get reverted and their cost still increases
// Switching from liquid to dirt placement can cause issues:
// - TileContentsManager l:113 - key not present (only sometimes)
// - tiles not counted since liquid tiles placed differently
// Block doesn't working when selecting different areas to start from and count not updated properly
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
    private TerrainTile TileToBuy = null;
    private float TotalCost = 0;

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
        if (this.TileToBuy != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.TilePlacementController.StartPreview(this.TileToBuy);
            }
            if (Input.GetMouseButtonUp(0))
            {
                this.TilePlacementController.StopPreview(this.TileToBuy);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space pressed");
                BuyItem();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Escape pressed");
                this.CancelPurchase();
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                this.TilePlacementController.isBlockMode = !this.TilePlacementController.isBlockMode;
            }
            // TODO: have block mode work for handling different starting points
            this.TotalCost = this.TilePlacementController.NumTilesPlaced;
        }
        this.TestDisplayFunds();
    }

    public void OnItemSelected(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        this.TotalCost = this.ItemSelected.GetComponent<SelectableItem>().ItemInfo.ItemCost;
        if (itemSelected.GetComponent<SelectableItem>().ItemInfo.ItemDescription.Equals("landscaping"))
        {
            this.TileToBuy = (TerrainTile)itemSelected.GetComponent<SelectableItem>().OriginalItem;
        }
        else
        {
            this.BuyItem();
        }
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
                if (this.TileToBuy != null)
                {
                    this.TilePlacementController.StopKeepingTrack();
                    this.TileToBuy = null;
                    Debug.Log("Tile(s) purchased");
                }
                else
                {
                    Debug.Log("Item purchased");
                }
            }
            else
            {
                if (this.TileToBuy != null)
                {
                    Debug.Log("Insufficient funds");
                    this.CancelPlacement();
                }
            }
        }
        this.TotalCost = 0;
    }

    public void CancelPurchase()
    {
        if (this.TileToBuy != null)
        {
            this.TilePlacementController.RevertChanges();
            this.TileToBuy = null;
        }
        this.TotalCost = 0;
        Debug.Log("Purchase cancelled");
    }

    public void CancelPlacement()
    {
        this.TilePlacementController.RevertChanges();
        Debug.Log("Placement cancelled");
    }

    public void TestDisplayFunds()
    {
        if (this.TotalCost > 0)
        {
            this.PlayerFundsDisplay.GetComponent<Text>().text = "Funds: " + this.PlayerFunds +
            "(-" + this.TotalCost + ")";
        }
        else
        {
            this.PlayerFundsDisplay.GetComponent<Text>().text = "Funds: " + this.PlayerFunds;
        }
    }
}
