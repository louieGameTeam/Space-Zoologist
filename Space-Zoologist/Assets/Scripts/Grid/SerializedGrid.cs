using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SerializedGrid
{
    public SerializedTilemap[] serializedTilemaps;
    // TODO Add other floating objects

    public SerializedGrid(Dictionary<Tilemap, TileLayerManager> tilemapsToTileLayerManagers)
    {
        this.serializedTilemaps = new SerializedTilemap[tilemapsToTileLayerManagers.Count];
        int i = 0;
        foreach(TileLayerManager tileLayerManager in tilemapsToTileLayerManagers.Values)
        {
            this.serializedTilemaps[i] = tileLayerManager.Serialize();
            i++;
        }
    }
}
