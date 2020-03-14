using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacementController : MonoBehaviour
{
    // Can be either pen or block mode.
    /* 
     * Known issues: 
     * Block mode must start at a valid cell or it doesn't draw
     * Stops drawing if cursor moves too fast in pen mode
     */
    public bool isBlockMode { get; set; } = false;
    public TerrainTile selectedTile { get; set; } = default;
    [SerializeField] private Camera currentCamera = default;
    private bool isPreviewing { get; set; } = false;
    private Vector3Int dragStartPosition = Vector3Int.zero;
    private Vector3Int lastMouseCellPosition = Vector3Int.zero;
    private Vector3Int currentMouseCellPosition = Vector3Int.zero;
    private Grid grid;
    public List<Tilemap> tilemapList { get { return tilemaps; } }
    [SerializeField] private List<Tilemap> tilemaps = new List<Tilemap>();
    private Dictionary<int, List<Vector3Int>> addedTiles = new Dictionary<int, List<Vector3Int>>(); // All NEW tiles placed
    private Dictionary<int, Dictionary<Vector3Int, TerrainTile>> removedTiles = new Dictionary<int, Dictionary<Vector3Int, TerrainTile>>(); //All tiles removed
    private List<Vector3Int> triedToPlaceTiles = new List<Vector3Int>(); // New tiles and same tile 
    private List<Vector3Int> neighborTiles = new List<Vector3Int>();
    private Dictionary<Vector3Int, float[]> changedAttributes = new Dictionary<Vector3Int, float[]>();

    private void Awake()
    {
        grid = GetComponent<Grid>();
        foreach (int layer in (int[])Enum.GetValues(typeof(TerrainTile.TileLayer)))
        {
            addedTiles.Add(layer, new List<Vector3Int>());
            removedTiles.Add(layer, new Dictionary <Vector3Int, TerrainTile> ());
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
    }
    public void StopPreview()
    {
        isPreviewing = false;
        lastMouseCellPosition = Vector3Int.zero;
        addedTiles.Clear();
        removedTiles.Clear();
        triedToPlaceTiles.Clear();
        foreach (int layer in (int[])Enum.GetValues(typeof(TerrainTile.TileLayer)))
        {
            addedTiles.Add(layer, new List<Vector3Int>());
            removedTiles.Add(layer, new Dictionary<Vector3Int, TerrainTile>());
            if (tilemaps[layer].TryGetComponent(out TileAttributes tileAttributes))
            {
                tileAttributes.ConfirmMerge();
            }
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
        PlaceTile(currentMouseCellPosition, selectedTile);
    }
    private void UpdatePreviewBlock()
    {
        ClearChanges();
        PlaceTile(dragStartPosition, selectedTile, false);
        Vector3Int sweepLocation = Vector3Int.zero;
        if (dragStartPosition.x < currentMouseCellPosition.x)
        {
            for (var x = dragStartPosition.x; x <= currentMouseCellPosition.x; x++)
            {
                if (dragStartPosition.y < currentMouseCellPosition.y)
                {
                    for (var y = dragStartPosition.y ; y <= currentMouseCellPosition.y ; y++)
                    {
                        sweepLocation = new Vector3Int(x, y, 0);
                        PlaceTile(sweepLocation, selectedTile);
                    }
                }
                else
                {
                    for (var y = dragStartPosition.y; y >= currentMouseCellPosition.y; y--)
                    {
                        sweepLocation = new Vector3Int(x, y, 0);
                        PlaceTile(sweepLocation, selectedTile);
                    }
                }
            }
        }
        else
        {
            for (var x = dragStartPosition.x; x >= currentMouseCellPosition.x; x--)
            {
                if (dragStartPosition.y < currentMouseCellPosition.y)
                {
                    for (var y = dragStartPosition.y; y <= currentMouseCellPosition.y; y++)
                    {
                        sweepLocation = new Vector3Int(x, y, 0);
                        PlaceTile(sweepLocation, selectedTile);
                    }
                }
                else
                {
                    for (var y = dragStartPosition.y; y >= currentMouseCellPosition.y; y--)
                    {
                        sweepLocation = new Vector3Int(x, y, 0);
                        PlaceTile(sweepLocation, selectedTile);
                    }
                }
            }
        }
    }

    private bool IsPlacable(Vector3Int cellLocation)
    {
        Vector3Int posX0 = new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z);
        Vector3Int posX1 = new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z);
        Vector3Int posy0 = new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z);
        Vector3Int posy1 = new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z);
        if (triedToPlaceTiles.Contains(posX0) ||
            triedToPlaceTiles.Contains(posX1) ||
            triedToPlaceTiles.Contains(posy0) ||
            triedToPlaceTiles.Contains(posy1) ||
            currentMouseCellPosition == dragStartPosition)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void PlaceTile(Vector3Int cellLocation, TerrainTile tile, bool checkPlacable = true)
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
                    TileRemovalProcess(layer, cellLocation);
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
                            TilePlacementProcess(tileLayer, targetTilemap, cellLocation, tile);
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
                    TilePlacementProcess(tileLayer, targetTilemap, cellLocation, tile);
                }
            }
            else
            {
                triedToPlaceTiles.Add(cellLocation);
            }
        }
    }
    private void TilePlacementProcess(int tileLayer,Tilemap targetTilemap, Vector3Int cellLocation, TerrainTile tile)
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
    private void TileRemovalProcess(int tileLayer, Vector3Int cellLocation)
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
        Vector3Int posX0 = new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z);
        Vector3Int posX1 = new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z);
        Vector3Int posy0 = new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z);
        Vector3Int posy1 = new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z);
        List<Vector3Int> tilesToCheck = new List<Vector3Int> { posX0, posX1, posy0, posy1 };
        foreach (Vector3Int tileToCheck in tilesToCheck)
        {
            if (!neighborTiles.Contains(tileToCheck) && targetTilemap.GetTile(tileToCheck) == tile)
            {
                neighborTiles.Add(tileToCheck);
                GetNeighborCellLocations(tileToCheck, tile, targetTilemap);
            }
        }
    }
}
