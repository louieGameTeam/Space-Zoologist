using UnityEngine;
using UnityEngine.Events;

// some code commented out until TilePlacementController updated with needed features
public class PlayerController : MonoBehaviour
{
    [SerializeField] ReserveStore StoreManager = default;
    [SerializeField] public float PlayerFunds = default;
    [Tooltip("Should be attached to the Grid GameObject")]
    [SerializeField] TilePlacementController tilePlacementController = default;
    private TerrainTile TileToBuy = null;
    public GameObject ItemSelected { get; set; }
    public int NumCopies { get; set; }
    public ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();
    // TODO: figure out a better way of communicating with HUD
    public UnityEvent CloseStore = new UnityEvent();
    public UnityEvent ItemPurchased = new UnityEvent();

    public void Start()
    {
        this.ItemSelected = null;
        this.NumCopies = 1;
        this.OnItemSelectedEvent.AddListener(this.OnItemSelected);
    }

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
            if (this.StoreManager.BuyItem(ref this.PlayerFunds, this.ItemSelected, this.NumCopies))
            {
                // this.tilePlacementController.StopKeepingTrack();   
            }
            else
            {
                this.tilePlacementController.RevertChanges();
            }
            this.TileToBuy = null;
            this.ItemPurchased.Invoke();
            this.ItemSelected = null;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.StoreManager.CancelPurchase();
            this.tilePlacementController.RevertChanges();
            this.TileToBuy = null;
            this.ItemSelected = null;
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            this.tilePlacementController.isBlockMode = !this.tilePlacementController.isBlockMode;
        }
        // this.NumCopies = this.tilePlacementController.NumTilesPlaced;
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
            this.ItemPurchased.Invoke();
            this.ItemSelected = null;
        }
        this.CloseStore.Invoke();
        // Need to get sprite of item and apply to gameobject, set true, then lerp from gameobject to cursor position in update
    }
}
