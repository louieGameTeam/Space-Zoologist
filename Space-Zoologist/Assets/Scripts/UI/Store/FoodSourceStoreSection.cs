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
                return;
            }
            Vector3Int mouseGridPosition = base.TileSystem.WorldToCell(mousePosition);
            base.GridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].ContainsItem = true;
            base.GridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].Item = FoodSourceManager.CreateFoodSource(selectedItem.ID, mousePosition);
            playerBalance.RuntimeValue -= selectedItem.Price;
        }
    }

    public override bool IsPlacementValid(Vector3 mousePosition)
    {
        if (mousePosition.x > 0 && mousePosition.y > 0
        && mousePosition.x < LevelDataReference.MapWidth && mousePosition.y < LevelDataReference.MapHeight)
        {
            Vector3Int mouseGridPosition = base.TileSystem.WorldToCell(mousePosition);
            if (base.GridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].ContainsItem)
            {
                base.GridSystem.CellGrid[mouseGridPosition.x, mouseGridPosition.y].Item.GetComponent<FloatingObjectStrobe>().StrobeColor(2, Color.red);
                return false;
            }
            TerrainTile tile = base.TileSystem.GetTerrainTileAtLocation(mouseGridPosition);
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
