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
                return tileAttributes.tileContent[cellLocation];
            }
            else
            {
                return null;
            }
        }
        return null;
    }
    public float DistanceToClosestTile(Vector3Int cellLocation,int scanRange, TerrainTile tile)
    {
        List<Vector3Int> tileLocations = new List<Vector3Int>();
        for(int x=cellLocation.x-scanRange;x<cellLocation.x+scanRange;x++)
        {
            for (int y = cellLocation.y - scanRange; y < cellLocation.y + scanRange; y++)
            {
                Vector3Int currentCell = new Vector3Int(x, y, 0);
                if (GetTerrainTileAtLocation(currentCell) == tile)
                {
                    tileLocations.Add(currentCell);
                }
            }
        }
        return 0;
    }
}
