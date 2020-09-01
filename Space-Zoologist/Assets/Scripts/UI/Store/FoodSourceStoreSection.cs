using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Store section for food source items.
/// </summary>
public class FoodSourceStoreSection : StoreSection
{
    [SerializeField] FoodSourceManager FoodSourceManager = default;

    public override void Initialize()
    {
        base.itemType = ItemType.Food;
        base.Initialize();
    }

    /// <summary>
    /// Handles the click release on the cursor item.
    /// </summary>
    public override void OnCursorPointerUp(PointerEventData eventData)
    {
        base.OnCursorPointerUp(eventData);
        if (base.IsCursorOverUI(eventData))
        {
            base.OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            if (!base.GridSystem.PlacementValidation.IsItemPlacementValid(mousePosition, base.selectedItem))
            {
                Debug.Log("Cannot place item that location");
                return;
            }
            Vector3Int mouseGridPosition = base.GridSystem.Grid.WorldToCell(mousePosition);
            base.GridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].ContainsFood = true;
            base.GridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].Food = FoodSourceManager.CreateFoodSource(selectedItem.ID, mousePosition);
            base.playerBalance.SubtractFromBalance(base.selectedItem.Price);
        }
    }
}
