using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GetTerrainTile : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Tilemap> tilemaps;

    private void Awake()
    {
        tilemaps = GetComponent<TilePlacementController>().tilemapList;
    }
    public TerrainTile GetTerrainTileAtLocation(Vector3Int cellLocation)
    {
        SortedDictionary<int, TerrainTile> existingTiles = new SortedDictionary<int, TerrainTile>();

        foreach (Tilemap tilemap in tilemaps)
        {
            TerrainTile tileOnLayer = (TerrainTile)tilemap.GetTile(cellLocation);
            if (tileOnLayer != null)
            {
                if (tileOnLayer.isRepresentative)
                {
                    existingTiles.Add(tileOnLayer.priority, tileOnLayer);
                }
            }
        }
        if (existingTiles.Count > 0)
        {
            return existingTiles.Last().Value;
        }
        else
        {
            return null;
        }
    }
    public float[] GetTileContentsAtLocation(Vector3Int cellLocation, TerrainTile tile)
    {
        if (tile != null)
        {
            if (tilemaps[(int)tile.tileLayer].TryGetComponent(out TileAttributes tileAttributes))
            {
                return tileAttributes.keyValuePairs[cellLocation];
            }
            else
            {
                return null;
            }
        }
        return null;
    }
}
