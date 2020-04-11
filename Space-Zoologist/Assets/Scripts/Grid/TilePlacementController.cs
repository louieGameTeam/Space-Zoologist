using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacementController : MonoBehaviour
{
    // Can be either pen or block mode.
    public bool isBlockMode { get; set; } = false;
    public TerrainTile selectedTile { get; set; } = default;
    public Vector3Int mouseCellPosition { get { return currentMouseCellPosition; } }
    [SerializeField] private Camera currentCamera = default;
    private bool isPreviewing { get; set; } = false;
    private Vector3Int dragStartPosition = Vector3Int.zero;
    private Vector3Int lastMouseCellPosition = Vector3Int.zero;
    private Vector3Int currentMouseCellPosition = Vector3Int.zero;
    private Grid grid;
    private Vector3Int lastPlacedTile;
    private bool isFirstTile;
    public Tilemap[] allTilemaps { get { return tilemaps; } }
    [SerializeField] private Tilemap[] tilemaps = default; // Order according to GridUtils.TileLayer
    [SerializeField] private TerrainTile[] terrainTiles = default;
    private Dictionary<Vector3Int, List<TerrainTile>> addedTiles = new Dictionary<Vector3Int, List<TerrainTile>>(); // All NEW tiles 
    private Dictionary<Vector3Int, List<TerrainTile>> removedTiles = new Dictionary<Vector3Int, List<TerrainTile>>(); //All tiles removed
    private Dictionary<Vector3Int, Dictionary<Color, Tilemap>> removedTileColors = new Dictionary<Vector3Int, Dictionary<Color, Tilemap>>();
    private List<Vector3Int> triedToPlaceTiles = new List<Vector3Int>(); // New tiles and same tile 
    private List<Vector3Int> neighborTiles = new List<Vector3Int>();
    private Dictionary<TerrainTile, List<Tilemap>> colorLinkedTiles = new Dictionary<TerrainTile, List<Tilemap>>();
    private TileSystem tileSystem;
    private int lastCornerX;
    private int lastCornerY;
    private void Awake()
    {
        tileSystem = FindObjectOfType<TileSystem>();
        grid = GetComponent<Grid>();
        foreach (TerrainTile terrainTile in terrainTiles)
        {
            terrainTile.targetTilemap = tilemaps[(int)terrainTile.targetLayer];
            terrainTile.constraintTilemap.Clear();
            terrainTile.replacementTilemap.Clear();
            foreach (GridUtils.TileLayer layer in terrainTile.constraintLayers)
            {
                terrainTile.constraintTilemap.Add(tilemaps[(int)layer]);
            }
            foreach (GridUtils.TileLayer layer in terrainTile.replacementLayers)
            {
                terrainTile.replacementTilemap.Add(tilemaps[(int)layer]);
            }
        }
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.TryGetComponent(out TileColorManager tileColorManager))
            {
                foreach (TerrainTile tile in tileColorManager.linkedTiles)
                {
                    if (!colorLinkedTiles.ContainsKey(tile))
                    {
                        colorLinkedTiles.Add(tile, new List<Tilemap>());
                    }
                    colorLinkedTiles[tile].Add(tilemap);
                }
            }
        }
    }
    void Update()
    {
        if (isPreviewing)
        {
            Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
            currentMouseCellPosition = grid.WorldToCell(mouseWorldPosition);
            if (currentMouseCellPosition != lastMouseCellPosition || isFirstTile)
            {
                if (isBlockMode)
                {
                    UpdatePreviewBlock();
                }
                else
                {
                    UpdatePreviewPen();
                }
                lastMouseCellPosition = currentMouseCellPosition;
            }
        }
    }
    public void StartPreview(TerrainTile newTile)
    {
        isPreviewing = true;
        selectedTile = newTile;
        Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        dragStartPosition = grid.WorldToCell(mouseWorldPosition);
        isFirstTile = true;
    }
    public void StopPreview()
    {
        isPreviewing = false;
        lastMouseCellPosition = Vector3Int.zero;
        foreach (Tilemap tilemap1 in tilemaps)
        {
            if (tilemap1.TryGetComponent(out TileContentsManager tilecContentsManager))
            {
                tilecContentsManager.ConfirmMerge();
            }
        }
        RenderColorOfColorLinkedTiles(addedTiles.Keys.ToList());
        if (selectedTile.targetTilemap.GetComponent<TileContentsManager>() == null && selectedTile.targetTilemap.TryGetComponent(out TileColorManager placedTileColorManager))
        {
            foreach (Vector3Int vector3Int in addedTiles.Keys)
            {
                placedTileColorManager.SetTileColor(vector3Int, selectedTile);
            }
        }
        removedTileColors.Clear();
        addedTiles.Clear();
        removedTiles.Clear();
        triedToPlaceTiles.Clear();
    }
    public void RevertChanges()
    {
        foreach (Vector3Int changedTileLocation in triedToPlaceTiles)
        {
            if (addedTiles.ContainsKey(changedTileLocation))
            {
                foreach (TerrainTile addedTile in addedTiles[changedTileLocation])
                {
                    addedTile.targetTilemap.SetTile(changedTileLocation, null);
                }
            }
            if (removedTiles.ContainsKey(changedTileLocation))
            {
                foreach (TerrainTile removedTile in removedTiles[changedTileLocation])
                {
                    removedTile.targetTilemap.SetTile(changedTileLocation, removedTile);
                }
            }
        }
        foreach (Tilemap tilemap in tilemaps)
        {
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
        removedTiles.Clear();
        triedToPlaceTiles.Clear();
        StopPreview();
    }
    public void RenderColorOfColorLinkedTiles(List<Vector3Int> changedTiles)
    {
        if (colorLinkedTiles.Keys.Contains(selectedTile))
        {
            foreach (Tilemap tilemap in colorLinkedTiles[selectedTile])
            {
                TileColorManager tileColorManager = tilemap.GetComponent<TileColorManager>();
                foreach (Vector3Int addedTileLocation in changedTiles)
                {
                    foreach (TerrainTile managedTile in tileColorManager.managedTiles)
                    {
                        foreach (Vector3Int affectedTileLocation in tileSystem.AllCellLocationsOfTileInRange(addedTileLocation, tileColorManager.coloringMethod.affectedRange, managedTile))
                        {
                            tileColorManager.SetTileColor(affectedTileLocation, managedTile);
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
            PlaceTile(currentMouseCellPosition, selectedTile);
            return;
        }
        if (!GridUtils.FourNeighborTiles(currentMouseCellPosition).Contains(lastPlacedTile))
        {
            if (currentMouseCellPosition.x == lastPlacedTile.x)// Handles divide by zero exception
            {
                foreach (int y in GridUtils.Range(lastPlacedTile.y, currentMouseCellPosition.y))
                {
                    Vector3Int location = new Vector3Int(lastPlacedTile.x, y, currentMouseCellPosition.z);
                    PlaceTile(location, selectedTile);
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
                    PlaceTile(interpolateTileLocation, selectedTile);
                }
            }
        }
        PlaceTile(currentMouseCellPosition, selectedTile);
    }
    private void UpdatePreviewBlock() //TODO Resolve lag when moving back and forth
    {
        if (isFirstTile)
        {
            PlaceTile(dragStartPosition, selectedTile, false);
            lastCornerX = dragStartPosition.x;
            lastCornerY = dragStartPosition.y;
        }
        List<Vector3Int> tilesToRemove = new List<Vector3Int>();
        List<Vector3Int> tilesToAdd = new List<Vector3Int>();
        List<Vector3Int> supposedTiles = new List<Vector3Int>();
        foreach (int x in GridUtils.Range(dragStartPosition.x, currentMouseCellPosition.x))
        {
            foreach (int y in GridUtils.Range(dragStartPosition.y, currentMouseCellPosition.y))
            {
                supposedTiles.Add(new Vector3Int(x, y, currentMouseCellPosition.z));
            }
        }
        foreach (Vector3Int existingTile in addedTiles.Keys) // Forcing removal of all tiles not in bound to avoid leftover tile not being removed due to lagging, possible optimization
        {
            if (!supposedTiles.Contains(existingTile))
            {
                tilesToRemove.Add(existingTile);
            }
        }
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
        foreach (Vector3Int addLocation in tilesToAdd)
        {
            PlaceTile(addLocation, selectedTile);
        }
        if (tilesToRemove.Count > 0)
        {
            foreach (Vector3Int removeLocation in tilesToRemove)
            {
                RestoreReplacedTile(removeLocation);
            }
            if (selectedTile.targetTilemap.TryGetComponent(out TileContentsManager tileAttributes))
            {
                tileAttributes.Revert(supposedTiles);
            }

        }
        lastCornerX = currentMouseCellPosition.x;
        lastCornerY = currentMouseCellPosition.y;
    }

    private bool IsPlacable(Vector3Int cellLocation)
    {

        if (currentMouseCellPosition == dragStartPosition)
        {
            return true;
        }
        foreach (Vector3Int location in GridUtils.FourNeighborTiles(cellLocation))
        {
            if (triedToPlaceTiles.Contains(location))
            {
                return true;
            }
        }
        return false;
    }
    private bool PlaceTile(Vector3Int cellLocation, TerrainTile placedTile, bool checkPlacable = true)
    {
        Tilemap targetTilemap = placedTile.targetTilemap;
        if (IsPlacable(cellLocation) || !checkPlacable)
        {
            // Remove conflicting tiles
            foreach (Tilemap replacingTilemap in placedTile.replacementTilemap)
            {
                if (replacingTilemap.HasTile(cellLocation))
                {
                    ReplaceTile(replacingTilemap, cellLocation);
                }
            }
            // Add new tiles
            TerrainTile replacedTile = (TerrainTile)targetTilemap.GetTile(cellLocation);
            if (placedTile != replacedTile)
            {
                if (placedTile.constraintTilemap.Count > 0)
                {
                    foreach (Tilemap constraintTilemap in placedTile.constraintTilemap)
                    {
                        if (constraintTilemap.HasTile(cellLocation))
                        {
                            AddNewTile(cellLocation, placedTile);
                        }
                    }
                }
                else
                {
                    if (replacedTile != null)
                    {
                        ReplaceTile(replacedTile.targetTilemap, cellLocation);
                    }
                    AddNewTile(cellLocation, placedTile);
                }
            }
            else
            {
                triedToPlaceTiles.Add(cellLocation);
            }
            lastPlacedTile = cellLocation;
            isFirstTile = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    private void RestoreReplacedTile (Vector3Int cellLocation)
    {
        foreach (TerrainTile addedTile in addedTiles[cellLocation])
        {
            addedTile.targetTilemap.SetTile(cellLocation, null);
            if (removedTiles.ContainsKey(cellLocation))
            {
                foreach (TerrainTile removedTile in removedTiles[cellLocation])
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
    }
    private void AddNewTile(Vector3Int cellLocation, TerrainTile tile)
    {
        triedToPlaceTiles.Add(cellLocation);
        if (!addedTiles.ContainsKey(cellLocation))
        {
            addedTiles.Add(cellLocation, new List<TerrainTile> { tile });
        }
        else
        {
            addedTiles[cellLocation].Add(tile);
        }
        tile.targetTilemap.SetTile(cellLocation, tile);
        PlaceAuxillaryTile(cellLocation, tile);
        if (tile.targetTilemap.TryGetComponent(out TileContentsManager tileAttributes))
        {
            tileAttributes.MergeTile(cellLocation, tile, addedTiles.Keys.ToList());
        }
    }
    private void ReplaceTile(Tilemap replacedTilemap, Vector3Int cellLocation)
    {
        TerrainTile removedTile = (TerrainTile)replacedTilemap.GetTile(cellLocation);
        if (!removedTiles.ContainsKey(cellLocation))
        {
            removedTiles.Add(cellLocation, new List<TerrainTile> { removedTile });
        }
        else if (!removedTiles[cellLocation].Contains(removedTile))
        {
            removedTiles[cellLocation].Add(removedTile);
        }
        if (replacedTilemap.TryGetComponent(out TileContentsManager tileAttributes))
        {
            tileAttributes.RemoveTile(cellLocation);
        }
        if (replacedTilemap.TryGetComponent(out TileColorManager tileColorManager))
        {
            if (!removedTileColors.ContainsKey(cellLocation))
            {
                removedTileColors.Add(cellLocation, new Dictionary<Color, Tilemap>());
                removedTileColors[cellLocation].Add(replacedTilemap.GetColor(cellLocation), replacedTilemap);
            }
        }
        replacedTilemap.SetTile(cellLocation, null);
    }
    private void PlaceAuxillaryTile(Vector3Int cellLocation, TerrainTile tile)
    {
        foreach (TerrainTile auxillaryTile in tile.auxillaryTiles)
        {
            if (!addedTiles.ContainsKey(cellLocation))
            {
                addedTiles.Add(cellLocation, new List<TerrainTile> { auxillaryTile });
            }
            else
            {
                addedTiles[cellLocation].Add(auxillaryTile);
            }
            auxillaryTile.targetTilemap.SetTile(cellLocation, auxillaryTile);
        }
    }
    private void GetNeighborCellLocations(Vector3Int cellLocation, TerrainTile tile, Tilemap targetTilemap)
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
}
