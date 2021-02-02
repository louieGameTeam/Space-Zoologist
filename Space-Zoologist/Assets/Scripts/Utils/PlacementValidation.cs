using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementValidation : MonoBehaviour
{
    private GridSystem GridSystem = default;
    private TileSystem TileSystem = default;
    private LevelDataReference LevelDataReference = default;
    private FoodReferenceData FoodReferenceData = default;

    public void Initialize(GridSystem gridSystem, TileSystem tileSystem, LevelDataReference levelData, FoodReferenceData foodReferenceData)
    {
        this.GridSystem = gridSystem;
        this.TileSystem = tileSystem;
        this.LevelDataReference = levelData;
        this.FoodReferenceData = foodReferenceData;
    }

    public bool IsPodPlacementValid(Vector3 mousePosition, AnimalSpecies species)
    {
        Vector3Int gridPosition = this.TileSystem.WorldToCell(mousePosition);
        if (!this.IsInMapBounds(gridPosition))
        {
            return false;
        }
        return this.CheckSurroundingTerrain(gridPosition, species);
    }

    private bool CheckSurroundingTerrain(Vector3Int cellPosition, AnimalSpecies selectedSpecies)
    {
        int count = 0;
        for (int i=-1; i<=1; i++)
        {
            for (int j=-1; j<=1; j++)
            {
                Vector3Int surroundingGridPosition = new Vector3Int(cellPosition.x + i, cellPosition.y + j, 0);
                GameTile tile = this.TileSystem.GetGameTileAt(surroundingGridPosition);
                foreach (TileType acceptablTerrain in selectedSpecies.AccessibleTerrain)
                {
                    if (tile.type.Equals(acceptablTerrain))
                    {
                        count++;
                        continue;
                    }
                }
            }
        }
        return count == 9;
    }

    public bool IsItemPlacementValid(Vector3 mousePosition, Item selectedItem)
    {
        Vector3Int gridPosition = this.GridSystem.Grid.WorldToCell(mousePosition);
        if (!this.IsInMapBounds(gridPosition))
        {
            return false;
        }
        GridSystem.CellData cellData = this.GridSystem.CellGrid[gridPosition.x, gridPosition.y];
        // Prevent placing on items already there.
        if (cellData.ContainsMachine)
        {
            cellData.Machine.GetComponent<FloatingObjectStrobe>().StrobeColor(2, Color.red);
            return false;
        }
        if (cellData.ContainsFood)
        {
            cellData.Food.GetComponent<FloatingObjectStrobe>().StrobeColor(2, Color.red);
            return false;
        }
        // Prevent placing on walls
        GameTile selectedTile = this.TileSystem.GetGameTileAt(gridPosition);
        if (selectedTile.type.Equals(TileType.Wall))
        {
            return false;
        }
        // if liquidmachine and on liquid
        if (selectedItem.ID.Equals("LiquidMachine"))
        {
            bool isCorrectTileType = selectedTile.type.Equals(TileType.Liquid);
            return isCorrectTileType;
        }
        // or atmosphere machine not on liquid
        else if (selectedItem.ID.Equals("AtmosphereMachine"))
        {
            bool isCorrectTileType = !selectedTile.type.Equals(TileType.Liquid);
            return isCorrectTileType;
        }
        // else food item so make sure terrain is good
        else
        {
            foreach (TileType acceptablTerrain in this.FoodReferenceData.FoodSources[selectedItem.ID].AccessibleTerrain)
            {
                if (selectedTile.type.Equals(acceptablTerrain))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool IsInMapBounds(Vector3Int mousePosition)
    {
        return mousePosition.x >= 1 && mousePosition.y >= 1 && mousePosition.x < LevelDataReference.MapWidth - 1 && mousePosition.y < LevelDataReference.MapHeight - 1;
    }
}
