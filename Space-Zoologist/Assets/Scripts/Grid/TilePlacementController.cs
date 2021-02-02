using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.IO;
public class TilePlacementController : MonoBehaviour
{
    private enum PlacementResult { Placed, Restricted, AlreadyExisted }
    public bool isBlockMode { get; set; } = false;
    public Vector3Int mouseCellPosition { get { return currentMouseCellPosition; } }
    public bool PlacementPaused { get; private set; }
    [SerializeField] private Camera currentCamera = default;
    private bool isPreviewing { get; set; } = false;
    private bool godMode = false;
    private Vector3Int dragStartPosition = Vector3Int.zero;
    private Vector3Int lastMouseCellPosition = Vector3Int.zero;
    private Vector3Int currentMouseCellPosition = Vector3Int.zero;
    private Grid grid;
    private Vector3Int lastPlacedTile;
    private List<GameTile> referencedTiles = new List<GameTile>();
    private bool isFirstTile;
    public Tilemap[] allTilemaps { get { return tilemaps; } }
    [SerializeField] private Tilemap[] tilemaps = default; // Order according to GridUtils.TileLayer
    public GameTile[] gameTiles { get; private set; } = default;
    private Dictionary<Vector3Int, List<GameTile>> addedTiles = new Dictionary<Vector3Int, List<GameTile>>(); // All NEW tiles
    private Dictionary<Vector3Int, Dictionary<Color, Tilemap>> removedTileColors = new Dictionary<Vector3Int, Dictionary<Color, Tilemap>>();
    private HashSet<Vector3Int> triedToPlaceTiles = new HashSet<Vector3Int>(); // New tiles and same tile
    private HashSet<Vector3Int> neighborTiles = new HashSet<Vector3Int>();
    private Dictionary<GameTile, List<Tilemap>> colorLinkedTiles = new Dictionary<GameTile, List<Tilemap>>();
    private Dictionary<Tilemap, TileLayerManager> tilemapsToTileLayerManagers = new Dictionary<Tilemap, TileLayerManager>();
    private int lastCornerX;
    private int lastCornerY;
    [SerializeField] private TileSystem TileSystem = default;
    [SerializeField] private GridSystem GridSystem = default;
    private void Awake()
    {
        this.gameTiles = Resources.LoadAll("Tiles",typeof(GameTile)).Cast<GameTile>().ToArray(); // Load tiles form resources
        foreach (GameTile terrainTile in this.gameTiles)// Construct list of tiles and their corresponding layers
        {
            terrainTile.targetTilemap = tilemaps[(int)terrainTile.targetLayer];
            terrainTile.constraintTilemaps.Clear();
            terrainTile.replacementTilemaps.Clear();
            foreach (GridUtils.TileLayer layer in terrainTile.constraintLayers)
            {
                terrainTile.constraintTilemaps.Add(tilemaps[(int)layer]);
            }
            foreach (GridUtils.TileLayer layer in terrainTile.replacementLayers)
            {
                terrainTile.replacementTilemaps.Add(tilemaps[(int)layer]);
            }
        }
        grid = GetComponent<Grid>();
        foreach (Tilemap tilemap in tilemaps)// Construct list of affected colors
        {
            this.tilemapsToTileLayerManagers.Add(tilemap, tilemap.GetComponent<TileLayerManager>());
            List<Vector3Int> colorInitializeTiles = new List<Vector3Int>();
/*            if (tilemap.TryGetComponent(out TileColorManager tileColorManager))
            {
                foreach (GameTile tile in tileColorManager.linkedTiles)
                {
                    if (!colorLinkedTiles.ContainsKey(tile))
                    {
                        colorLinkedTiles.Add(tile, new List<Tilemap>());
                    }
                    colorLinkedTiles[tile].Add(tilemap);
                }
            }*/
            referencedTiles = this.gameTiles.ToList();
            RenderColorOfColorLinkedTiles(colorInitializeTiles);
            referencedTiles.Clear();
        }
        this.gameObject.GetComponent<GridIO>().Initialize();
        this.gameObject.GetComponent<GridIO>().LoadGrid();
    }
    private void Update()
    {
        if (isPreviewing) // Update for preview
        {
            Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
            this.currentMouseCellPosition = grid.WorldToCell(mouseWorldPosition);
            this.PlacementPaused = false;
            if (this.currentMouseCellPosition != this.lastMouseCellPosition || this.isFirstTile)
            {
                if (isBlockMode)
                {
                    UpdatePreviewBlock();
                }
                else
                {
                    UpdatePreviewPen();
                }
                this.lastMouseCellPosition = this.currentMouseCellPosition;
            }
        }
    }
    /// <summary>
    /// Start tile placement preview.
    /// </summary>
    /// <param name="tileID">The ID of the tile to preview its placement.</param>
    public void StartPreview(string tileID, bool godMode = false)
    {
        this.godMode = godMode;
        Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        this.dragStartPosition = this.grid.WorldToCell(mouseWorldPosition);
        if (!Enum.IsDefined(typeof(TileType), tileID))
        {
            throw new System.ArgumentException(tileID + " was not found in the TilePlacementController's tiles");
        }
        this.isPreviewing = true;
        foreach (GameTile tile in gameTiles)
        {
            if (tile.type == (TileType)Enum.Parse(typeof(TileType), tileID))
            {
                this.referencedTiles.Add(tile);
            }
        }
        this.isFirstTile = true;
    }

    public void StopPreview()
    {
        this.godMode = false;
        isPreviewing = false;
        lastMouseCellPosition = Vector3Int.zero;
        foreach (Tilemap tilemap in tilemaps)
        {
            this.tilemapsToTileLayerManagers[tilemap].ConfirmPlacement();
        }
        RenderColorOfColorLinkedTiles(addedTiles.Keys.ToList());
        foreach (GameTile tile in referencedTiles)
        {
            if (tile.targetTilemap.GetComponent<TileContentsManager>() == null && tile.targetTilemap.TryGetComponent(out TileColorManager placedTileColorManager))
            {
                foreach (Vector3Int vector3Int in addedTiles.Keys)
                {
                    placedTileColorManager.SetTileColor(vector3Int, tile);
                }
            }
        }

        // Set terrain modified flag
        this.TileSystem.HasTerrainChanged = true;
        this.TileSystem.changedTiles.AddRange(addedTiles.Keys.ToList());

        // Clear all dics
        this.referencedTiles.Clear();
        this.removedTileColors.Clear();
        this.addedTiles.Clear();
        this.triedToPlaceTiles.Clear();
    }

    public int PlacedTileCount()
    {
        return addedTiles.Count();
    }

    public void RevertChanges() // Go through each change and revert back to original
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            this.tilemapsToTileLayerManagers[tilemap].Revert();
            if (tilemap.TryGetComponent(out TileContentsManager tileAttributes))
            {
                List<Vector3Int> changedTiles = tileAttributes.changedTilesPositions;
                changedTiles.AddRange(tileAttributes.addedTilePositions);
                tileAttributes.Revert();
                RenderColorOfColorLinkedTiles(changedTiles);
            }
        }
        foreach (Vector3Int colorChangedTiles in removedTileColors.Keys)
        {
            removedTileColors[colorChangedTiles].Values.First().SetColor(colorChangedTiles, removedTileColors[colorChangedTiles].Keys.First());
        }
        removedTileColors.Clear();
        addedTiles.Clear();
        triedToPlaceTiles.Clear();
        StopPreview();
    }

    public void RenderColorOfColorLinkedTiles(List<Vector3Int> changedTiles) // Update color for linked tiles.
    {
        foreach (GameTile tile in referencedTiles)
        {
            if (colorLinkedTiles.Keys.Contains(tile))
            {
                foreach (Tilemap tilemap in colorLinkedTiles[tile])
                {
                    TileColorManager tileColorManager = tilemap.GetComponent<TileColorManager>();
                    foreach (Vector3Int addedTileLocation in changedTiles)
                    {
                        foreach (GameTile managedTile in tileColorManager.managedTiles)
                        {
                            foreach (Vector3Int affectedTileLocation in this.TileSystem.AllCellLocationsOfTileInRange(addedTileLocation, tileColorManager.coloringMethod.affectedRange, managedTile))
                            {
                                tileColorManager.SetTileColor(affectedTileLocation, managedTile);
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdatePreviewPen()
    {
        if (isFirstTile)
        {
            PlaceTile(currentMouseCellPosition);
            return;
        }
        if (!GridUtils.FourNeighborTiles(currentMouseCellPosition).Contains(lastPlacedTile)) // Detect non-continuous points, and linearly interpolate to fill the gaps
        {
            if (currentMouseCellPosition.x == lastPlacedTile.x)// Handles divide by zero exception
            {
                foreach (int y in GridUtils.Range(lastPlacedTile.y, currentMouseCellPosition.y))
                {
                    Vector3Int location = new Vector3Int(lastPlacedTile.x, y, currentMouseCellPosition.z);
                    PlaceTile(location);
                }
            }
            else
            {
                float gradient = (currentMouseCellPosition.y - lastPlacedTile.y) / (currentMouseCellPosition.x - lastPlacedTile.x);
                foreach (float x in GridUtils.RangeFloat(GridUtils.IncreaseMagnitude(lastPlacedTile.x, -0.5f), currentMouseCellPosition.x))
                {
                    float interpolatedY = gradient * (x - lastPlacedTile.x);
                    int incrementY = GridUtils.RoundTowardsZeroInt(interpolatedY);
                    Vector3Int interpolateTileLocation = new Vector3Int(GridUtils.RoundTowardsZeroInt(x), lastPlacedTile.y + incrementY, lastPlacedTile.z);
                    PlaceTile(interpolateTileLocation);
                }
            }
        }
        PlaceTile(currentMouseCellPosition);
    }

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
        foreach (int x in GridUtils.Range(dragStartPosition.x, currentMouseCellPosition.x))
        {
            foreach (int y in GridUtils.Range(dragStartPosition.y, currentMouseCellPosition.y))
            {
                supposedTiles.Add(new Vector3Int(x, y, currentMouseCellPosition.z));
                tilesToRemove.Add(new Vector3Int(x, y, currentMouseCellPosition.z));
            }
        }
        tilesToRemove.ExceptWith(addedTiles.Keys); // Forcing removal of all tiles not in bound to avoid leftover tile not being removed due to lagging and tick skipping, possible optimization
        Vector3Int sweepLocation = Vector3Int.zero;
        sweepLocation.z = currentMouseCellPosition.z;
        bool isXShrinking = GridUtils.IsOppositeSign(currentMouseCellPosition.x - dragStartPosition.x, currentMouseCellPosition.x - lastCornerX);
        bool isYShrinking = GridUtils.IsOppositeSign(currentMouseCellPosition.y - dragStartPosition.y, currentMouseCellPosition.y - lastCornerY);
        if (currentMouseCellPosition.x != lastCornerX || !isXShrinking)
        {
            foreach (int x in GridUtils.Range(lastCornerX, currentMouseCellPosition.x))
            {
                foreach (int y in GridUtils.Range(dragStartPosition.y, currentMouseCellPosition.y))
                {
                    sweepLocation.x = x;
                    sweepLocation.y = y;
                    tilesToAdd.Add(sweepLocation);
                }
            }
        }
        if (currentMouseCellPosition.y != lastCornerY || !isYShrinking)
        {
            foreach (int x in GridUtils.Range(dragStartPosition.x, currentMouseCellPosition.x))
            {
                foreach (int y in GridUtils.Range(lastCornerY, currentMouseCellPosition.y))
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
/*        foreach (Vector3Int addLocation in tilesToAdd)
        {
            PlaceTile(addLocation);
        }
        if (tilesToRemove.Count > 0)
        {
            foreach (Vector3Int removeLocation in tilesToRemove)
            {
                RestoreReplacedTile(removeLocation);
            }
            foreach (GameTile tile in referencedTiles)
            {
                if (tile.targetTilemap.TryGetComponent(out TileContentsManager tileAttributes))
                {
                    //tileAttributes.Revert(supposedTiles);
                }
            }

        }*/
        lastCornerX = currentMouseCellPosition.x;
        lastCornerY = currentMouseCellPosition.y;
    }

    private bool IsPlacable(Vector3Int cellPosition)
    {
        if (currentMouseCellPosition == dragStartPosition)
        {
            return true;
        }
        foreach (Vector3Int location in GridUtils.FourNeighborTiles(cellPosition))
        {
            if (triedToPlaceTiles.Contains(location))
            {
                return true;
            }
        }
        return false;
    }

    private PlacementResult PlaceTile(Vector3Int cellPosition, bool checkPlacable = true) //Main function controls tile placement
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
                // If Constrain not met
                foreach (Tilemap constrainTilemap in tile.constraintTilemaps)
                {
                    if (!constrainTilemap.HasTile(cellPosition))
                    {
                        return PlacementResult.Restricted;
                    }
                }
                // If same tile
                if ((GameTile)tile.targetTilemap.GetTile(cellPosition) == tile)
                {
                    this.triedToPlaceTiles.Add(cellPosition);
                    return PlacementResult.AlreadyExisted;
                }
            }
            // Check clear, place tiles
            foreach (GameTile tile in referencedTiles)
            {
                tilemapsToTileLayerManagers[tile.targetTilemap].AddTile(cellPosition, tile);
                foreach (Tilemap replacementTilemap in tile.replacementTilemaps)
                {
                    tilemapsToTileLayerManagers[replacementTilemap].RemoveTile(cellPosition);
                }
            }
            this.triedToPlaceTiles.Add(cellPosition);
            return PlacementResult.Placed;
        }
        return PlacementResult.Restricted;
    }

/*    private void RestoreReplacedTile (Vector3Int cellLocation)
    {
        foreach (GameTile addedTile in addedTiles[cellLocation])
        {
            addedTile.targetTilemap.SetTile(cellLocation, null);
            if (removedTiles.ContainsKey(cellLocation))
            {
                foreach (GameTile removedTile in removedTiles[cellLocation])
                {
                    removedTile.targetTilemap.SetTile(cellLocation, removedTile);
                    if (removedTile.targetTilemap.TryGetComponent(out TileContentsManager tileAttributes))
                    {
                        tileAttributes.Restore(cellLocation);
                    }
                }
            }
            addedTiles.Remove(cellLocation);
        }
        if (removedTileColors.ContainsKey(cellLocation))
        {
            removedTileColors[cellLocation].Values.First().SetColor(cellLocation, removedTileColors[cellLocation].Keys.First());
            removedTileColors.Remove(cellLocation);
        }
    }*/

    private void GetNeighborCellLocations(Vector3Int cellLocation, GameTile tile, Tilemap targetTilemap)
    {
        foreach (Vector3Int tileToCheck in GridUtils.FourNeighborTiles(cellLocation))
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
        if (!GridSystem.IsWithinGridBouds(cellLocation))
        {
            Debug.Log("outside bound");
            return false;
        }
        GridSystem.CellData cellData = GridSystem.CellGrid[cellLocation[0], cellLocation[1]];
        return (!cellData.ContainsAnimal && !cellData.ContainsFood && !cellData.ContainsMachine && !cellData.HomeLocation);
    }
}
