
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// TODO figure out why can't click on items sometimes
/// <summary>
/// A section of items in the store. Subclass for specific behavior regarding what happens after an item is selected.
/// </summary>
public class StoreSection : MonoBehaviour, IStoreMenu
{
    public NeedType ItemType => itemType;

    protected NeedType itemType = default;
    [Header("Dependencies")]
    [SerializeField] private Transform itemGrid = default;
    [SerializeField] private GameObject itemCellPrefab = default;
    protected CursorItem cursorItem = default;
    protected List<RectTransform> UIElements = default;
    protected IntVariable playerBalance = default;
    protected LevelDataReference LevelDataReference = default;

    protected Item selectedItem = null;

    public void SetupDependencies(LevelDataReference levelData, CursorItem cursorItem, List<RectTransform> UIElements)
    {
        this.LevelDataReference = levelData;
        this.cursorItem = cursorItem;
        this.UIElements = UIElements;
    }

    public virtual void Initialize()
    {
        LevelData levelData = LevelDataReference.LevelData;
        this.playerBalance = levelData.StartingBalance;
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
        newItemCellGO.GetComponent<StoreItemCell>().Initialize(item, OnItemSelected);
    }

    /// <summary>
    /// Triggered by items in the section.
    /// </summary>
    /// <param name="item">The item that was selected.</param>
    public virtual void OnItemSelected(Item item)
    {
        if (!this.CanAfford(item))
        {
            Debug.Log("Selection cancelled");
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
        if (!this.CanAfford(this.selectedItem))
        {
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnItemSelectionCanceled();
        }
    }

    private bool CanAfford(Item item)
    {
        if (this.playerBalance == null)
        {
            Debug.Log("Null playerbalance reference");
            return false;
        }
        if (item.Price > this.playerBalance.RuntimeValue)
        {
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
