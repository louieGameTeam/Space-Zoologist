using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileData
{
    public GameTile currentTile { get; private set; }
    public GameTile previousTile { get; private set; }
    public Color currentColor { get; private set; }
    public Color previousColor { get; private set; }
    public LiquidBody currentLiquidBody { get; private set; }

    public LiquidBody previousLiquidBody { get; private set; }
    public Vector3Int tilePosition { get; private set; }
    public bool isTileChanged { get; private set; } = false;
    public bool isLiquidBodyChanged { get; private set; } = false;
    public bool isColorChanged { get; private set; } = false;
    public void Clear()
    {
        this.currentTile = null;
        this.previousTile = null;
        this.currentLiquidBody = null;
        this.previousLiquidBody = null;
    }
    public TileData(Vector3Int tilePosition)
    {
        this.tilePosition = tilePosition;
        this.currentTile = null;
        this.currentColor = Color.white;
        this.currentLiquidBody = null;
        this.isTileChanged = false;
        this.isColorChanged = false;
    }
    public TileData(GameTile tile, Vector3Int tilePosition, Color tileColor, LiquidBody liquidBody = null)
    {
        this.currentTile = tile;
        this.currentColor = tileColor;
        this.previousColor = new Color(1, 1, 1);
        this.currentLiquidBody = liquidBody;
        this.tilePosition = tilePosition;

        this.isTileChanged = false;
        this.isColorChanged = false;
    }
    public void PreviewReplacement(GameTile tile, LiquidBody liquidBody = null)
    {
        if (isTileChanged)
        {
            this.currentTile = tile;
            this.currentLiquidBody = liquidBody;
            return;
        }
        this.previousTile = this.currentTile;
        Debug.Log("previous:" + this.previousTile ?? this.previousTile.TileName + "current:" + this.currentTile ?? this.currentTile.TileName);
        this.previousLiquidBody = this.currentLiquidBody;
        this.currentTile = tile;
        this.currentLiquidBody = liquidBody;
        this.isTileChanged = true;
    }
    public void PreviewColorChange(Color color)
    {
        if (isColorChanged)
        {
            this.currentColor = color;
            return;
        }
        this.previousColor = this.currentColor;
        this.currentColor = color;
        this.isColorChanged = true;
    }
    public void PreviewLiquidBody(LiquidBody newLiquidBody)
    {
        if (previousLiquidBody == null && (this.currentLiquidBody != null && this.currentLiquidBody.bodyID != 0))
        {
            this.previousLiquidBody = this.currentLiquidBody;
            this.currentLiquidBody = newLiquidBody;
            this.isLiquidBodyChanged = true;
            return;
        }
        this.isLiquidBodyChanged = true;
        currentLiquidBody = newLiquidBody;
    }
    public void ConfirmReplacement()
    {
        if (currentTile == null)
        {
            this.currentColor = Color.white;
            if (this.currentLiquidBody != null)
            {
                this.currentLiquidBody.RemoveTile(tilePosition); // Remove Tile from liquid body
            }
            return;
        }
        ClearHistory();
    }
    public void Revert()
    {
        if (isTileChanged)
        {
            this.currentTile = this.previousTile;
        }
        if (isLiquidBodyChanged)
        {
            this.currentLiquidBody = this.previousLiquidBody;
        }
        if (isColorChanged)
        {
            this.currentColor = this.previousColor;
        }
        ClearHistory();
    }
    private void ClearHistory()
    {
        this.previousColor = Color.white;
        this.previousLiquidBody = null;
        this.previousTile = null;
        this.isTileChanged = false;
        this.isColorChanged = false;
    }
    public SerializedTileData Serialize()
    {
        if (this.currentTile == null)
        {
            Debug.LogError("Tile is null at " + this.tilePosition);
        }
        return new SerializedTileData(this.tilePosition, this.currentTile, this.currentColor, this.currentLiquidBody);
    }
}
