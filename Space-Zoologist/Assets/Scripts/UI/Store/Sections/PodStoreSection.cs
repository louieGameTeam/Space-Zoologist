using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PodStoreSection : StoreSection
{
    [Header("Handled by Prefab")]
    [SerializeField] Transform PodItemContainer = default;
    private PopulationManager populationManager = default;

    AnimalSpecies selectedSpecies = null;

    public override void Initialize()
    {
        populationManager = GameManager.Instance.m_populationManager;
        base.itemType = ItemRegistry.Category.Species;
        base.Initialize();
    }

    public override void HandleCursor () {
        base.HandleCursor ();

        if (!UIBlockerSettings.OperationIsAvailable ("Build")) {
            return;
        }

        if (Input.GetMouseButtonUp(0)) 
        {
            Vector2 position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            selectedSpecies = GameManager.Instance.AnimalSpecies [selectedItem.ID];
            if (!this.GridSystem.IsPodPlacementValid (position, selectedSpecies)) {
                return;
            }
            if (base.ResourceManager.CheckRemainingResource (selectedSpecies) <= 0 && !tilePlacementController.godMode) {
                base.OnItemSelectionCanceled ();
                return;
            }
            base.HandleAudio ();
            populationManager.SpawnAnimal (selectedSpecies, position);
            GridSystem.UpdateAnimalCellGrid ();
            base.ResourceManager.Placed (selectedSpecies, 1);
        }
    }

    public override void OnCursorPointerUp(PointerEventData eventData)
    {/*
        // If in CursorItem mode and the cursor is clicked while over the menu
        if (!UIBlockerSettings.OperationIsAvailable("Build"))
        {
            Debug.Log("Clicked over UI");
            base.OnItemSelectionCanceled();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(eventData.position);
            selectedSpecies = GameManager.Instance.AnimalSpecies[selectedItem.ID];
            if (!this.GridSystem.IsPodPlacementValid(position, selectedSpecies))
            {
                Debug.Log("Can't place species there");
                return;
            }
            if (base.ResourceManager.CheckRemainingResource(selectedSpecies) <= 0 && !tilePlacementController.godMode)
            {
                base.OnItemSelectionCanceled();
                return;
            }
            base.HandleAudio();
            populationManager.SpawnAnimal(selectedSpecies, position);
            GridSystem.UpdateAnimalCellGrid();
            base.ResourceManager.Placed(selectedSpecies, 1);
        }
        if (!base.CanBuy(selectedItem))
        {
            base.OnItemSelectionCanceled();
        }*/
    }
}