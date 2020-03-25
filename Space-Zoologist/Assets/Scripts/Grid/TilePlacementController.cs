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
    [SerializeField] private Camera currentCamera = default;
    private bool isPreviewing { get; set; } = false;
    private Vector3Int dragStartPosition = Vector3Int.zero;
    private Vector3Int lastMouseCellPosition = Vector3Int.zero;
    private Vector3Int currentMouseCellPosition = Vector3Int.zero;
    private Grid grid;
    private Vector3Int lastPlacedTile;
    private bool isFirstTile;
    public List<Tilemap> tilemapList { get { return tilemaps; } }
    [SerializeField] private List<Tilemap> tilemaps = new List<Tilemap>(); // Set up according to the order of Enum TileLayer in Terrain tile
    private Dictionary<int, List<Vector3Int>> addedTiles = new Dictionary<int, List<Vector3Int>>(); // All NEW tiles placed
    private Dictionary<int, Dictionary<Vector3Int, TerrainTile>> removedTiles = new Dictionary<int, Dictionary<Vector3Int, TerrainTile>>(); //All tiles removed
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
        foreach (int layer in (int[])Enum.GetValues(typeof(TerrainTile.TileLayer)))
        {
            addedTiles.Add(layer, new List<Vector3Int>());
            removedTiles.Add(layer, new Dictionary <Vector3Int, TerrainTile>());
        }
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.TryGetComponent<TileColorManager>(out TileColorManager tileColorManager))
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
            if (currentMouseCellPosition != lastMouseCellPosition)
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
        foreach(Tilemap tilemap1 in tilemaps)
        {
            if (tilemap1.TryGetComponent(out TileAttributes tileAttributes))
            {
                tileAttributes.ConfirmMerge();
            }
        }
        if (colorLinkedTiles.Keys.Contains(selectedTile))
        {
            foreach (Tilemap tilemap in colorLinkedTiles[selectedTile])
            {
                TileColorManager tileColorManager = tilemap.GetComponent<TileColorManager>();
                List<Vector3Int> affectedTileLocations = new List<Vector3Int>();
                List<Vector3Int> changedTileLocations = addedTiles[(int)selectedTile.tileLayer];
                if(tilemaps[(int)selectedTile.tileLayer].TryGetComponent<TileAttributes>(out TileAttributes tileAttributes))
                {
                    changedTileLocations.AddRange(tileAttributes.contentChangedTiles.Keys);
                }
                foreach (Vector3Int addedTileLocation in addedTiles[(int)selectedTile.tileLayer])
                {
                    foreach(TerrainTile managedTile in tileColorManager.managedTiles)
                    {
                        foreach (Vector3Int affectedTileLocation in tileSystem.AllCellLocationsOfTileInRange(addedTileLocation, tileColorManager.coloringMethod.affectedRange, managedTile))
                        {
                            tileColorManager.SetTileColor(affectedTileLocation, managedTile);
                        }
                    }
                }
            }
        }
        if (!tilemaps[(int)selectedTile.tileLayer].TryGetComponent<TileAttributes>(out TileAttributes attributes) && tilemaps[(int)selectedTile.tileLayer].TryGetComponent<TileColorManager>(out TileColorManager placedTileColorManager))
        {
            foreach (Vector3Int vector3Int in addedTiles[(int)selectedTile.tileLayer])
            {
                placedTileColorManager.SetTileColor(vector3Int, selectedTile);
            }
        }
        addedTiles.Clear();
        removedTiles.Clear();
        triedToPlaceTiles.Clear();
        foreach (int layer in (int[])Enum.GetValues(typeof(TerrainTile.TileLayer)))
        {
            addedTiles.Add(layer, new List<Vector3Int>());
            removedTiles.Add(layer, new Dictionary<Vector3Int, TerrainTile>());
        }
    }
    public void RevertChanges()
    {
        ClearChanges();
        StopPreview();

    }
    private void ClearChanges()
    {
        foreach (int layer in addedTiles.Keys)
        {
            foreach (Vector3Int cellLocatoion in addedTiles[layer])
            {
                tilemaps[layer].SetTile(cellLocatoion, null);
            }
            foreach (KeyValuePair<Vector3Int, TerrainTile> removedTile in removedTiles[layer])
            {
                tilemaps[layer].SetTile(removedTile.Key, removedTile.Value);
            }
            if (tilemaps[layer].TryGetComponent(out TileAttributes tileAttributes))
            {
                tileAttributes.Revert();
            }
        }
        addedTiles.Clear();
        removedTiles.Clear();
        triedToPlaceTiles.Clear();
        foreach (int layer in (int[])Enum.GetValues(typeof(TerrainTile.TileLayer)))
        {
            addedTiles.Add(layer, new List<Vector3Int>());
            removedTiles.Add(layer, new Dictionary<Vector3Int, TerrainTile>());
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
    private void UpdatePreviewBlock() //TODO Improve Efficiency && Resolve lag when moving back and forth
    {
        /*        ClearChanges();
                PlaceTile(dragStartPosition, selectedTile, false);
                Vector3Int sweepLocation = Vector3Int.zero;
                foreach (int x in GridUtils.Range(dragStartPosition.x, currentMouseCellPosition.x))
                {
                    foreach (int y in GridUtils.Range(dragStartPosition.y, currentMouseCellPosition.y))
                    {
                        sweepLocation = new Vector3Int(x, y, 0);
                        PlaceTile(sweepLocation, selectedTile);
                    }
                }*/
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
        foreach (Vector3Int existingTile in addedTiles[(int)selectedTile.tileLayer])
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
                RemoveTile(removeLocation, selectedTile);
            }
            if (tilemaps[(int)selectedTile.tileLayer].TryGetComponent(out TileAttributes tileAttributes))
            {
                tileAttributes.Revert(supposedTiles);
                foreach (Vector3Int tileInBox in supposedTiles)
                {
                    tileAttributes.MergeTile(tileInBox, selectedTile, addedTiles[(int)selectedTile.tileLayer]);
                }
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
    private bool PlaceTile(Vector3Int cellLocation, TerrainTile tile, bool checkPlacable = true)
    {
        int tileLayer = (int)tile.tileLayer;
        Tilemap targetTilemap = tilemaps[tileLayer];
        if (IsPlacable(cellLocation) || !checkPlacable)
        {
            // Remove conflicting tiles
            foreach (int layer in tile.replacementLayers)
            {
                if (tilemaps[layer].HasTile(cellLocation))
                {
                    ReplaceTile(layer, cellLocation);
                }
            }
            // Add new tiles
            if (tile != (TerrainTile)targetTilemap.GetTile(cellLocation))
            {
                if (tile.constraintLayers.Count > 0)
                {
                    foreach (int layer in tile.constraintLayers)
                    {
                        if (tilemaps[layer].HasTile(cellLocation))
                        {
                            AddTile(tileLayer, targetTilemap, cellLocation, tile);
                        }
                    }
                }
                else
                {
                    TerrainTile removedTile = (TerrainTile)tilemaps[tileLayer].GetTile(cellLocation);
                    if (!removedTiles[tileLayer].ContainsKey(cellLocation))
                    {
                        removedTiles[tileLayer].Add(cellLocation, removedTile);
                    }
                    AddTile(tileLayer, targetTilemap, cellLocation, tile);
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
    private void RemoveTile (Vector3Int cellLocation, TerrainTile tile)
    {
        foreach (int layer in addedTiles.Keys)
        {
            tilemaps[layer].SetTile(cellLocation, null);
            if (removedTiles[layer].ContainsKey(cellLocation))
            {
                tilemaps[layer].SetTile(cellLocation, removedTiles[layer][cellLocation]);
            }
        }
    }
    private void AddTile(int tileLayer,Tilemap targetTilemap, Vector3Int cellLocation, TerrainTile tile)
    {
        triedToPlaceTiles.Add(cellLocation);
        addedTiles[tileLayer].Add(cellLocation);
        targetTilemap.SetTile(cellLocation, tile);
        PlaceAuxillaryTile(cellLocation, tile);
        if (targetTilemap.TryGetComponent(out TileAttributes tileAttributes))
        {
            tileAttributes.MergeTile(cellLocation, tile, addedTiles[(int)tile.tileLayer]);
        }
    }
    private void ReplaceTile(int tileLayer, Vector3Int cellLocation)
    {
        TerrainTile removedTile = (TerrainTile)tilemaps[tileLayer].GetTile(cellLocation);
        if (!removedTiles[tileLayer].ContainsKey(cellLocation))
        {
            removedTiles[tileLayer].Add(cellLocation, removedTile);
        }
        if (tilemaps[tileLayer].TryGetComponent(out TileAttributes tileAttributes))
        {
            tileAttributes.RemoveTile(cellLocation);
        }
        tilemaps[tileLayer].SetTile(cellLocation, null);
    }
    private void PlaceAuxillaryTile(Vector3Int cellLocation, TerrainTile tile)
    {
        foreach (TerrainTile auxillaryTile in tile.auxillaryTiles)
        {
            addedTiles[(int)auxillaryTile.tileLayer].Add(cellLocation);
            tilemaps[(int)auxillaryTile.tileLayer].SetTile(cellLocation, auxillaryTile);
            addedTiles[(int)auxillaryTile.tileLayer].Add(cellLocation);
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
