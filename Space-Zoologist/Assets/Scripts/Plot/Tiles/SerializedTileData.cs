using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedTileData
{
    public int TileID;
    public int LiquidBodyID;
    public int Repetitions;
    public SerializedTileData(GameTile tile, LiquidBody liquidBody, int repetitions)
    {
        this.TileID = (int)tile.type;
        this.LiquidBodyID = liquidBody != null ? liquidBody.bodyID : 0;
        this.Repetitions = repetitions;
    }
}
