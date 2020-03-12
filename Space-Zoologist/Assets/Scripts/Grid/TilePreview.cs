using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilePreview : MonoBehaviour
{
    public TileBase testTile = default;

    // Can be either pen or block mode.
    public bool isBlockMode { get; set; } = false;
    public TileBase selectedTile { get; set; } = default;
    [SerializeField] private Camera currentCamera = default;
    private bool isPreviewing { get; set; } = false;
    private Vector3Int dragStartPosition = Vector3Int.zero;
    private Vector3Int lastMouseCellPosition = Vector3Int.zero;
    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        if (isPreviewing)
        {
            Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int currentMouseCellPosition = tilemap.WorldToCell(mouseWorldPosition);
            if(currentMouseCellPosition != lastMouseCellPosition)
            {
                if (isBlockMode)
                {
                    UpdatePreviewBlock();
                }
                else
                {
                    tilemap.SetTile(currentMouseCellPosition, selectedTile);
                }
                lastMouseCellPosition = currentMouseCellPosition;
            }
        }
    }

    public void StartPreview(TileBase newTile)
    {
        isPreviewing = true;
        selectedTile = newTile;
        Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        dragStartPosition = tilemap.WorldToCell(mouseWorldPosition);
    }

    public void StopPreview()
    {
        isPreviewing = false;
        tilemap.ClearAllTiles();
        lastMouseCellPosition = Vector3Int.zero;
    }

    private void UpdatePreviewBlock()
    {
        Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseLocalPosition = tilemap.WorldToLocal(mouseWorldPosition);
        Vector3Int roundedMouseCellPosition = GridUtils.SignsRoundToIntVector3(mouseLocalPosition, dragStartPosition);
        Vector3Int max = Vector3Int.Max(dragStartPosition + Vector3Int.one, roundedMouseCellPosition);
        Vector3Int min = Vector3Int.Min(dragStartPosition, roundedMouseCellPosition);
        Vector3Int size = max - min;
        tilemap.ClearAllTiles();

        size.z = 1;
        BoundsInt bounds = new BoundsInt(min, size);
        DrawBlock(bounds, selectedTile);
    }

    /// <summary>
    /// Replaces TileMap.setTilesBlock() to allow for block placement using RuleTiles.
    /// </summary>
    /// <param name="bounds"> The block in which the tiles will be placed. </param>
    /// <param name="tile"> The tile to fill the block with. </param>
    private void DrawBlock(BoundsInt bounds, TileBase tile)
    {
        for(var x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (var y = bounds.yMin; y < bounds.yMax; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
