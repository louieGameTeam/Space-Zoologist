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
    [SerializeField] TileSystem TileSystem = default;

    public override void Initialize()
    {
        base.itemType = NeedType.FoodSource;
        Debug.Assert(ReferenceUtil.ins != null);
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
            if (!IsPlacementValid(mousePosition))
            {
                Debug.Log("Cannot place item that location");
                //return;
            }
            FoodSourceManager.CreateFoodSource(selectedItem.ID, mousePosition);
            playerBalance.RuntimeValue -= selectedItem.Price;
        }
    }

    public override bool IsPlacementValid(Vector3 mousePosition)
    {
        if (mousePosition.x >= 0 && mousePosition.y >= 0
        && mousePosition.x <= LevelDataReference.MapWidth && mousePosition.y <= LevelDataReference.MapHeight)
        {
            Vector3Int mouseGridPosition = this.TileSystem.WorldToCell(mousePosition);
            TerrainTile tile = this.TileSystem.GetTerrainTileAtLocation(mouseGridPosition);
            foreach (TileType acceptablTerrain in ReferenceUtil.ins.FoodReference.AllSpecies[selectedItem.ID].AccessibleTerrain)
            {
                if (tile.type.Equals(acceptablTerrain))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
