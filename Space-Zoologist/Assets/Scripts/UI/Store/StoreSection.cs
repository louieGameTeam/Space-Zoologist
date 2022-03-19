
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
    public ItemType ItemType => itemType;
    public Item SelectedItem => selectedItem;

    // Can't display in editor anymore because it is in a prefab
    /*[SerializeField] */private GraphicRaycaster raycaster;

    protected ItemType itemType = default;
    [Header("Dependencies")]
    [SerializeField] private Transform itemGrid = default;
    [SerializeField] private GameObject itemCellPrefab = default;
    protected CursorItem cursorItem = default;
    protected List<RectTransform> UIElements = default;
    protected TileDataController GridSystem = default;
    protected ResourceManager ResourceManager = default;
    private Dictionary<Item, StoreItemCell> storeItems = new Dictionary<Item, StoreItemCell>();
    protected Item selectedItem = null;
    private Vector3Int previousLocation = default;
    protected int currentAudioIndex = 0;

    public void SetupDependencies(CursorItem cursorItem, List<RectTransform> UIElements, ResourceManager resourceManager)
    {
        this.cursorItem = cursorItem;
        this.UIElements = UIElements;
        this.GridSystem = GameManager.Instance.m_tileDataController;
        this.ResourceManager = resourceManager;
    }
    public Item GetItemByID(string id)
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

    public void Update()
    {
        if (cursorItem.IsOn)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(cursorItem.transform.position);
            if (!GridSystem.IsWithinGridBounds(mousePosition)) return;

            Vector3Int gridLocation = GridSystem.WorldToCell(mousePosition);

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
        foreach (LevelData.ItemData data in levelData.ItemQuantities)
        {
            Item item = data.itemObject;
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
        if (this.ResourceManager.hasLimitedSupply(item.ItemName))
        {
            this.ResourceManager.setupItemSupplyTracker(itemCell);
            if (!storeItems.ContainsKey(item))
                storeItems.Add(item, itemCell);
        }
    }

    public void AddItemQuantity(Item item, int count = 1)
    {
        if (!storeItems.ContainsKey(item))
        {
            AddItem(item);
        }
        else
        {
            this.ResourceManager.AddItem(item.ItemName, count);
            //storeItems[item].RemainingAmount += count;
        }
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
        cursorItem.Begin(item.Icon, OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
        selectedItem = item;
        //Reset inspector selection
        GameManager.Instance.m_inspector.ResetSelection();
    }

    public virtual void OnItemSelectionCanceled()
    {
        cursorItem.Stop(OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
        selectedItem = null;
        AudioManager.instance?.PlayOneShot(SFXType.Cancel);
        GridSystem.ClearHighlights();
    }

    public virtual void OnItemUnavailable()
    {
        AudioManager.instance.PlayOneShot(SFXType.Unavailable);
    }

    public void OnCursorItemClicked(PointerEventData eventData)
    {
        if (!this.CanBuy(this.selectedItem))
        {
            OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnItemSelectionCanceled();
        }
    }

    public bool CanBuy(Item item)
    {
        if (storeItems.ContainsKey(item) && ResourceManager.CheckRemainingResource(item) == 0)
        {
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
        if (!this.CanBuy(this.selectedItem))
        {
            OnItemSelectionCanceled();
            return;
        }
    }

    private void OnDisable()
    {
        cursorItem.Stop(OnCursorItemClicked, OnCursorPointerDown, OnCursorPointerUp);
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
