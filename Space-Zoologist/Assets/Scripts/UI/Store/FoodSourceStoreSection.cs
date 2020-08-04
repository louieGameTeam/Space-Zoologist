using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Store section for food source items.
/// </summary>
public class FoodSourceStoreSection : StoreSection, IValidatePlacement
{
    [SerializeField] FoodSourceManager FoodSourceManager = default;
    [SerializeField] TileSystem TileSystem = default;

    private void Awake()
    {
        base.itemType = NeedType.FoodSource;
    }

    protected override void Start()
    {
        Debug.Assert(ReferenceUtil.ins != null);
        base.Start();
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

    public bool IsPlacementValid(Vector3 mousePosition)
    {
        if (mousePosition.x >= 0 && mousePosition.y >= 0
        && mousePosition.x <= TilemapUtil.ins.MaxWidth && mousePosition.y <= TilemapUtil.ins.MaxHeight)
        {
            Vector3Int mouseGridPosition = TilemapUtil.ins.WorldToCell(mousePosition);
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
