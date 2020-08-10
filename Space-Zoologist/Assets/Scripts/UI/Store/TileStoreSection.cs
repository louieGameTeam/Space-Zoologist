using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Store section for tile items.
/// </summary>
public class TileStoreSection : StoreSection
{
    [SerializeField] private TilePlacementController tilePlacementController = default;
    [SerializeField] private NeedSystemManager NeedSystemManager = default;
    [Header("Only needed if tutorial level")]
    [SerializeField] private TutorialTesting tutorial = default;

    private IntVariable startingBalance;
    private bool isPlacing = false;
    private int numTilesPlaced = 0;

    public override void Initialize()
    {
        base.itemType = NeedType.Terrain;
        base.Initialize();
        startingBalance = base.playerBalance;
        Debug.Assert(tilePlacementController != null);
        Debug.Assert(startingBalance != null);
    }

    /// <summary>
    /// Start tile placement preview.
    /// </summary>
    private void StartPlacing()
    {
        numTilesPlaced = 0;
        isPlacing = true;
        startingBalance = base.playerBalance;
        tilePlacementController.StartPreview(selectedItem.ID);
    }

    /// <summary>
    /// Stop tile placement preview and remove any changes.
    /// </summary>
    private void CancelPlacing()
    {
        isPlacing = false;
        tilePlacementController.RevertChanges();
        base.playerBalance = startingBalance;
    }

    /// <summary>
    /// Stop tile placement preview and finalize changes to the grid.
    /// </summary>
    private void FinishPlacing()
    {
        isPlacing = false;
        tilePlacementController.StopPreview();
        playerBalance.RuntimeValue = startingBalance.RuntimeValue - numTilesPlaced * selectedItem.Price;
        if (tutorial != null)
        {
            tutorial.TriggerDialogueOnce();
        }
    }

    /// <summary>
    /// Triggered by mouse down on the cursor item.
    /// </summary>
    public override void OnCursorPointerDown(PointerEventData eventData)
    {
        base.OnCursorPointerDown(eventData);
        if (base.IsCursorOverUI(eventData))
        {
            base.OnItemSelectionCanceled();
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left && !isPlacing)
        {
            this.StartPlacing();
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
        //Debug.Log("Tile placement cancelled");
        base.OnItemSelectionCanceled();
        CancelPlacing();
    }

    private void Update()
    {
        if (isPlacing)
        {
            if (this.tilePlacementController.PlacementPaused)
            {
                return;
            }
            else
            {
                numTilesPlaced = tilePlacementController.PlacedTileCount();
                playerBalance.RuntimeValue = startingBalance.RuntimeValue - numTilesPlaced * selectedItem.Price;
            }
        }
    }
}
