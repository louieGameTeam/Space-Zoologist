using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private List<Tilemap> tilemaps;

    private void Awake()
    {
        tilemaps = GetComponent<TilePlacementController>().tilemapList;
    }
    /// <summary>
    /// Returns TerrainTile(inherited from Tilebase) at given location of a cell within the Grid.
    /// </summary>
    /// <param name="cellLocation"> Position of the cell. </param>
    /// <returns></returns>
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
    /// <summary>
    /// Returns contents within a tile, e.g. Liquid Composition. If tile has no content, returns null.
    /// </summary>
    /// <param name="cellLocation"> Position of the cell. </param>
    /// <returns></returns>
    public float[] GetTileContentsAtLocation(Vector3Int cellLocation, TerrainTile tile)
    {
        if (tile != null)
        {
            if (tilemaps[(int)tile.tileLayer].TryGetComponent(out TileAttributes tileAttributes))
            {
                return tileAttributes.tileContents[cellLocation];
            }
            else
            {
                return null;
            }
        }
        return null;
    }
    /// <summary>
    /// Returns distance of cloest tile from a given cell position. Scans tiles in a square within a given range, checks a total of (scanRange*2 + 1)^2 tiles. Returns -1 if not found.
    /// </summary>
    /// <param name="centerCellLocation"> Position of the center cell</param>
    /// <param name="scanRange">Maximum displacement in x or y from the center cell</param>
    /// <param name="tile"> Tile to look for </param>
    /// <returns></returns>
    public float DistanceToClosestTile(Vector3Int centerCellLocation, TerrainTile tile, int scanRange = 8)
    {
        int[] distance3 = new int[3];
        int i = 0;
        int posX = 0;
        int posY = 0;
        Vector3Int cellToCheck = centerCellLocation;
        while (i < scanRange)
        {
            if (posX == 0)
            {
                if (GetTerrainTileAtLocation(centerCellLocation) == tile)
                {
                    return 0;
                }
                else
                {
                    i++;
                    posX = i;
                    continue;
                }
            }
            if (posX == posY)
            {
                if (IsTileInAnyOfFour(posX, posY, centerCellLocation, tile))
                {
                    return Mathf.Sqrt(posX * posX + posY * posY);
                }
                i++;
                posX = i;
                posY = 0;
            }
            else
            {
                if (IsTileInAnyOfEight(posX, posY, centerCellLocation, tile))
                {
                    return Mathf.Sqrt(posX * posX + posY * posY);
                }
                posY++;
            }
        }
        return -1;
    }
    public List<Vector3Int> AllCellLocationsOfTileInRange(Vector3Int cellLocation, int scanRange, TerrainTile tile)
    {
        List<Vector3Int> tileLocations = new List<Vector3Int>();
        foreach(int x in GridUtils.Range(cellLocation.x - scanRange, cellLocation.x + scanRange))
        {
            foreach (int y in GridUtils.Range(cellLocation.y - scanRange, cellLocation.y + scanRange))
            {
                Vector3Int scanLocation = new Vector3Int(x, y, cellLocation.z);
                if (GetTerrainTileAtLocation(scanLocation) == tile)
                {
                    tileLocations.Add(scanLocation);
                }
            }
        }
        return tileLocations;
    }
    public bool IsAnyTileInRange(Vector3Int cellLocation, int scanRange, TerrainTile tile)
    {
        if (DistanceToClosestTile(cellLocation,tile,scanRange) == -1)
        {
            return false;
        }
        return true;
    }
    private bool IsTileInAnyOfFour(int distanceX, int distanceY, Vector3Int subjectCellLocation, TerrainTile tile)
    {
        Vector3Int cell_1 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_2 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        Vector3Int cell_3 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_4 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        if (GetTerrainTileAtLocation(cell_1) == tile ||
           GetTerrainTileAtLocation(cell_2) == tile ||
           GetTerrainTileAtLocation(cell_3) == tile ||
           GetTerrainTileAtLocation(cell_4) == tile)
        {
            return true;
        }
        return false;
    }
    private bool IsTileInAnyOfEight(int distanceX, int distanceY,Vector3Int subjectCellLocation, TerrainTile tile)
    {
        if (IsTileInAnyOfFour(distanceX, distanceY, subjectCellLocation, tile))
        {
            return true;
        }
        else
        {
            Vector3Int cell_1 = new Vector3Int(subjectCellLocation.x + distanceY, subjectCellLocation.y + distanceX, subjectCellLocation.z);
            Vector3Int cell_2 = new Vector3Int(subjectCellLocation.x - distanceY, subjectCellLocation.y + distanceX, subjectCellLocation.z);
            Vector3Int cell_3 = new Vector3Int(subjectCellLocation.x + distanceY, subjectCellLocation.y - distanceX, subjectCellLocation.z);
            Vector3Int cell_4 = new Vector3Int(subjectCellLocation.x - distanceY, subjectCellLocation.y - distanceX, subjectCellLocation.z);
            if (GetTerrainTileAtLocation(cell_1) == tile ||
               GetTerrainTileAtLocation(cell_2) == tile ||
               GetTerrainTileAtLocation(cell_3) == tile ||
               GetTerrainTileAtLocation(cell_4) == tile)
            {
                return true;
            }
        }
        return false;
    }
}
