using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItemCellsByCategory : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the toggle group used to select an item category")]
    private ItemCategoryToggleGroupPicker groupPicker;
    [SerializeField]
    [Tooltip("Reference to the transform to instantiate the cells under")]
    private Transform cellParent;
    [SerializeField]
    [Tooltip("Reference to the prefab to instantiate for each cell")]
    private StoreItemCell itemCellPrefab;
    #endregion

    #region Private Fields
    // List of cells currently displayed
    private List<StoreItemCell> existingCells = new List<StoreItemCell>();
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        SetupCells();
        groupPicker.OnToggleStateChanged.AddListener(SetupCells);
    }
    #endregion

    #region Public Methods
    public void SetupCells()
    {
        // Destroy all existing cells
        foreach (StoreItemCell cell in existingCells)
        {
            Destroy(cell.gameObject);
        }
        existingCells.Clear();

        // Select items with the given category
        IEnumerable<LevelData.ItemData> itemsWithCategory = GameManager.Instance.LevelData.itemQuantities
            .Where(item => item.itemObject.ItemID.Category == groupPicker.FirstValuePicked);

        // For each item, create a new cell, initialize it, and add it to the list
        foreach (LevelData.ItemData itemData in itemsWithCategory)
        {
            StoreItemCell cell = Instantiate(itemCellPrefab, cellParent);
            // Initialize the cell instance
            cell.Initialize(itemData.itemObject, null);
            cell.RemainingAmount = GameManager.Instance.m_resourceManager.CheckRemainingResource(itemData.itemObject);
            existingCells.Add(cell);
        }
    }
    #endregion
}
