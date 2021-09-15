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
    private PopulationManager populationManager = default;

    public override void Initialize()
    {
        populationManager = FindObjectOfType<PopulationManager>();
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
        Debug.Log("Attempting to place food");
        base.OnCursorPointerUp(eventData);
        if (base.IsCursorOverUI(eventData) || eventData.button == PointerEventData.InputButton.Right ||
            GameManager.Instance.Balance < selectedItem.Price || base.ResourceManager.CheckRemainingResource(selectedItem) == 0)
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
        // if over ui don't do it
        if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.layer == 5)
            return;

        if (!base.GridSystem.IsFoodPlacementValid(mousePosition, base.selectedItem))
        {
            Debug.Log("Cannot place item that location");
            return;
        }
        GameManager.Instance.SubtractFromBalance(selectedItem.Price);
        base.ResourceManager.Placed(selectedItem, 1);
        base.HandleAudio();
        Vector3Int mouseGridPosition = base.GridSystem.WorldToCell(mousePosition);
        
        FoodSourceManager.placeFood(mouseGridPosition, GameManager.Instance.FoodSources[selectedItem.ID], this.selectedItem.buildTime);
    }
}
