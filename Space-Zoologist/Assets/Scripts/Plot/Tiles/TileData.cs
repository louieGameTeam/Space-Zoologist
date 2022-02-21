using System;
using UnityEngine;

[Serializable]
public class TileData
{
    // The three variables below were { get; private set; } but that prevents these fields from being serialized for the verbose inspector so they're not
    public Vector3Int tilePosition;
    public GameTile currentTile;
    public GameTile previousTile;

    public GameObject Food;
    public GameObject Animal;
    public float [] contents;
    public bool isTilePlaceable = false;
    public bool isConstructing = false;

    // no longer relevant to prevent coupling
    // public LiquidBody currentLiquidBody { get; set; }
    // public LiquidBody previewLiquidBody { get; private set; }
    // public float[] contents { get; set; }

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
