using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializedTileData
{
    public int TileID;
    public int LiquidBodyID;
    public bool Placeable;
    public bool Locked;
    public int Repetitions;
    public SerializedTileData(GameTile tile, int liquidbodyID, bool placeable, int repetitions, bool locked = false)
    {
        this.TileID = tile == null ? -1 : (int)tile.type;
        this.LiquidBodyID = liquidbodyID;
        this.Placeable = placeable;
        this.Repetitions = repetitions;
        this.Locked = locked;
    }
}
