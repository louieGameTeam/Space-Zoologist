using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

///// <summary>
///// Handles store item preview, placement, and purchasing
///// </summary>
//public class StoreItemController : MonoBehaviour
//{
//    [SerializeField] private ReserveStore StoreManager = default;
//    [SerializeField] private  PlayerInfo playerInformation = default;
//    [Tooltip("Should be attached to the Grid GameObject")]
//    [SerializeField] private  TilePlacementController tilePlacementController = default;
//    [Tooltip("Under Plot GameObject")]
//    [SerializeField] private FloatingObjectManager floatingObjectManager = default;
//    [Header("Tiles List is for testing purposes")]
//    [SerializeField] private List<TerrainTile> Tiles = default;
//    public SetupItemPreviewEvent StartPreview = new SetupItemPreviewEvent();
//    public UnityEvent StopPreview = new UnityEvent();

//    private TerrainTile TileToPlace = default;
//    private StoreItem StoreItemSelected = default;
//    private bool IsPlacingTile = false;
//    private bool IsPreviewingStoreItem = false;
//    private bool IsPlacing = false;
//    private int NumTilesPlace = 0;
//    // private tileEnum TileToPlace

//    public void Start()
//    {
//        this.StoreItemSelected = null;
//        this.tilePlacementController.selectedTile = Tiles[0];
//    }

//    // Some of this or all of this logic including pre
//    void Update()
//    {
//        if (this.IsPreviewingStoreItem)
//        {
//            this.PreviewStoreItem();
//        }
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            this.FinishedPreviewing();
//        }
//    }

//    private void PreviewStoreItem()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            // Resolves issue with item being clicked in store and being placed immediately
//            this.IsPlacing = true;
//        }
//        if (this.IsPlacing)
//        {      
//            // Start placing tiles, otherwise buy item and place immediately
//            if (this.IsPlacingTile)
//            {
//                this.NumTilesPlace = this.tilePlacementController.PlacedTileCount();
//                this.PlaceTerrainTile("this.TileToPlace");
//            }   
//            // Create new GameObject where the player has released the mouse button
//            else 
//            {
//                if (Input.GetMouseButtonUp(0))
//                {
//                    this.floatingObjectManager.CreateNewFloatingObject(this.StoreItemSelected.ItemInformation.Sprite);
//                }
//            }
//            if (Input.GetMouseButtonUp(0))
//            {
//                if (this.NumTilesPlace > 0)
//                {
//                    this.StoreManager.BuyItem(ref this.playerInformation.Funds, this.StoreItemSelected, this.NumTilesPlace);
//                }
//                else 
//                {
//                    this.StoreManager.BuyItem(ref this.playerInformation.Funds, this.StoreItemSelected, numCopies: 1);
//                }
//                this.FinishedPreviewing();
//            }    
//        }
//    }

//    // Could potentially be refactored into TileController
//    // TODO when tiles can be called by type, change the parameter to TileType
//    private void PlaceTerrainTile(string tileToPlace)
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            tilePlacementController.StartPreview(this.TileToPlace);
//        }
//        if (Input.GetMouseButtonUp(0))
//        {
//            tilePlacementController.StopPreview();
//        }
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            tilePlacementController.RevertChanges();
//        }
//        if (Input.GetKeyUp(KeyCode.B))
//        {
//            tilePlacementController.isBlockMode = !tilePlacementController.isBlockMode;
//        }
//    }

//    public void BeginStoreItemPreview(GameObject itemSelected)
//    {
//        StoreItem Item = itemSelected.GetComponent<StoreItem>();
//        this.StartPreview.Invoke(Item.ItemInformation.Sprite);
//        this.IsPreviewingStoreItem = true;
//        this.StoreItemSelected = Item;
//        if (this.StoreItemSelected.ItemInformation.StoreItemCategory.Equals("Tiles"))
//        {
//            // TODO update tilePlacementController to take in string
//            this.IsPlacingTile = true;
//            foreach (TerrainTile tileToPlace in this.Tiles)
//            {
//                if (tileToPlace.type.ToString().Equals(this.StoreItemSelected.ItemInformation.ItemIdentifier))
//                {
//                    this.TileToPlace = tileToPlace;
//                }
//            }
//        }
//    }

//    private void FinishedPreviewing()
//    {
//        this.StopPreview.Invoke();
//        this.IsPreviewingStoreItem = false;
//        this.IsPlacing = false;
//        this.StoreItemSelected = null;
//        this.IsPlacingTile = false;
//    }
//}
