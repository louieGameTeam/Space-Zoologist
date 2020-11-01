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
    public TileData(GameTile tile, Vector3Int tilePosition, Color tileColor, LiquidBody liquidBody = null)
    {
        this.currentTile = tile;
        this.currentColor = tileColor;
        this.previousColor = new Color(1, 1, 1);
        this.currentLiquidBody = liquidBody;
        this.tilePosition = tilePosition;
    }
    public void PreviewReplacement(GameTile tile, LiquidBody liquidBody = null)
    {
        this.previousTile = this.currentTile;
        this.previousColor = this.currentColor;
        this.previousLiquidBody = this.currentLiquidBody;
        this.currentTile = tile;
        this.currentLiquidBody = liquidBody;
    }
    public void PreviewLiquidBody(LiquidBody newLiquidBody)
    {
        if (previousLiquidBody == null && this.currentLiquidBody.bodyID != 0)
        {
            this.previousLiquidBody = this.currentLiquidBody;
            this.currentLiquidBody = newLiquidBody;
            return;
        }
        currentLiquidBody = newLiquidBody;
    }
    public void ConfirmReplacement()
    {
        if (currentTile == null)
        {
            this.currentColor = new Color(1, 1, 1);
            this.currentLiquidBody.RemoveTile(tilePosition);
            return;
        }
        ClearHistory();
    }
    public void Revert()
    {
        this.currentTile = this.previousTile;
        this.currentColor = this.previousColor;
        this.currentLiquidBody = this.previousLiquidBody;
        ClearHistory();
    }
    private void ClearHistory()
    {
        this.previousColor = new Color(1, 1, 1);
        this.previousLiquidBody = null;
        this.previousTile = null;
    }
}
