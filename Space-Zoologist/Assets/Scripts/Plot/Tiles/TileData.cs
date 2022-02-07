using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public Vector3Int tilePosition { get; private set; }
    public GameObject Food { get; set; }
    public GameObject Animal { get; set; }

    public GameTile currentTile { get; private set; }
    public GameTile previousTile { get; private set; }
    // no longer relevant to prevent coupling
    // public LiquidBody currentLiquidBody { get; set; }
    // public LiquidBody previewLiquidBody { get; private set; }
    // public float[] contents { get; set; }
    public bool isTilePlaceable { get; set; } = false;
    public bool isConstructing { get; set; } = false;
    public TileData(Vector3Int tilePosition, GameTile tile = null)
    {
        this.Food = null;
        this.Animal = null;

        this.tilePosition = tilePosition;
        this.currentTile = tile;
    }
    public void Clear()
    {
        this.currentTile = null;
        this.previousTile = null;
    }
    public void PreviewReplacement(GameTile tile)
    {
        this.previousTile = this.currentTile;
        this.currentTile = tile;
    }
    public void Revert()
    {
        if (this.previousTile)
        {
            this.currentTile = this.previousTile;
            this.previousTile = null;
        }
    }
    public override string ToString()
    {
        string positionString = "Tile Position: " + tilePosition.ToString() + "\n";
        string foodString = "Food: " + (Food != null ? Food.name : "none") + "\n";
        string currentTileString = "Current Tile: " + (currentTile != null ? currentTile.TileName : "none") + "\n";
        string previousTileString = "Previous Tile: " + (previousTile != null ? previousTile.TileName : "none") + "\n";
        string placeableString = "Placeable: " + (isTilePlaceable ? "True" : "False") + "\n";

        return positionString + foodString + currentTileString + previousTileString;
    }
}
