using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Handles store item preview, placement, and purchasing
/// </summary>
public class StoreItemController : MonoBehaviour, IHandler
{
    [SerializeField] private StoreManager StoreManager = default;
    [Tooltip("Should be attached to the Grid GameObject")]
    [SerializeField] private  TilePlacementController tilePlacementController = default;
    [Tooltip("Under Plot GameObject")]
    [SerializeField] private FloatingObjectManager floatingObjectManager = default;
    [Header("Tiles List is for testing purposes")]
    [SerializeField] private List<TerrainTile> Tiles = default;
    public UnityEvent StopPreview = new UnityEvent();

    private TerrainTile TileToPlace = default;
    private StoreItemData StoreItemSelected = null;
    private bool IsPlacingTile = false;
    private bool IsPreviewingStoreItem = false;
    private bool IsPlacing = false;
    private int NumObjectsToBuy = 1;
    // private tileEnum TileToPlace

    public void Start()
    {
        this.tilePlacementController.selectedTile = Tiles[0];
    }

    void Update()
    {
        if (this.IsPreviewingStoreItem)
        {
            this.PreviewStoreItem();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.FinishedPreviewing();
        }
    }

    // Logic for specific handling of an item when selected
    private void PreviewStoreItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // prevent clicks through UI layer with raycast target enabled 
            if (EventSystem.current.IsPointerOverGameObject())
            {
                this.IsPlacing = false;
                return;
            }
            this.IsPlacing = true;
        }
        if (this.IsPlacing)
        {      
            if (this.IsPlacingTile)
            {
                this.NumObjectsToBuy = this.tilePlacementController.PlacedTileCount();
                this.PlaceTerrainTile("this.TileToPlace");
            } 
            if (Input.GetMouseButtonUp(0))
            {
                this.TryToPurchaseItem();
            }    
        }
    }

    // TODO Should be refactored into TileController
    private void PlaceTerrainTile(string tileToPlace)
    {
        if (Input.GetMouseButtonDown(0))
        {
            tilePlacementController.StartPreview(this.TileToPlace);
        }
        if (Input.GetMouseButtonUp(0))
        {
            tilePlacementController.StopPreview();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            tilePlacementController.RevertChanges();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            tilePlacementController.isBlockMode = !tilePlacementController.isBlockMode;
        }
    }

    public string GetItemPreviewingInfo()
    {
        string ItemPreviewingInfo = "";
        if (this.StoreItemSelected != null)
        {
            // if previewing tiles but no tiles placed yet
            if (this.IsPlacingTile && this.NumObjectsToBuy == 0)
            {
                ItemPreviewingInfo = "(-" +this.StoreItemSelected.ItemData.ItemCost + ")";
            }
            else
            {
                ItemPreviewingInfo = "(-" + this.NumObjectsToBuy * this.StoreItemSelected.ItemData.ItemCost + ")";
            }
        }
        return ItemPreviewingInfo;
    }

    private void TryToPurchaseItem()
    {
        bool purchaseSuccessful = this.StoreManager.BuyItem(this.StoreItemSelected, this.NumObjectsToBuy);
        if (purchaseSuccessful && !this.IsPlacingTile)
        {
            this.floatingObjectManager.CreateNewFloatingObject(this.StoreItemSelected);
        }
        else if (!purchaseSuccessful && this.IsPlacingTile)
        {
            this.tilePlacementController.RevertChanges();  
        }
    }

    public void OnItemSelectedEvent(GameObject itemSelected)
    {
        // Clear up old preview data if new item selected
        this.FinishedPreviewing();
        this.SetupStoreItemPreview(itemSelected);
    }
    // TODO refactor when tilesystem and store identification more finalized
    private void SetupStoreItemPreview(GameObject itemSelected)
    {
        this.StoreItemSelected = itemSelected.GetComponent<StoreItemData>();
        this.IsPreviewingStoreItem = true;
        if (this.StoreItemSelected.ItemData.StoreItemCategory.Equals("Tiles"))
        {
            this.IsPlacingTile = true;
            foreach (TerrainTile tileToPlace in this.Tiles)
            {
                if (tileToPlace.type.ToString().Equals(this.StoreItemSelected.ItemData.ItemIdentifier))
                {
                    this.TileToPlace = tileToPlace;
                }
            }
        }
    }

    public void FinishedPreviewing()
    {
        this.StopPreview.Invoke();
        this.IsPreviewingStoreItem = false;
        this.IsPlacing = false;
        this.StoreItemSelected = null;
        this.IsPlacingTile = false;
        this.NumObjectsToBuy = 1;
    }
}
