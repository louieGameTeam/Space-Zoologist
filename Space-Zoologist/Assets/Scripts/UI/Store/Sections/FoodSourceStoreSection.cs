using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Store section for food source items.
/// </summary>
public class FoodSourceStoreSection : StoreSection
{
    public override void Initialize()
    {
        base.itemType = ItemType.Food;
        base.Initialize();
    }

    public void ManuallyPlaceItem(Item item, Vector3Int mouseGridPosition)
    {
        selectedItem = item;
        PlaceFood(mouseGridPosition);
    }

    /// <summary>
    /// Handles the click release on the cursor item.
    /// </summary>
    public override void OnCursorPointerUp(PointerEventData eventData)
    {
        base.OnCursorPointerUp(eventData);
        if (!UIBlockerSettings.OperationIsAvailable("Build") || 
            eventData.button == PointerEventData.InputButton.Right ||
            base.ResourceManager.CheckRemainingResource(selectedItem) == 0)
        {
            Debug.Log("Cannot place item that location");
            base.OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            PlaceFood(mousePosition);
        }
        if (!base.CanBuy(selectedItem))
        {
            base.OnItemSelectionCanceled();
        }
    }

    public void PlaceFood(Vector3 mousePosition)
    {
        // Exit if build is unavailable
        if (!UIBlockerSettings.OperationIsAvailable("Build"))
            return;

        if (!base.GridSystem.IsFoodPlacementValid(mousePosition, base.selectedItem))
        {
            Debug.Log("Cannot place item that location");
            return;
        }
        // NOTE: placing objects no longer costs money, only requesting them does
        // GameManager.Instance.SubtractFromBalance(selectedItem.Price);
        base.ResourceManager.Placed(selectedItem, 1);
        base.HandleAudio();
        Vector3Int mouseGridPosition = base.GridSystem.WorldToCell(mousePosition);

        // Try to get the game manager instance
        GameManager instance = GameManager.Instance;

        // If instance exists use it to access the food source manager
        if(instance)
        {
            FoodSourceSpecies species = instance.FoodSources [selectedItem.ID];
            Vector3 FoodLocation = instance.m_tileDataController.CellToWorld (mouseGridPosition);
            FoodLocation.x += species.Size.x / 2f;
            FoodLocation.y += species.Size.y / 2f;
            instance.m_foodSourceManager.CreateFoodSource (species, FoodLocation, this.selectedItem.buildTime);
        }
        else
        {
            Debug.Log(GetType().ToString() + ": cannot place food because no game manager instance was found");
        }

    }
}
