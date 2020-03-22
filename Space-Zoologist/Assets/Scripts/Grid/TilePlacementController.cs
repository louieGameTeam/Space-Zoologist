﻿using System.Collections;
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
                tileAttributes.ConfirmMerge(selectedTile);
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
    private void UpdatePreviewPen() //TODO: Fix discrete lines at overlaps within same stroke
    {
        if (!PlaceTile(currentMouseCellPosition, selectedTile))
        {
            if (currentMouseCellPosition.x == lastMouseCellPosition.x)// Handles divide by zero exception
            {
                for (int y = lastMouseCellPosition.y; y <= currentMouseCellPosition.y; y++)
                {
                    Vector3Int location = new Vector3Int(lastMouseCellPosition.x, y, lastMouseCellPosition.z);
                    PlaceTile(location, selectedTile);
                }
            }
            else
            {
                float gradient = (currentMouseCellPosition.y - lastMouseCellPosition.y) / (lastMouseCellPosition.x - currentMouseCellPosition.x);
                bool isPositiveY = currentMouseCellPosition.y > lastMouseCellPosition.y;
                foreach (float x in GridUtils.RangeFloat(lastMouseCellPosition.x + 0.5f , currentMouseCellPosition.x))
                {
                    float interpolatedY = gradient * (x - lastMouseCellPosition.x);
                    int incrementY = Mathf.CeilToInt(interpolatedY);
                    if (!isPositiveY)
                    {
                        incrementY = Mathf.FloorToInt(interpolatedY);
                    }
                    Vector3Int interpolateTileLocation = new Vector3Int(Mathf.CeilToInt(x), lastMouseCellPosition.y + incrementY, lastMouseCellPosition.z);
                    PlaceTile(interpolateTileLocation, selectedTile);
                }
            }
            PlaceTile(currentMouseCellPosition, selectedTile);
        }
    }
    private void UpdatePreviewBlock()
    {
        ClearChanges();
        PlaceTile(dragStartPosition, selectedTile, false);
        Vector3Int sweepLocation = Vector3Int.zero;
        foreach (int x in GridUtils.Range(dragStartPosition.x, currentMouseCellPosition.x))
        {
            foreach (int y in GridUtils.Range(dragStartPosition.y, currentMouseCellPosition.y))
            {
                sweepLocation = new Vector3Int(x, y, 0);
                PlaceTile(sweepLocation, selectedTile);
            }
        }
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
            return true;
        }
        else
        {
            return false;
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
