using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedTileData
{
    public int[] TilePosition;
    public string TileName;
    public float[] Color;
    public int LiquidBodyID;
    public SerializedTileData(Vector3Int tilePosition, GameTile tile, Color color, LiquidBody liquidBody)
    {
        this.TilePosition = new int[3] { tilePosition.x, tilePosition.y, tilePosition.z };
        this.TileName = tile.name;
        this.Color = new float[4] { color[0], color[1], color[2], color[3] };
        this.LiquidBodyID = liquidBody != null ? liquidBody.bodyID : 0; 
    }
}
