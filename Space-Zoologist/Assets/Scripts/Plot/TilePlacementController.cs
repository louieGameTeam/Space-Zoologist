using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TilePlacementController : MonoBehaviour
{
    public enum PlacementResult { Placed, Restricted, AlreadyExisted }
    
    private TileDataController gridSystemReference;

    [SerializeField] public bool godMode = false;
    private bool isPreviewing;
    private bool isFirstTile;
    [SerializeField] public bool isErasing = false;

    private Vector3Int dragStartPosition = Vector3Int.zero;
    private Vector3Int lastMouseCellPosition = Vector3Int.zero;
    private Vector3Int currentMouseCellPosition = Vector3Int.zero;
    private Vector3Int lastPlacedTile;
    private List<GameTile> referencedTiles = new List<GameTile>();
    public GameTile[] gameTiles { get; private set; } = default;
    public HashSet<Vector3Int> addedTiles = new HashSet<Vector3Int>(); // All NEW tiles
    private HashSet<Vector3Int> triedToPlaceTiles = new HashSet<Vector3Int>(); // New tiles and same tile
    public void Initialize()
    {
        gridSystemReference = GameManager.Instance.m_gridSystem;
    }

    private void Update()
    {
        if (isPreviewing) // Update for preview
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.currentMouseCellPosition = gridSystemReference.WorldToCell(mouseWorldPosition);
            if (this.currentMouseCellPosition != this.lastMouseCellPosition || this.isFirstTile)
            {
                if (isErasing)
                {
                    this.EraseTile();
                    return;
                }
                UpdatePreviewPen();
                this.lastMouseCellPosition = this.currentMouseCellPosition;
            }
        }
    }

    public void LoadResources()
    {
        this.gameTiles = Resources.LoadAll("Tiles", typeof(GameTile)).Cast<GameTile>().ToArray();
    }

    /// <summary>
    /// Start tile placement preview.
    /// </summary>
    /// <param name="tileID">The ID of the tile to preview its placement.</param>
    public void StartPreview(string tileID, bool godMode = false, float[] liquidContents = null)
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.dragStartPosition = gridSystemReference.WorldToCell(mouseWorldPosition);
        if (!Enum.IsDefined(typeof(TileType), tileID))
        {
            throw new System.ArgumentException(tileID + " was not found in the TilePlacementController's tiles");
        }
        this.isPreviewing = true;
        foreach (GameTile tile in gameTiles)
        {
            if (tile.type == (TileType)Enum.Parse(typeof(TileType), tileID))
            {
                if(liquidContents != null)
                {
                    tile.defaultContents = liquidContents;
                }
                this.referencedTiles.Add(tile);
            }
        }
        this.isFirstTile = true;
    }
    public void StopPreview()
    {
        isPreviewing = false;
        lastMouseCellPosition = Vector3Int.zero;
        //temporarily removed because of revert bug
        //GameManager.Instance.m_gridSystem.ConfirmPlacement();

        // Set terrain modified flag
        GameManager.Instance.m_gridSystem.HasTerrainChanged = true;
        GameManager.Instance.m_gridSystem.ChangedTiles.UnionWith(addedTiles);

        // Clear all dics
        this.referencedTiles.Clear();
        this.addedTiles.Clear();
        this.triedToPlaceTiles.Clear();
    }
    // does not account for walls
    public void EraseTile()
    {
        foreach (GameTile tile in this.referencedTiles)
        {
            GameTile currentTile = gridSystemReference.GetGameTileAt(this.currentMouseCellPosition);
            if (currentTile != null)
            {
                gridSystemReference.RemoveTile(this.currentMouseCellPosition);
            }
        }
    }
    public int PlacedTileCount()
    {
        return addedTiles.Count();
    }

    public void RevertChanges() // Go through each change and revert back to original
    {
        gridSystemReference.Revert();
        addedTiles.Clear();
        triedToPlaceTiles.Clear();
        StopPreview();
    }

    private void UpdatePreviewPen()
    {
        if (gridSystemReference.GetGameTileAt(this.currentMouseCellPosition)?.type == TileType.Wall && !GameManager.Instance.LevelData.WallBreakable) {
            return;
        }

        if (isFirstTile)
        {
            PlaceTile(currentMouseCellPosition);
            return;
        }
        if (!TileDataController.FourNeighborTileLocations(currentMouseCellPosition).Contains(lastPlacedTile)) // Detect non-continuous points, and linearly interpolate to fill the gaps
        {
            if (currentMouseCellPosition.x == lastPlacedTile.x)// Handles divide by zero exception
            {
                foreach (int y in TileDataController.Range(lastPlacedTile.y, currentMouseCellPosition.y))
                {
                    Vector3Int location = new Vector3Int(lastPlacedTile.x, y, currentMouseCellPosition.z);
                    PlaceTile(location);
                }
            }
            else
            {
                float gradient = (currentMouseCellPosition.y - lastPlacedTile.y) / (currentMouseCellPosition.x - lastPlacedTile.x);
                foreach (float x in TileDataController.RangeFloat(TileDataController.IncreaseMagnitude(lastPlacedTile.x, -0.5f), currentMouseCellPosition.x))
                {
                    float interpolatedY = gradient * (x - lastPlacedTile.x);
                    int incrementY = TileDataController.RoundTowardsZeroInt(interpolatedY);
                    Vector3Int interpolateTileLocation = new Vector3Int(TileDataController.RoundTowardsZeroInt(x), lastPlacedTile.y + incrementY, lastPlacedTile.z);
                    PlaceTile(interpolateTileLocation);
                }
            }
        }
        PlaceTile(currentMouseCellPosition);
    }

    private int lastCornerX;
    private int lastCornerY;
    private void UpdatePreviewBlock()
    {
        if (isFirstTile)
        {
            PlaceTile(dragStartPosition, false);
            lastCornerX = dragStartPosition.x;
            lastCornerY = dragStartPosition.y;
        }
        HashSet<Vector3Int> tilesToRemove = new HashSet<Vector3Int>();
        HashSet<Vector3Int> tilesToAdd = new HashSet<Vector3Int>();
        HashSet<Vector3Int> supposedTiles = new HashSet<Vector3Int>();
        foreach (int x in TileDataController.Range(dragStartPosition.x, currentMouseCellPosition.x))
        {
            foreach (int y in TileDataController.Range(dragStartPosition.y, currentMouseCellPosition.y))
            {
                supposedTiles.Add(new Vector3Int(x, y, currentMouseCellPosition.z));
                tilesToRemove.Add(new Vector3Int(x, y, currentMouseCellPosition.z));
            }
        }
        tilesToRemove.ExceptWith(addedTiles); // Forcing removal of all tiles not in bound to avoid leftover tile not being removed due to lagging and tick skipping, possible optimization
        Vector3Int sweepLocation = Vector3Int.zero;
        sweepLocation.z = currentMouseCellPosition.z;
        bool isXShrinking = (currentMouseCellPosition.x - dragStartPosition.x) * (currentMouseCellPosition.x - lastCornerX) < 0;
        bool isYShrinking = (currentMouseCellPosition.y - dragStartPosition.y) * (currentMouseCellPosition.y - lastCornerY) < 0;
        if (currentMouseCellPosition.x != lastCornerX || !isXShrinking)
        {
            foreach (int x in TileDataController.Range(lastCornerX, currentMouseCellPosition.x))
            {
                foreach (int y in TileDataController.Range(dragStartPosition.y, currentMouseCellPosition.y))
                {
                    sweepLocation.x = x;
                    sweepLocation.y = y;
                    tilesToAdd.Add(sweepLocation);
                }
            }
        }
        if (currentMouseCellPosition.y != lastCornerY || !isYShrinking)
        {
            foreach (int x in TileDataController.Range(dragStartPosition.x, currentMouseCellPosition.x))
            {
                foreach (int y in TileDataController.Range(lastCornerY, currentMouseCellPosition.y))
                {
                    sweepLocation.x = x;
                    sweepLocation.y = y;
                    if (!tilesToRemove.Contains(sweepLocation) && !tilesToAdd.Contains(sweepLocation))
                    {
                        tilesToAdd.Add(sweepLocation);
                    }
                }
            }
        }
        lastCornerX = currentMouseCellPosition.x;
        lastCornerY = currentMouseCellPosition.y;
    }

    private bool IsPlacable(Vector3Int cellPosition)
    {
        if (godMode)
            return true;

        if (currentMouseCellPosition == dragStartPosition)
        {
            return GameManager.Instance.LevelData.WallBreakable || gridSystemReference.GetTileData(cellPosition).isTilePlaceable;
        }
        foreach (Vector3Int location in TileDataController.FourNeighborTileLocations(cellPosition))
        {
            if (triedToPlaceTiles.Contains(location))
            {
                return GameManager.Instance.LevelData.WallBreakable || gridSystemReference.GetTileData(location).isTilePlaceable;
            }
        }
        return false;
    }


    public PlacementResult PlaceTile(Vector3Int cellPosition, bool checkPlacable = true) //Main function controls tile placement
    {
        if (IsPlacable(cellPosition) || !checkPlacable)
        {
            // Check availability
            foreach (GameTile tile in referencedTiles)
            {
                // If animal/food at location
                if (!IsPositionFree(cellPosition))
                {
                    return PlacementResult.Restricted;
                }
                // If same tile
                if (gridSystemReference.GetGameTileAt(cellPosition) == tile)
                {
                    this.triedToPlaceTiles.Add(cellPosition);
                    return PlacementResult.AlreadyExisted;
                }
            }
            foreach (GameTile tile in referencedTiles)
            {
                gridSystemReference.SetTile(cellPosition, tile, godMode);
            }
            this.triedToPlaceTiles.Add(cellPosition);
            this.addedTiles.Add(cellPosition);
            
            return PlacementResult.Placed;
        }
        return PlacementResult.Restricted;
    }

    private HashSet<Vector3Int> neighborTiles = new HashSet<Vector3Int>();
    private void GetNeighborCellLocations(Vector3Int cellLocation, GameTile tile, Tilemap targetTilemap)
    {
        foreach (Vector3Int tileToCheck in TileDataController.FourNeighborTileLocations(cellLocation))
        {
            if (!neighborTiles.Contains(tileToCheck) && targetTilemap.GetTile(tileToCheck) == tile)
            {
                neighborTiles.Add(tileToCheck);
                GetNeighborCellLocations(tileToCheck, tile, targetTilemap);
            }
        }
    }
    private bool IsPositionFree(Vector3Int cellLocation)
    {
        if (godMode)
        {
            return true;
        }
        if (!gridSystemReference.IsWithinGridBounds(cellLocation))
        {
            return false;
        }

        TileData tileData = gridSystemReference.GetTileData(cellLocation);
        if (tileData.Food)
        {
            foreach(GameTile tile in referencedTiles)
            {
                if(tile.type == TileType.Liquid)
                    return false;
            }
        }

        if (gridSystemReference.IsConstructing(cellLocation.x, cellLocation.y))
        {
            return false;
        }
        return true;
    }
}
