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

    public void ManuallyPlaceItem(Item item, Vector3Int mouseGridPosition)
    {
        selectedItem = item;
        placeFood(mouseGridPosition);
    }

    /// <summary>
    /// Handles the click release on the cursor item.
    /// </summary>
    public override void OnCursorPointerUp(PointerEventData eventData)
    {
        base.OnCursorPointerUp(eventData);
        if (base.IsCursorOverUI(eventData) || eventData.button == PointerEventData.InputButton.Right || base.playerBalance.Balance < selectedItem.Price || base.ResourceManager.CheckRemainingResource(selectedItem) == 0)
        {
            base.OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            if (!base.GridSystem.PlacementValidation.IsFoodPlacementValid(worldPosition, base.selectedItem))
            {
                Debug.Log("Cannot place item that location");
                return;
            }
            base.playerBalance.SubtractFromBalance(selectedItem.Price);
            base.ResourceManager.Placed(selectedItem, 1);
            Vector3Int mouseGridPosition = base.GridSystem.Grid.WorldToCell(worldPosition);
            placeFood(mouseGridPosition);
        }
    }

    // TODO setup manual food placement while Virgil gets saving and loading updated.

    public void placeFood(Vector3Int mouseGridPosition)
    {
        FoodSourceSpecies species = base.GridSystem.PlacementValidation.GetFoodSpecies(selectedItem);
        Vector3Int Temp = mouseGridPosition;
        Temp.x += 1;
        Temp.y += 1;
        if (species.Size % 2 == 1)
        {
            //size is odd: center it
            Vector3 FoodLocation = base.GridSystem.Grid.CellToWorld(mouseGridPosition); //equivalent since cell and world is 1:1, but in Vector3
            FoodLocation += Temp;
            FoodLocation /= 2f;

            GameObject Food = FoodSourceManager.CreateFoodSource(selectedItem.ID, FoodLocation);

            GridSystem.AddFood(mouseGridPosition, species.Size, Food);
        }
        else
        {
            //size is even: place it at cross-center (position of tile)
            Vector3 FoodLocation = base.GridSystem.Grid.CellToWorld(Temp); //equivalent since cell and world is 1:1, but in Vector3
            GameObject Food = FoodSourceManager.CreateFoodSource(selectedItem.ID, FoodLocation);

            GridSystem.AddFood(mouseGridPosition, species.Size, Food);
        }
    }
}
