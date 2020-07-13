using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Store section for tile items.
/// </summary>
public class TileStoreSection : StoreSection
{
    [Header("Tile Store Section")]
    [SerializeField] TilePlacementController tilePlacementController = default;

    private int startingBalance = 0;
    private bool isPlacing = false;
    private int numTilesPlaced = 0;

    private void Awake()
    {
        startingBalance = base.playerBalance.RuntimeValue;
    }

    /// <summary>
    /// Start tile placement preview.
    /// </summary>
    private void StartPlacing()
    {
        numTilesPlaced = 0;
        isPlacing = true;
        startingBalance = base.playerBalance.RuntimeValue;
        tilePlacementController.StartPreview(selectedItem.ID);
    }

    /// <summary>
    /// Stop tile placement preview and remove any changes.
    /// </summary>
    private void CancelPlacing()
    {
        isPlacing = false;
        tilePlacementController.RevertChanges();
        base.playerBalance.RuntimeValue = startingBalance;
    }

    /// <summary>
    /// Stop tile placement preview and finalize changes to the grid.
    /// </summary>
    private void FinishPlacing()
    {
        isPlacing = false;
        tilePlacementController.StopPreview();
        playerBalance.RuntimeValue = startingBalance - numTilesPlaced * selectedItem.Price;
    }

    /// <summary>
    /// Triggered by mouse down on the cursor item.
    /// </summary>
    public override void OnCursorPointerDown(PointerEventData eventData)
    {
        base.OnCursorPointerDown(eventData);
        if (eventData.button == PointerEventData.InputButton.Left && !isPlacing && !UIUtility.ins.IsCursorOverUI(eventData))
        {
            StartPlacing();
        }
    }

    /// <summary>
    /// Triggered by mouse up on the cursor item.
    /// </summary>
    public override void OnCursorPointerUp(PointerEventData eventData)
    {
        base.OnCursorPointerUp(eventData);
        if (eventData.button == PointerEventData.InputButton.Left && isPlacing)
        {
            FinishPlacing();
        }
    }

    /// <summary>
    /// Event when the item selection is cancelled.
    /// </summary>
    public override void OnItemSelectionCanceled()
    {
        Debug.Log("Tile placement cancelled");
        base.OnItemSelectionCanceled();
        CancelPlacing();
    }

    private void Update()
    {
         if (isPlacing)
         {
            numTilesPlaced = tilePlacementController.PlacedTileCount();
            playerBalance.RuntimeValue = startingBalance - numTilesPlaced * selectedItem.Price;
         }
    }
}
