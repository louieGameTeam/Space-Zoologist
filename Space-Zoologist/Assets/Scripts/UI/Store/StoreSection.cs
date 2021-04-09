
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// TODO setup player balance to show remaining items and strobe when out
/// <summary>
/// A section of items in the store. Subclass for specific behavior regarding what happens after an item is selected.
/// </summary>
public class StoreSection : MonoBehaviour
{
    public ItemType ItemType => itemType;

    protected ItemType itemType = default;
    [Header("Dependencies")]
    [SerializeField] private Transform itemGrid = default;
    [SerializeField] private GameObject itemCellPrefab = default;
    protected CanvasObjectStrobe PlayerBalanceDisplay = default;
    protected CursorItem cursorItem = default;
    protected List<RectTransform> UIElements = default;
    protected PlayerBalance playerBalance = default;
    protected LevelDataReference LevelDataReference = default;
    protected GridSystem GridSystem = default;
    protected ResourceManager ResourceManager = default;
    private Dictionary<Item, StoreItemCell> storeItems = new Dictionary<Item, StoreItemCell>();

    protected Item selectedItem = null;

    public void SetupDependencies(LevelDataReference levelData, CursorItem cursorItem, List<RectTransform> UIElements, GridSystem gridSystem, PlayerBalance playerBalance, CanvasObjectStrobe playerBalanceDisplay, ResourceManager resourceManager)
    {
        this.LevelDataReference = levelData;
        this.cursorItem = cursorItem;
        this.UIElements = UIElements;
        this.GridSystem = gridSystem;
        this.playerBalance = playerBalance;
        this.PlayerBalanceDisplay = playerBalanceDisplay;
        this.ResourceManager = resourceManager;
    }

    public virtual void Initialize()
    {
        LevelData levelData = LevelDataReference.LevelData;
        foreach (Item item in levelData.Items)
        {
            if (item.Type.Equals(itemType))
            {
                this.AddItem(item);
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
        itemCell.Initialize(item, OnItemSelected);
        if (this.ResourceManager.hasLimitedSupply(item.ItemName))
        {
            this.ResourceManager.setupItemSupplyTracker(itemCell);
            storeItems.Add(item, itemCell);
        }
    }

    /// <summary>
    /// Triggered by items in the section.
    /// </summary>
    /// <param name="item">The item that was selected.</param>
    public virtual void OnItemSelected(Item item)
    {
        if (!this.HasSupply(item))
        {
            // this.PlayerBalanceDisplay.StrobeColor(2, Color.red);
            return;
        }
        cursorItem.Begin(item.Icon, OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
        selectedItem = item;
    }

    public virtual void OnItemSelectionCanceled()
    {
        cursorItem.Stop(OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
    }

    public void OnCursorItemClicked(PointerEventData eventData)
    {
        if (!this.HasSupply(this.selectedItem))
        {
            // this.PlayerBalanceDisplay.StrobeColor(2, Color.red);
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnItemSelectionCanceled();
        }
    }

    public bool HasSupply(Item item)
    {
        if (storeItems.ContainsKey(item) && playerBalance.Balance < storeItems[item].item.Price)
        {
            Debug.Log("You can't afford this!");
            OnItemSelectionCanceled();
            return false;
        }
        return true;
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
        if (!this.HasSupply(this.selectedItem))
        {
            // this.PlayerBalanceDisplay.StrobeColor(2, Color.red);
            return;
        }
    }

    public virtual bool IsPlacementValid(Vector3 mousePosition)
    {
        return false;
    }

    private void OnDisable()
    {
        cursorItem.Stop(OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
    }

    public bool IsCursorOverUI(PointerEventData eventData)
    {
        foreach (RectTransform UIElement in this.UIElements)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(UIElement, eventData.position))
            {
                return true;
            }
        }
        return false;
    }
}
