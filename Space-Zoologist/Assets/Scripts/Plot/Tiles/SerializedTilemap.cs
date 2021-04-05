using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedTilemap
{
    public string TilemapName;
    public SerializedTileData[] SerializedTileDatas;
    public SerializedLiquidBody[] SerializedLiquidBodies;
    public SerializedTilemap (string tilemapName, Dictionary<Vector3Int, TileData> positionsToTileDatas, HashSet<LiquidBody> liquidBodies)
    {
        this.TilemapName = tilemapName;
        this.SerializedTileDatas = new SerializedTileData[positionsToTileDatas.Count];
        int i = 0;
        foreach (TileData tileData in positionsToTileDatas.Values)
        {
            this.SerializedTileDatas[i] = tileData.Serialize();
            i++;
        }
        i = 0;
        this.SerializedLiquidBodies = new SerializedLiquidBody[liquidBodies.Count];
        foreach (LiquidBody liquidBody in liquidBodies)
        {
            this.SerializedLiquidBodies[i] = liquidBody.Serialize();
            i++;
        }
    }
}
