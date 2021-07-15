using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedTilemap
{
    public string TilemapName;
    public SerializedTileData[] SerializedTileDatas;
    public SerializedLiquidBody[] SerializedLiquidBodies;
    public SerializedTilemap (string tilemapName, List<GridSystem.TileData> tiles, HashSet<LiquidBody> liquidBodies)
    {
        this.TilemapName = tilemapName;

        // figure out a way to parse all the tilemap information here
        /*
        this.SerializedTileDatas = new SerializedTileData[positionsToTileDatas.Count];
        int i = 0;
        foreach (TileData tileData in positionsToTileDatas.Values)
        {
            this.SerializedTileDatas[i] = tileData.Serialize();
            i++;
        }
        */
        int i = 0;
        this.SerializedLiquidBodies = new SerializedLiquidBody[liquidBodies.Count];
        foreach (LiquidBody liquidBody in liquidBodies)
        {
            this.SerializedLiquidBodies[i] = liquidBody.Serialize();
            i++;
        }
    }
}
