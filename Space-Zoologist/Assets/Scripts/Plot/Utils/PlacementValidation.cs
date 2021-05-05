using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementValidation : MonoBehaviour
{
    private GridSystem GridSystem = default;
    private TileSystem TileSystem = default;
    private LevelDataReference LevelDataReference = default;
    private ReferenceData ReferenceData = default;
    private GridOverlay gridOverlay = default;

    private void Start()
    {
        gridOverlay = this.gameObject.GetComponent<GridOverlay>();
    }

    public void Initialize(GridSystem gridSystem, TileSystem tileSystem, LevelDataReference levelData, ReferenceData ReferenceData)
    {
        this.GridSystem = gridSystem;
        this.TileSystem = tileSystem;
        this.LevelDataReference = levelData;
        this.ReferenceData = ReferenceData;
    }

    public bool IsPodPlacementValid(Vector3 mousePosition, AnimalSpecies species)
    {
        Vector3Int gridPosition = this.TileSystem.WorldToCell(mousePosition);
        return this.CheckSurroundingTerrain(gridPosition, species);
    }

    public bool IsFoodPlacementValid(Vector3 mousePosition, Item selectedItem)
    {
        FoodSourceSpecies species = this.ReferenceData.FoodSources[selectedItem.ID];
        Vector3Int gridPosition = this.GridSystem.Grid.WorldToCell(mousePosition);
        return CheckSurroudingTiles(gridPosition, species);
    }

    public void updateVisualPlacement(Vector3Int gridPosition, Item selectedItem)
    {
        Debug.Log("Item selected " + selectedItem.ID);
        if (this.ReferenceData.FoodSources.ContainsKey(selectedItem.ID))
        {
            FoodSourceSpecies species = this.ReferenceData.FoodSources[selectedItem.ID];
            CheckSurroudingTiles(gridPosition, species);
        }
        else if (this.ReferenceData.Species.ContainsKey(selectedItem.ID))
        {
            AnimalSpecies species = this.ReferenceData.Species[selectedItem.ID];
            CheckSurroundingTerrain(gridPosition, species);
        }
        else
        {
            // TODO figure out how to determine if tile is placable
            // gridOverlay.HighlightTile(gridPosition, Color.green);
        }
    }

    private bool CheckSurroundingTerrain(Vector3Int cellPosition, AnimalSpecies selectedSpecies)
    {
        Vector3Int pos;
        GameTile tile;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                pos = cellPosition;
                pos.x += x;
                pos.y += y;
                tile = this.TileSystem.GetGameTileAt(pos);
                if (tile == null)
                {
                    return false;
                }
                bool isTerrainAcceptable = IsTerrainAcceptable(tile.type, selectedSpecies.AccessibleTerrain);
                if (isTerrainAcceptable)
                {
                    gridOverlay.HighlightTile(pos, Color.green);
                }
                else
                {
                    gridOverlay.HighlightTile(pos, Color.red);
                }
            }
        }
        tile = this.TileSystem.GetGameTileAt(cellPosition);
        return IsTerrainAcceptable(tile.type, selectedSpecies.AccessibleTerrain);
    }

    private bool IsTerrainAcceptable(TileType tileType, List<TileType> accessibleTerrain)
    {
        foreach (TileType acceptablTerrain in accessibleTerrain)
        {
            if (tileType.Equals(acceptablTerrain))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckSurroudingTiles(Vector3Int cellPosition, FoodSourceSpecies species)
    {
        int radius = species.Size / 2;
        Vector3Int pos;
        bool isValid = true;
        int offset = 0;
        // Size is even, offset by 1
        if (species.Size % 2 == 0)
        {
            offset = 1;
        }
        // Check if the whole object is in bounds
        for (int x = (-1 - offset) * (radius - offset); x <= radius; x++)
        {
            for (int y = (-1 - offset) * (radius - offset) ; y <= radius ; y++)
            {
                pos = cellPosition;
                pos.x += x;
                pos.y += y;
                if (!IsFoodPlacementValid(pos, species))
                {
                    isValid = false;
                    gridOverlay.HighlightTile(pos, Color.red);
                }
                else
                {
                    gridOverlay.HighlightTile(pos, Color.green);
                }
            }
        }
        return isValid;
    }

    public bool IsOnWall(Vector3Int pos)
    {
        // Prevent placing on walls
        GameTile selectedTile = this.TileSystem.GetGameTileAt(pos);
        if (selectedTile.type.Equals(TileType.Wall))
        {
            return true;
        }
        return false;
    }

    // helper function that checks the validity at one tile
    private bool IsFoodPlacementValid(Vector3Int pos, FoodSourceSpecies species)
    {
        if (!this.GridSystem.IsWithinGridBounds(pos))
        {
            return false;
        }

        // Prevent placing on items already there.
        GridSystem.CellData cellData = this.GridSystem.CellGrid[pos.x, pos.y];
        if (cellData.ContainsFood)
        {
            return false;
        }
        if (IsOnWall(pos)) return false;
        GameTile selectedTile = this.TileSystem.GetGameTileAt(pos);

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
    public FoodSourceSpecies GetFoodSpecies(Item item)
    {
        return this.ReferenceData.FoodSources[item.ID];
    }

    public AnimalSpecies GetAnimalSpecies(Item item)
    {
        return this.ReferenceData.Species[item.ID];
    }
}
