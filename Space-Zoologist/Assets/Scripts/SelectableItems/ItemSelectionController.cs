using UnityEngine;
using UnityEngine.Events;

public class ItemSelectionController : MonoBehaviour
{
    [SerializeField] ReserveStore StoreManager = default;
    [Expandable] public PlayerInformation playerInformation = default;
    [Tooltip("Should be attached to the Grid GameObject")]
    [SerializeField] TilePlacementController tilePlacementController = default;
    [SerializeField] TerrainTile TileToPlaceTest = default;

    public SetupItemPreviewEvent StartPreview = new SetupItemPreviewEvent();
    public UnityEvent StopPreview = new UnityEvent();

    private GameObject ItemSelected = default;
    private bool IsPlacingTile = false;
    private bool IsPreviewingStoreItem = false;
    private bool IsPlacing = false;
    // private tileEnum TileToPlace

    public void Start()
    {
        this.ItemSelected = null;
    }

    // Some of this or all of this logic including pre
    void Update()
    {
        if (this.IsPreviewingStoreItem)
        {
            this.PreviewStoreItem();
        }
    }

    // Should this be stores responsibility?
    public void PreviewStoreItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Resolves issue with item being clicked in store and being placed immediately
            this.IsPlacing = true;
        }
        if (this.IsPlacing)
        {
            // Start placing tiles, otherwise buy item and place immediately
            if (this.IsPlacingTile)
            {
                this.PlaceTerrainTile("this.TileToPlace");
            }   
            if (Input.GetMouseButtonUp(0))
            {
                // TODO update parameter when tile placement controller gets updated to count number of tiles
                this.StoreManager.BuyItem(ref this.playerInformation.PlayerFunds, this.ItemSelected, numCopies:1);//this.tilePlacementController.numTilesPlaced))
                this.FinishedPreviewing();
            }
        }
    }

    // TODO: Should be refactored into TilePlacementController 
    public void PlaceTerrainTile(string tileToPlace)
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.tilePlacementController.StartPreview(this.TileToPlaceTest);
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.tilePlacementController.StopPreview(this.TileToPlaceTest);
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            this.tilePlacementController.RevertChanges();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.tilePlacementController.isBlockMode = !this.tilePlacementController.isBlockMode;
        }
    }

    public void BeginStoreItemPreview(GameObject itemSelected)
    {
        this.StartPreview.Invoke(itemSelected.GetComponent<StoreItem>().ItemInformation.Sprite);
        this.IsPreviewingStoreItem = true;
        this.ItemSelected = itemSelected;
        StoreItemSO itemInformation = itemSelected.GetComponent<StoreItem>().ItemInformation;
        if (itemInformation.ItemIdentifier.Equals("Terrain"))
        {
            this.IsPlacingTile = true;
            // this.TileToPlace = itemInformation.ItemName;
        }
    }

    private void FinishedPreviewing()
    {
        this.IsPreviewingStoreItem = false;
        this.StopPreview.Invoke();
        this.IsPlacing = false;
        this.ItemSelected = null;
        this.IsPlacingTile = false;
    }
}
