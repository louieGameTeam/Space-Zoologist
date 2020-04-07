using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Known issues:
// Replaced tiles don't get reverted and their cost still increases
// Switching from liquid to other tile placement can cause issues:
// - TileContentsManager l:113 - key not present (only sometimes)
// - tiles not counted since liquid tiles placed differently
// Blockmode doesn't working when selecting different areas to start from and count not updated properly

public class PlayerController : MonoBehaviour
{
    [SerializeField] ReserveStore StoreManager = default;
    [SerializeField] public float PlayerFunds = default;
    [SerializeField] GameObject PlayerFundsDisplay;
    [SerializeField] TilePlacementController tilePlacementController = default;
    [SerializeField] HUDTesting2 HUD = default;
    private TerrainTile TileToBuy = null;
    public GameObject ItemSelected = default;
    public int NumCopies = 0;
    public readonly ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.TileToBuy != null)
        {
            this.PlaceTerrainTile(this.TileToBuy);
            
        }
    }

    private void PlaceTerrainTile(TerrainTile tileToPlace)
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.tilePlacementController.StartPreview(tileToPlace);
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.tilePlacementController.StopPreview(tileToPlace);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed");
            if (this.StoreManager.BuyItem(ref this.PlayerFunds, this.ItemSelected, this.NumCopies))
            {
                this.tilePlacementController.StopKeepingTrack();   
            }
            else
            {
                this.tilePlacementController.RevertChanges();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed");
            this.StoreManager.CancelPurchase();
            this.tilePlacementController.RevertChanges();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.tilePlacementController.isBlockMode = !this.tilePlacementController.isBlockMode;
        }
        this.NumCopies = this.tilePlacementController.NumTilesPlaced;
    }

    // all items that can be selected by play should go through this
    // there needs to be a way of determining what item was selected-category? itemdescription?
    public void OnItemSelected(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        if (itemSelected.GetComponent<SelectableItem>().ItemInfo.ItemDescription.Equals("landscaping"))
        {
            this.TileToBuy = (TerrainTile)itemSelected.GetComponent<SelectableItem>().OriginalItem;
        }
        else
        {
            this.StoreManager.BuyItem(ref this.PlayerFunds, this.ItemSelected, this.NumCopies);
        }
        // Need to get sprite of item and apply to gameobject, set true, then lerp from gameobject to cursor position in update
    }
}
