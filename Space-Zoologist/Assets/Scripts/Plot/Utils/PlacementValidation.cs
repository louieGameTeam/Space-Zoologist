using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementValidation : MonoBehaviour
{
    private GridSystem GridSystem = default;
    private TileSystem TileSystem = default;
    private LevelDataReference LevelDataReference = default;
    private SpeciesReferenceData SpeciesReferenceData = default;

    public void Initialize(GridSystem gridSystem, TileSystem tileSystem, LevelDataReference levelData, SpeciesReferenceData speciesReferenceData)
    {
        this.GridSystem = gridSystem;
        this.TileSystem = tileSystem;
        this.LevelDataReference = levelData;
        this.SpeciesReferenceData = speciesReferenceData;
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
        return false;
    }

    public bool IsFoodPlacementValid(Vector3 mousePosition, Item selectedItem)
    {
        FoodSourceSpecies species = this.SpeciesReferenceData.FoodSources[selectedItem.ID];
        Vector3Int gridPosition = this.GridSystem.Grid.WorldToCell(mousePosition);

        // size 1 -> rad 0, size 3 -> rad 1 ...
        int radius = species.Size / 2;
        Vector3Int pos;

        if (species.Size % 2 == 1)
        {
            //size is odd: center it

            // Check if the whole object is in bounds
            for (int x = -1 * radius; x <= radius; x++)
            {
                for (int y = -1 * radius; y <= radius; y++)
                {
                    pos = gridPosition;
                    pos.x += x;
                    pos.y += y;
                    if (!IsFoodPlacementValid(pos, species)) { return false; }
                }
            }

        }
        else
        {
            //size is even: place it at cross-center

            // Check if the whole object is in bounds
            for (int x = -1 * (radius - 1); x <= radius; x++)
            {
                for (int y = -1 * (radius - 1); y <= radius; y++)
                {
                    pos = gridPosition;
                    pos.x += x;
                    pos.y += y;

                    if (!IsFoodPlacementValid(pos, species)) { return false; }
                }
            }
        }
        return true;
    }

    // helper function that checks the validity at one tile
    private bool IsFoodPlacementValid(Vector3Int pos, FoodSourceSpecies species)
    {
        if (!this.IsInMapBounds(pos))
        {
            return false;
        }

        // Prevent placing on items already there.
        GridSystem.CellData cellData = this.GridSystem.CellGrid[pos.x, pos.y];
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
        GameTile selectedTile = this.TileSystem.GetGameTileAt(pos);
        if (selectedTile.type.Equals(TileType.Wall))
        {
            return false;
        }

        // Make sure the tile is acceptable
        foreach (TileType acceptablTerrain in species.AccessibleTerrain)
        {
            if (selectedTile.type.Equals(acceptablTerrain))
            {
                return true;
            }
        }
        return false;
    }
    public FoodSourceSpecies GetFoodSpecies(string itemID)
    {
        return this.SpeciesReferenceData.FoodSources[itemID];
    }

    public bool IsInMapBounds(Vector3Int mousePosition)
    {
        return mousePosition.x >= 1 && mousePosition.y >= 1 && mousePosition.x < LevelDataReference.MapWidth - 1 && mousePosition.y < LevelDataReference.MapHeight - 1;
    }
}
