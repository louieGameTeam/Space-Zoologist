using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TilemapStatics
{
    /// <summary>
    /// Returns four cell locations next to the given cell location
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <returns></returns>
    public static List<Vector3Int> FourNeighborTileLocations(Vector3Int cellLocation)
    {
        List<Vector3Int> fourNeighborTiles = new List<Vector3Int>();
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z));
        return fourNeighborTiles;
    }

    /// <summary>
    /// Checks if the tile neighbors in 4 directions
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static bool IsNeighborTile(Vector3Int pos1, Vector3Int pos2)
    {
        return (pos1 - pos2).sqrMagnitude == 1;
    }
}
