using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Store section for tile items.
/// </summary>
/// Figure out how to handle case when more than one machine present after changes
public class TileStoreSection : StoreSection
{
    private EnclosureSystem EnclosureSystem = default;
    private TilePlacementController tilePlacementController = default;

    private float startingBalance;
    private int initialAmt;
    private bool isPlacing = false;
    private int numTilesPlaced = 0;
    private int prevTilesPlaced = 0;
    public override void Initialize()
    {
        EnclosureSystem = GameManager.Instance.m_enclosureSystem;
        tilePlacementController = GameManager.Instance.m_tilePlacementController;
        base.itemType = ItemType.Terrain;
        base.Initialize();
        //Debug.Assert(tilePlacementController != null);
    }

    /// <summary>
    /// Start tile placement preview.
    /// </summary>
    private void StartPlacing()
    {
        Debug.Log("Start placing");
        numTilesPlaced = 0;
        initialAmt = ResourceManager.CheckRemainingResource(selectedItem);
        isPlacing = true;
        startingBalance = GameManager.Instance.Balance;

        float[] contents = null;
        if(selectedItem is LiquidItem)
        {
            Vector3 liquidVector = ( (LiquidItem)selectedItem ).LiquidContents;
            contents = new float[] {liquidVector.x, liquidVector.y, liquidVector.z};
        }

        tilePlacementController.StartPreview(selectedItem.ID, liquidContents: contents);
    }

    /// <summary>
    /// Stop tile placement preview and remove any changes.
    /// </summary>
    private void CancelPlacing()
    {
        isPlacing = false;
        tilePlacementController.RevertChanges();
        GameManager.Instance.SetBalance(startingBalance);
    }

    /// <summary>
    /// Stop tile placement preview and finalize changes to the grid.
    /// </summary>
    private void FinishPlacing()
    {
        Debug.Log("Finish placing");
        isPlacing = false;
        foreach (Vector3Int pos in this.tilePlacementController.addedTiles)
        {
            if (tilePlacementController.godMode)
                GameManager.Instance.m_reservePartitionManager.UpdateAccessMapChangedAt(tilePlacementController.addedTiles.ToList<Vector3Int>());
            else
            {
                GridSystem.CreateUnitBuffer(new Vector2Int(pos.x, pos.y), this.selectedItem.buildTime, GridSystem.ConstructionCluster.ConstructionType.TILE);
            }
        }
        this.EnclosureSystem.UpdateEnclosedAreas();
        tilePlacementController.StopPreview();
        // NOTE: placing tiles no longer costs money, only requesting them does
        // GameManager.Instance.SetBalance(startingBalance - numTilesPlaced * selectedItem.Price);
        base.ResourceManager.Placed(selectedItem, numTilesPlaced);
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
        if (!base.CanBuy(selectedItem))
        {
            base.OnItemSelectionCanceled();
        }
    }

    /// <summary>
    /// Event when the item selection is canceled.
    /// </summary>
    public override void OnItemSelectionCanceled()
    {
        //Debug.Log("Tile placement canceled");
        base.OnItemSelectionCanceled();
        //CancelPlacing();
    }

    private void Update()
    {
        base.Update();
        if (isPlacing)
        {
            if (this.tilePlacementController.PlacementPaused)
            {
                return;
            }
            else
            {
                numTilesPlaced = tilePlacementController.PlacedTileCount();
                if (prevTilesPlaced != numTilesPlaced)
                {
                    base.HandleAudio();
                    prevTilesPlaced = numTilesPlaced;
                }
                //GameManager.Instance.SetBalance(startingBalance - numTilesPlaced * selectedItem.Price);
                if (GameManager.Instance.Balance < selectedItem.Price || initialAmt - numTilesPlaced == 0)
                {
                    FinishPlacing();
                    base.OnItemSelectionCanceled();
                }
            }
        }
    }
}
