
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// TODO setup player balance to show remaining items and strobe when out
/// <summary>
/// A section of items in the store. Subclass for specific behavior regarding what happens after an item is selected.
/// </summary>
public class StoreSection : MonoBehaviour
{
    public ItemRegistry.Category ItemType => itemType;
    public Item SelectedItem => selectedItem;

    // Can't display in editor anymore because it is in a prefab
    /*[SerializeField] */private GraphicRaycaster raycaster;

    protected ItemRegistry.Category itemType = default;
    [Header("Dependencies")]
    [SerializeField] private Transform itemGrid = default;
    [SerializeField] private GameObject itemCellPrefab = default;
    // Cursor dependencies
    protected UICursorInput cursorInput = default;
    protected ItemPlaceCursorPreviewMover cursorPreviewObject = null;

    protected List<RectTransform> UIElements = default;
    protected TileDataController GridSystem = default;
    protected ResourceManager ResourceManager = default;
    protected TilePlacementController tilePlacementController = default;
    protected Dictionary<Item, StoreItemCell> storeItems = new Dictionary<Item, StoreItemCell>();
    protected Item selectedItem = null;
    private Vector3Int previousLocation = default;
    protected int currentAudioIndex = 0;

    public void SetupDependencies(UICursorInput cursorItem, List<RectTransform> UIElements, ResourceManager resourceManager)
    {
        this.cursorInput = cursorItem;
        this.UIElements = UIElements;
        this.GridSystem = GameManager.Instance.m_tileDataController;
        this.ResourceManager = resourceManager;
        this.tilePlacementController = GameManager.Instance.m_tilePlacementController;

        resourceManager.OnRemainingResourcesChanged += UpdateDisplayValue;
    }
    public Item GetItemByID(ItemID id)
    {
        foreach (Item item in this.storeItems.Keys)
        {
            if (item.ID.Equals(id))
            {
                return item;
            }
        }
        return null;
    }

    public virtual void Update()
    {
        if (selectedItem) {
            HandleCursor ();
        }
        if (cursorInput.IsOn)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(cursorInput.transform.position);
            Vector3Int gridLocation = GridSystem.WorldToCell(mousePosition);
            cursorPreviewObject?.UpdatePosition();
            if (!GridSystem.IsWithinGridBounds(mousePosition))
            {
                previousLocation = gridLocation;
                GridSystem.ClearHighlights();
                return;
            }
            if (gridLocation.x != previousLocation.x || gridLocation.y != previousLocation.y)
            {
                previousLocation = gridLocation;
                GridSystem.ClearHighlights();
                //Invalid if the level does not allow placing on walls
                if (this.GridSystem.IsOnWall(gridLocation) && !GameManager.Instance.LevelData.WallBreakable) return;
                GridSystem.updateVisualPlacement(gridLocation, selectedItem);
            }
        }
    }

    public virtual void Initialize()
    {
        // Try to get the raycaster in the parent
        raycaster = GetComponentInParent<GraphicRaycaster>();
        // Add the items to the store section
        LevelData levelData = GameManager.Instance.LevelData;
        foreach (ItemData itemData in ItemRegistry.GetAllItems())
        {
            Item item = itemData.ShopItem;
            if (item)
            {
                if (item.Type.Equals(itemType))
                {
                    this.AddItem(item);
                }
            }
        }

    }

    /// <summary>
    /// Add item to the section.
    /// </summary>
    public void AddItem(Item item)
    {
        GameObject newItemCellGO = Instantiate(itemCellPrefab, itemGrid);
        StoreItemCell itemCell = newItemCellGO.GetComponent<StoreItemCell>();
        itemCell.Initialize(item, false, OnItemSelected);
        if (this.ResourceManager.hasLimitedSupply(item.ID))
        {
            if (!storeItems.ContainsKey(item))
                storeItems.Add(item, itemCell);
            // try to get initial value
            itemCell.RemainingAmount = ResourceManager.CheckRemainingResource(item);
        }
    }

    private void UpdateDisplayValue(ItemID item, int newQuantity)
    {
        foreach(var v in storeItems)
        {
            if (v.Key.ID == item)
                v.Value.RemainingAmount = newQuantity;
        }
        //storeItems[ItemRegistry.Get(item).ShopItem].RemainingAmount = newQuantity;
    }

    public void AddItemQuantity(Item item, int count = 1)
    {
        if (!storeItems.ContainsKey(item))
        {
            AddItem(item);
        }
        else
        {
            this.ResourceManager.ChangeItemQuantity(item.ID, count);
            //storeItems[item].RemainingAmount += count;
        }
    }

    public int SellItem(Item item, int amount)
    {
        int soldAmount = ResourceManager.SellItem(item, amount);
        GameManager.Instance.AddToBalance(soldAmount * item.Price);
        return soldAmount;
    }

    /// <summary>
    /// Triggered by items in the section.
    /// </summary>
    /// <param name="item">The item that was selected.</param>
    public virtual void OnItemSelected(Item item)
    {
        if (!this.CanBuy(item))
        {
            OnItemUnavailable();
            return;
        }
        AudioManager.instance.PlayOneShot(SFXType.Valid);
        cursorInput.Begin(OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
        CreateCursorPreview(item);
        selectedItem = item;
    }

    protected void CreateCursorPreview(Item item)
    {        
        Vector2Int size = Vector2Int.zero;
        bool snap = false;
        if(item.Type == ItemRegistry.Category.Food)
        {
            snap = true;
            size = GameManager.Instance.FoodSources[item.ID].Size;
        }
        else if(item.Type == ItemRegistry.Category.Tile)
        {
            snap = true;
            size = new Vector2Int(1, 1);
        }
        cursorPreviewObject = new ItemPlaceCursorPreviewMover(GridSystem, item.Icon, size, snap);
    }

    public virtual void OnItemSelectionCanceled()
    {
        cursorInput.Stop(OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
        cursorPreviewObject.Clear();
        cursorPreviewObject = null;
        selectedItem = null;
        AudioManager.instance?.PlayOneShot(SFXType.Cancel);
        GridSystem.ClearHighlights();
    }

    public virtual void OnItemUnavailable()
    {
        AudioManager.instance.PlayOneShot(SFXType.Unavailable);
    }

    public void OnCursorItemClicked(PointerEventData eventData)
    {/*
        if (!this.CanBuy(this.selectedItem))
        {
            OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnItemSelectionCanceled();
        }*/
    }

    public bool CanBuy(Item item)
    {
        if (item != null && storeItems.ContainsKey(item) && ResourceManager.CheckRemainingResource(item) == 0)
        {
            return false;
        }
        return true;
    }

    public virtual void HandleCursor () {
        if (Input.GetMouseButtonDown (1) || 
            (Input.GetMouseButtonDown (0) && (!this.CanBuy (this.selectedItem) || !UIBlockerSettings.OperationIsAvailable ("Build")))) {
            OnItemSelectionCanceled ();
            return;
        }
    }

    /// <summary>
    /// Overwritten by child classes.
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnCursorPointerDown(PointerEventData eventData)
    {

    }

    /// <summary>
    /// Overwritten by child classes.
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnCursorPointerUp(PointerEventData eventData)
    {
        /*if (!this.CanBuy(this.selectedItem))
        {
            OnItemSelectionCanceled();
            return;
        }*/
    }

    private void OnDisable()
    {
        cursorInput.Stop(OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
    }

    protected virtual void HandleAudio()
    {
        if (selectedItem.AudioClips.Count == 0)
        {
            Debug.Log("Selected item " + selectedItem.ItemName + " has no audio sources!");
            return;
        }
        if (selectedItem.AudioClips.Count > 1)
        {
            currentAudioIndex += 1;
            if (currentAudioIndex >= selectedItem.AudioClips.Count)
            {
                currentAudioIndex = 0;
            }
        }
        AudioManager.instance.PlayOneShot(selectedItem.AudioClips[currentAudioIndex]);
    }
}
