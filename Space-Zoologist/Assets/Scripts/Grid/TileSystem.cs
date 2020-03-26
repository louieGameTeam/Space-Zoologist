using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap[] tilemaps;

    private void Awake()
    {
        tilemaps = GetComponent<TilePlacementController>().allTilemaps;
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
    /// Whether a tile exists at given location, regardless to overlapping
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool TileExistsAtLocation(Vector3Int cellLocation, TerrainTile tile)
    {
        return tile.targetTilemap.GetTile(cellLocation) == tile;
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
            if (tile.targetTilemap.TryGetComponent(out TileAttributes tileAttributes))
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

    public List<Vector3Int> CellLocationsOfClosestTiles(Vector3Int centerCellLocation, TerrainTile tile, int scanRange = 8, bool isCircleMode = false)
    {
        int[] distance3 = new int[3];
        int i = 0;
        int posX = 0;
        int posY = 0;
        List<Vector3Int> closestTiles = new List<Vector3Int>();
        while (i < scanRange)
        {
            if (isCircleMode && Mathf.Sqrt(posX * posX + posY * posY) > scanRange)
            {
                i++;
                posX = i;
                posY = 0;
                continue;
            }
            if (posX == 0)
            {
                if (GetTerrainTileAtLocation(centerCellLocation) == tile)
                {
                    closestTiles.Add(centerCellLocation);
                    break;
                }
                i++;
                posX = i;
                continue;
            }
            if (posX == posY)
            {
                if (IsTileInAnyOfFour(posX, posY, centerCellLocation, tile))
                {
                    return TileCellLocationsInFour(posX, posY, centerCellLocation, tile);
                }
                i++;
                posX = i;
                posY = 0;
            }
            else
            {
                if (IsTileInAnyOfEight(posX, posY, centerCellLocation, tile))
                {
                    return TileCellLocationsInEight(posX, posY, centerCellLocation, tile);
                }
                posY++;
            }
        }
        return closestTiles;
    }
    public Dictionary<float[], float> DistancesToClosestTilesOfEachBody(Vector3Int centerCellLocation, TerrainTile tile, int scanRange = 8, bool isCircleMode = false)
    {
        int[] distance3 = new int[3];
        int i = 0;
        int posX = 0;
        int posY = 0;
        Dictionary<float[], float> compositionDistancePairs = new Dictionary<float[], float>();
        while (i < scanRange)
        {
            float distance = Mathf.Sqrt(posX * posX + posY * posY);
            if (isCircleMode &&  distance > scanRange)
            {
                i++;
                posX = i;
                posY = 0;
                continue;
            }
            if (posX == 0)
            {
                if (GetTerrainTileAtLocation(centerCellLocation) == tile)
                {
                    compositionDistancePairs.Add(GetTileContentsAtLocation(centerCellLocation, tile), distance);
                }
                i++;
                posX = i;
                continue;
            }
            if (posX == posY)
            {
                if (IsTileInAnyOfFour(posX, posY, centerCellLocation, tile))
                {
                    foreach (Vector3Int cellLocation in TileCellLocationsInFour(posX, posY, centerCellLocation, tile))
                    {
                        float[] contents = GetTileContentsAtLocation(cellLocation, tile);
                        if (!compositionDistancePairs.Keys.Contains(contents))
                        {
                            compositionDistancePairs.Add(GetTileContentsAtLocation(cellLocation, tile), distance);
                        }
                    }
                }
                i++;
                posX = i;
                posY = 0;
            }
            else
            {
                if (IsTileInAnyOfEight(posX, posY, centerCellLocation, tile))
                {
                    foreach (Vector3Int cellLocation in TileCellLocationsInEight(posX, posY, centerCellLocation, tile))
                    {
                        float[] contents = GetTileContentsAtLocation(cellLocation, tile);
                        if (!compositionDistancePairs.Keys.Contains(contents))
                        {
                            compositionDistancePairs.Add(GetTileContentsAtLocation(cellLocation, tile), distance);
                        }
                    }
                }
                posY++;
            }
        }
        return compositionDistancePairs;
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
    /// <summary>
    /// Returns a list of locations of all tiles in a certain range
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <param name="scanRange"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Whether any of given tile is within a given range. 
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <param name="scanRange"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
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
    private List<Vector3Int> TileCellLocationsInFour(int distanceX, int distanceY, Vector3Int subjectCellLocation, TerrainTile tile)
    {
        Vector3Int cell_1 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_2 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        Vector3Int cell_3 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_4 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        List<Vector3Int> cells = new List<Vector3Int> { cell_1, cell_2, cell_3, cell_4 };
        List<Vector3Int> results = new List<Vector3Int>();
        foreach (Vector3Int cell in cells)
        {
            if (GetTerrainTileAtLocation(cell) == tile)
            {
                results.Add(cell);
            }
        }
        return results;
    }
    private List<Vector3Int> TileCellLocationsInEight(int distanceX, int distanceY, Vector3Int subjectCellLocation, TerrainTile tile)
    {
        Vector3Int cell_1 = new Vector3Int(subjectCellLocation.x + distanceY, subjectCellLocation.y + distanceX, subjectCellLocation.z);
        Vector3Int cell_2 = new Vector3Int(subjectCellLocation.x - distanceY, subjectCellLocation.y + distanceX, subjectCellLocation.z);
        Vector3Int cell_3 = new Vector3Int(subjectCellLocation.x + distanceY, subjectCellLocation.y - distanceX, subjectCellLocation.z);
        Vector3Int cell_4 = new Vector3Int(subjectCellLocation.x - distanceY, subjectCellLocation.y - distanceX, subjectCellLocation.z);
        List<Vector3Int> cells = new List<Vector3Int> { cell_1, cell_2, cell_3, cell_4 };
        List<Vector3Int> results = new List<Vector3Int>();
        foreach (Vector3Int cell in cells)
        {
            if (GetTerrainTileAtLocation(cell) == tile)
            {
                results.Add(cell);
            }
        }
        results.AddRange(TileCellLocationsInFour(distanceX, distanceY, subjectCellLocation, tile));
        return results;
    }
}
