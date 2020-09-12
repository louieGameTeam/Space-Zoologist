using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class TileSystem : MonoBehaviour
{
    [SerializeField] Tilemap GrassTilemap = default;
    // Start is called before the first frame update
    private List<Tilemap> tilemaps = new List<Tilemap>();
    private Grid grid;

    public bool HasTerrainChanged = false;
    public List<Vector3Int> chagnedTiles = new List<Vector3Int>();
    private List<Vector3Int> liquidBodyTiles = new List<Vector3Int>();
    private List<Vector3Int> liquidBodyTilesAndContents = new List<Vector3Int>();

    [SerializeField] NeedSystemManager needSystemManager = default;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        Tilemap[] unorderedTilemaps = GetComponent<TilePlacementController>().allTilemaps;
        Dictionary<Tilemap, int> tilemapLayerOrderPairs = new Dictionary<Tilemap, int>();
        foreach (Tilemap tilemap in unorderedTilemaps)
        {
            tilemapLayerOrderPairs.Add(tilemap, tilemap.GetComponent<TilemapRenderer>().sortingOrder);
        }
        foreach (KeyValuePair<Tilemap, int> pair in tilemapLayerOrderPairs.OrderByDescending(key => key.Value))
        {
            tilemaps.Add(pair.Key);
        }
    }

    private void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.StoreClosed, () =>
        {
            // Invoke event and pass the changed tiles that are not walls
            EventManager.Instance.InvokeEvent(EventType.TerrainChange, this.chagnedTiles.FindAll(
                pos => this.GetTerrainTileAtLocation(pos).type != TileType.Wall
            ));
        });
    }

    /// <summary>
    /// Convert a world position to cell positions on the grid.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }
    /// <summary>
    /// Returns all liquid tiles belong to the same liquid body at the given location
    /// </summary>
    /// <param name="vector3Int">Cell location of any liquid tile within the body to change</param>
    /// <returns></returns>
    public List<Vector3Int> GetLiquidBodyPositions(Vector3Int vector3Int)
    {
        liquidBodyTiles.Clear();
        TerrainTile terrainTile = GetTerrainTileAtLocation(vector3Int);
        liquidBodyTiles.Add(vector3Int);
        GetNeighborCellLocations(vector3Int, terrainTile);
        return liquidBodyTiles;
    }
    private void GetNeighborCellLocations(Vector3Int cellLocation, TerrainTile tile)
    {
        foreach (Vector3Int tileToCheck in GridUtils.FourNeighborTiles(cellLocation))
        {
            if (
                tile.targetTilemap.GetTile(tileToCheck) == tile &&
                GetTileContentsAtLocation(tileToCheck, tile) != null &&
                !liquidBodyTiles.Contains(tileToCheck))
            {
                liquidBodyTiles.Add(tileToCheck);
                GetNeighborCellLocations(tileToCheck, tile);
            }
        }
    }
    /// <summary>
    /// Change the composition of all connecting liquid tiles of the selected location
    /// </summary>
    /// <param name="cellLocation">Cell location of any liquid tile within the body to change </param>
    /// <param name="composition">Composition that will either be added or used to modify original composition</param>
    /// <param name="isSetting">When set to true, original composition will be replaced by input composition. When set to false, input composition will be added to original Composition</param>
    public void ChangeLiquidBodyComposition(Vector3Int cellLocation, float[] composition, bool isSetting)
    {
        liquidBodyTilesAndContents.Clear();
        TerrainTile terrainTile = GetTerrainTileAtLocation(cellLocation);
        liquidBodyTilesAndContents.Add(cellLocation);
        ChangeLiquidComposition(cellLocation, composition, terrainTile, isSetting);
        GetNeighborCellLocationsAndAccessComposition(cellLocation, composition, terrainTile, isSetting);

        // Mark liquid NS dirty
        this.needSystemManager.Systems[NeedType.Liquid].MarkAsDirty();

        RefreshTilemapColor(terrainTile.targetTilemap);

        // Invoke event
        EventManager.Instance.InvokeEvent(EventType.LiquidChange, cellLocation);
    }
    private void GetNeighborCellLocationsAndAccessComposition(Vector3Int cellLocation, float[] composition, TerrainTile tile, bool isSetting)
    {
        foreach (Vector3Int tileToCheck in GridUtils.FourNeighborTiles(cellLocation))
        {
            if (
                tile.targetTilemap.GetTile(tileToCheck) == tile &&
                GetTileContentsAtLocation(tileToCheck, tile) != null &&
                !liquidBodyTiles.Contains(tileToCheck))
            {
                liquidBodyTiles.Add(tileToCheck);
                ChangeLiquidComposition(tileToCheck, composition, tile, isSetting);
                GetNeighborCellLocationsAndAccessComposition(tileToCheck, composition, tile, isSetting);
            }
        }
    }
    private void ChangeLiquidComposition(Vector3Int cellLocation, float[] composition, TerrainTile terrainTile, bool isSetting)
    {
        if (terrainTile.targetTilemap.TryGetComponent(out TileContentsManager tileAttributes))
        {
            if (isSetting)
            {
                tileAttributes.SetCompostion(cellLocation, composition);
            }
            else
            {
                tileAttributes.ModifyComposition(cellLocation, composition);
            }
        }
    }
    /// <summary>
    /// Returns TerrainTile(inherited from Tilebase) at given location of a cell within the Grid.
    /// </summary>
    /// <param name="cellLocation"> Position of the cell. </param>
    /// <returns></returns>
    public TerrainTile GetTerrainTileAtLocation(Vector3Int cellLocation)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            var returnedTile = tilemap.GetTile<TerrainTile>(cellLocation);
            if (returnedTile != null && returnedTile.isRepresentative)
            {
                return returnedTile;
            }
            // if (returnedTile != null && returnedTile.GetType().Equals(typeof(TerrainTile))) {
            //     TerrainTile tileOnLayer = (TerrainTile)returnedTile;
            //     if (tileOnLayer.isRepresentative)
            //     {
            //         return tileOnLayer;
            //     }
            // }
        }
        return null;
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
            if (tile.targetTilemap.TryGetComponent(out TileContentsManager tileAttributes))
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
    /// Returns the cell location of the tile closest to the given center. List contains multiple if more than one tile at same distance.
    /// </summary>
    /// <param name="centerCellLocation">The cell location to calculate distance to</param>
    /// <param name="tile">The tile of interest</param>
    /// <param name="scanRange">1/2 side length of the scaned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns distance of cloest tiles with different tile contents. e.g.Liquid composition
    /// </summary>
    /// <param name="centerCellLocation">The cell location to calculate distance to</param>
    /// <param name="tile">The tile of interest</param>
    /// <param name="scanRange">1/2 side length of the scaned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
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
    /// Returns distance of cloest tile from a given cell position. Returns -1 if not found.
    /// </summary>
    /// <param name="centerCellLocation">The cell location to calculate distance to</param>
    /// <param name="tile">The tile of interest</param>
    /// <param name="scanRange">1/2 side length of the scaned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
    public float DistanceToClosestTile(Vector3Int centerCellLocation, TerrainTile tile, int scanRange = 8, bool isCircleMode = false)
    {
        int i = 0;
        int posX = 0;
        int posY = 0;
        while (i < scanRange)
        {
            float distance = Mathf.Sqrt(posX * posX + posY * posY);
            if (isCircleMode && distance > scanRange)
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
                    return distance;
                }
                i++;
                posX = i;
                posY = 0;
            }
            else
            {
                if (IsTileInAnyOfEight(posX, posY, centerCellLocation, tile))
                {
                    return distance;
                }
                posY++;
            }
        }
        return -1;
    }

    /// <summary>
    /// Return all cell locations within a range from a center point
    /// </summary>
    /// <param name="centerCellLocation">Starting center point</param>
    /// <param name="scanRange">search radius</param>
    /// <returns></returns>
    public List<Vector3Int> AllCellLocationsinRange(Vector3Int centerCellLocation, int scanRange)
    {
        List<Vector3Int> tileLocations = new List<Vector3Int>();
        Vector3Int scanLocation = new Vector3Int(0, 0, centerCellLocation.z);
        foreach (int x in GridUtils.Range(centerCellLocation.x - scanRange, centerCellLocation.x + scanRange))
        {
            foreach (int y in GridUtils.Range(centerCellLocation.y - scanRange, centerCellLocation.y + scanRange))
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                if (distance > scanRange)
                {
                    continue;
                }
                scanLocation.x = x;
                scanLocation.y = y;
                tileLocations.Add(scanLocation);
            }
        }
        return tileLocations;
    }


    /// <summary>
    /// Returns a list of locations of all tiles in a certain range
    /// </summary>
    /// <param name="centerCellLocation">The cell location to calculate range from</param>
    /// <param name="tile">The tile of interest</param>
    /// <param name="scanRange">1/2 side length of the scaned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
    public List<Vector3Int> AllCellLocationsOfTileInRange(Vector3Int centerCellLocation, int scanRange, TerrainTile tile, bool isCircleMode = false)
    {
        List<Vector3Int> tileLocations = new List<Vector3Int>();
        Vector3Int scanLocation = new Vector3Int(0, 0, centerCellLocation.z);
        foreach (int x in GridUtils.Range(centerCellLocation.x - scanRange, centerCellLocation.x + scanRange))
        {
            foreach (int y in GridUtils.Range(centerCellLocation.y - scanRange, centerCellLocation.y + scanRange))
            {
                if (isCircleMode)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance > scanRange)
                    {
                        continue;
                    }
                }
                scanLocation.x = x;
                scanLocation.y = y;
                if (GetTerrainTileAtLocation(scanLocation) == tile)
                {
                    tileLocations.Add(scanLocation);
                }
            }
        }
        return tileLocations;
    }

    /// <summary>
    /// Return the count of different types of tiles within a radius range of a given cell location
    /// </summary>
    /// <param name="centerCellLocation">The location of the center cell</param>
    /// <param name="scanRange">The radius range to look for</param>
    public int[] CountOfTilesInRange(Vector3Int centerCellLocation, int scanRange)
    {
        int[] typesOfTileWithinRadius = new int[(int)TileType.TypesOfTiles];
        Vector3Int scanLocation = new Vector3Int(0, 0, centerCellLocation.z);
        foreach (int x in GridUtils.Range(-scanRange, scanRange))
        {
            foreach (int y in GridUtils.Range(-scanRange, scanRange))
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                if (distance > scanRange)
                {
                    continue;
                }
                
                scanLocation.x = x + centerCellLocation.x;
                scanLocation.y = y + centerCellLocation.y;

                TerrainTile tile = GetTerrainTileAtLocation(scanLocation);
                if (tile)
                {
                    typesOfTileWithinRadius[(int)tile.type]++;
                }
            }
        }
        return typesOfTileWithinRadius;
    }

    /// <summary>
    /// Scan from all the liquid tiles withint a radius range and return all different liquid compositions
    /// </summary>
    /// <param name="centerCellLocation">The location of the center cell</param>
    /// <param name="scanRange">The radius range to look for</param>
    /// <returns>A list of the composistions, null is there is no liquid within range</returns>
    public List<float[]> GetLiquidCompositionWithinRange(Vector3Int centerCellLocation, int scanRange)
    {
        List<float[]> liquidCompositions = new List<float[]>();

        Vector3Int scanLocation = new Vector3Int(0, 0, centerCellLocation.z);
        foreach (int x in GridUtils.Range(-scanRange, scanRange))
        {
            foreach (int y in GridUtils.Range(-scanRange, scanRange))
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                if (distance > scanRange)
                {
                    continue;
                }

                scanLocation.x = x + centerCellLocation.x;
                scanLocation.y = y + centerCellLocation.y;

                TerrainTile tile = GetTerrainTileAtLocation(scanLocation);

                if (tile)
                {
                    if (tile.type == TileType.Liquid)
                    {
                        float[] composition = this.GetTileContentsAtLocation(scanLocation, tile);

                        if (!liquidCompositions.Contains(composition))
                        {
                            liquidCompositions.Add(composition);
                        }
                    }
                }
            }
        }

        if (liquidCompositions.Count == 0)
        {
            return null;
        }

        return liquidCompositions;
    }

    /// <summary>
    /// Whether any of given tile is within a given range. 
    /// </summary>
    /// <param name="centerCellLocation">The cell location to calculate range from</param>
    /// <param name="tile">The tile of interest</param>
    /// <param name="scanRange">1/2 side length of the scaned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
    public bool IsAnyTileInRange(Vector3Int centerCellLocation, int scanRange, TerrainTile tile, bool isCircleMode = false)
    {
        if (DistanceToClosestTile(centerCellLocation, tile,scanRange, isCircleMode) == -1)
        {
            return false;
        }
        return true;
    }
    public void RefreshGrassTilemapColor()
    {
        if (this.GrassTilemap.TryGetComponent(out TileColorManager tileColorManager))
        {
            foreach (Vector3Int cellLocation in this.GrassTilemap.cellBounds.allPositionsWithin)
            {
                tileColorManager.SetTileColor(cellLocation, (TerrainTile)this.GrassTilemap.GetTile(cellLocation));
            }
        }
    }

    public void RefreshTilemapColor(Tilemap tilemap)
    {
        if (tilemap.TryGetComponent(out TileContentsManager tileAttributes))
        {
            foreach (Vector3Int cellLocation in tilemap.cellBounds.allPositionsWithin)
            {
                if (tileAttributes.tileContents.ContainsKey(cellLocation))
                {
                    tileAttributes.RefreshAllColors();
                }
            }
        }
        else if (tilemap.TryGetComponent(out TileColorManager tileColorManager))
        {
            foreach (Vector3Int cellLocation in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(cellLocation))
                {
                    tileColorManager.SetTileColor(cellLocation, (TerrainTile)tilemap.GetTile(cellLocation));
                }
            }
        }
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