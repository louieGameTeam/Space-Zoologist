using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

// Setup function to give back home locations of given population
/// <summary>
/// Translates the tilemap into a 2d array for keeping track of object locations.
/// </summary>
/// PlaceableArea transparency can be increased or decreased when adding it
public class GridSystem : MonoBehaviour
{
    // using prime numbers instead of binary for flagging LMAO
    public enum TileFlag
    {
        LIQUID_FLAG = 0x2,
        HIGHLIGHT_FLAG = 0x3
    }
    private const float FLAG_VALUE_MULTIPLIER = 256f;

    [SerializeField] public Grid Grid = default;
    [SerializeField] public SpeciesReferenceData SpeciesReferenceData = default;
    [SerializeField] private ReservePartitionManager RPM = default;
    [SerializeField] private PopulationManager PopulationManager = default;
    public List<Tilemap> Tilemaps;
    private BuildBufferManager buildBufferManager;
    [Header("Used to define 2d array")]
    [SerializeField] public int ReserveWidth = default;
    [SerializeField] public int ReserveHeight = default;
    public Vector3Int startTile = default;
    // Food and home locations updated when added, animal locations updated when the store opens up.
    public CellData[,] CellGrid = default;

    List<Vector3Int> HighlightedTiles;
    private Dictionary<Tilemap, Texture2D> TilemapTextureDictionary;
    private MaterialPropertyBlock TilemapPropertyBlock;

    #region Monobehaviour Callbacks
    private void Awake()
    {
        // set up the information textures
        TilemapTextureDictionary = new Dictionary<Tilemap, Texture2D>();
        TilemapPropertyBlock = new MaterialPropertyBlock();
        foreach (Tilemap t in Tilemaps)
        {
            Texture2D tex = new Texture2D(ReserveWidth, ReserveHeight);
            // make black texture
            for (int i = 0; i < ReserveWidth; ++i)
            {
                for (int j = 0; j < ReserveHeight; ++j)
                    tex.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
            tex.Apply();

            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Repeat;

            TilemapRenderer renderer = t.GetComponent<TilemapRenderer>();
            renderer.GetPropertyBlock(TilemapPropertyBlock);
            TilemapPropertyBlock.SetTexture("_GridInformationTexture", tex);
            renderer.SetPropertyBlock(TilemapPropertyBlock);

            TilemapTextureDictionary.Add(t, tex);
        }
        Tilemaps[0].GetComponent<TilemapRenderer>().sharedMaterial.SetVector("_GridTextureDimensions", new Vector2(ReserveWidth, ReserveHeight));
        HighlightedTiles = new List<Vector3Int>();



        this.CellGrid = new CellData[this.ReserveWidth, this.ReserveHeight];
        for (int i=0; i<this.ReserveWidth; i++)
        {
            for (int j=0; j<this.ReserveHeight; j++)
            {
                Vector3Int loc = new Vector3Int(i, j, 0);

                if (startTile == default)
                {
                    startTile = loc;
                }

                // every cell is currently in bounds if they are within the reserved size
                this.CellGrid[i, j] = new CellData(true);
            }
        }
    }

    private void Start()
    {
        this.buildBufferManager = FindObjectOfType<BuildBufferManager>();

        // temporary to show effect
        Tilemap Terrain = Tilemaps.Find(t => t.gameObject.name == "Terrain");
        ApplyFlagToTileTexture(Terrain, new Vector3Int(1, 1, 0), TileFlag.LIQUID_FLAG, Color.clear);
        ApplyFlagToTileTexture(Terrain, new Vector3Int(1, 2, 0), TileFlag.LIQUID_FLAG, Color.clear);

        Terrain.SetTile(new Vector3Int(5, 5, 0), new TileBase());

        try
        {
            EventManager.Instance.SubscribeToEvent(EventType.StoreOpened, () =>
            {
                this.changedTiles.Clear();
            });

            EventManager.Instance.SubscribeToEvent(EventType.StoreClosed, () =>
            {
                // Invoke event and pass the changed tiles that are not walls
                EventManager.Instance.InvokeEvent(EventType.TerrainChange, this.changedTiles.FindAll(
                        pos => this.GetGameTileAt(pos).type != TileType.Wall
                    ));
            });
        }
        catch { };
    }
    #endregion

    #region Flagging and Tile Effects System
    /// <summary>
    /// Applies the flags to the tile in texture data. Flags can create different visual effects depending on the flag/tile.
    /// </summary>
    /// <param name="tilemap">Tilemap to be affected (mostly for terrain).</param>
    /// <param name="tilePosition">Position of tile in grid space.</param>
    /// <param name="flag">Compiled flags to be set.</param>
    /// <param name="color">Color of the tile (in cases of highlighting or other effects that require color).</param>
    public void ApplyFlagToTileTexture(Tilemap tilemap, Vector3Int tilePosition, TileFlag flag, Color color)
    {
        Texture2D TilemapTex = TilemapTextureDictionary[tilemap];

        Color tilePixel = TilemapTex.GetPixel(tilePosition.x, tilePosition.y);

        // currently only in alpha channel, can expand to rgb as well
        // note that color channels can only contain [0...1] due to samplers
        int alphaBitMask = (int)(tilePixel.a * FLAG_VALUE_MULTIPLIER);
        int flagInt = (int)flag;

        // if nothing there, just set the flag
        if (alphaBitMask == 0)
            alphaBitMask = flagInt;
        // otherwise if the flag isn't already there then add it
        else if (alphaBitMask % flagInt != 0)
            alphaBitMask *= flagInt;
        // if there is already the flag do nothing
        else
            return;

        tilePixel.r = color.r;
        tilePixel.g = color.g;
        tilePixel.b = color.b;

        tilePixel.a = (float)alphaBitMask / FLAG_VALUE_MULTIPLIER;
        TilemapTex.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
        TilemapTex.Apply();

        // add to propertyblock
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        renderer.GetPropertyBlock(TilemapPropertyBlock);
        TilemapPropertyBlock.SetTexture("_GridInformationTexture", TilemapTex);
        renderer.SetPropertyBlock(TilemapPropertyBlock);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tilemap">Tilemap to be affected (mostly for terrain).</param>
    /// <param name="tilePosition">Position of tile in grid space.</param>
    /// <param name="flag">Compiled flags to be set.</param>
    public void RemoveFlagsFromTileTexture(Tilemap tilemap, Vector3Int tilePosition, TileFlag flag)
    {
        Texture2D TilemapTex = TilemapTextureDictionary[tilemap];

        Color tilePixel = TilemapTex.GetPixel(tilePosition.x, tilePosition.y);

        int alphaBitMask = (int)(tilePixel.a * FLAG_VALUE_MULTIPLIER);
        int flagInt = (int)flag;

        // return if flag is not there
        if (alphaBitMask == 0 || alphaBitMask % flagInt != 0)
            return;

        alphaBitMask /= flagInt;
        tilePixel.r = 0;
        tilePixel.g = 0;
        tilePixel.b = 0;

        tilePixel.a = (float)alphaBitMask / FLAG_VALUE_MULTIPLIER;
        TilemapTex.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
        TilemapTex.Apply();

        // add to propertyblock
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        renderer.GetPropertyBlock(TilemapPropertyBlock);
        TilemapPropertyBlock.SetTexture("_GridInformationTexture", TilemapTex);
        renderer.SetPropertyBlock(TilemapPropertyBlock);
    }
    public void ToggleGridOverlay()
    {
        // toggle using shader here
        float currentToggleValue = Tilemaps.Find(t => t.gameObject.name == "Terrain").gameObject.GetComponent<TilemapRenderer>().material.GetFloat("_GridOverlayToggle");
        // set up methods for updating all or only some tilemaps
        foreach (Tilemap t in Tilemaps)
            t.gameObject.GetComponent<TilemapRenderer>().sharedMaterial.SetFloat("_GridOverlayToggle", currentToggleValue == 0 ? 1 : 0);
    }

    public void ClearHighlights()
    {
        foreach (Tilemap t in Tilemaps)
        {
            foreach (Vector3Int tilePosition in HighlightedTiles)
                RemoveFlagsFromTileTexture(t, tilePosition, TileFlag.HIGHLIGHT_FLAG);
        }

        HighlightedTiles.Clear();
    }

    public void HighlightTile(Vector3Int tilePosition, Color color)
    {
        if (!HighlightedTiles.Contains(tilePosition))
        {
            HighlightedTiles.Add(tilePosition);

            foreach (Tilemap t in Tilemaps)
                ApplyFlagToTileTexture(t, tilePosition, TileFlag.HIGHLIGHT_FLAG, color);
        }
    }

    // super inefficient but not priority rn
    public void HighlightRadius(Vector3Int tilePosition, Color color, float radius)
    {
        for (int i = 0; i < ReserveHeight; ++i)
        {
            for (int j = 0; j < ReserveWidth; ++j)
            {
                Vector3Int loopedPosition = new Vector3Int(j, i, 0);
                if (Vector3Int.Distance(tilePosition, loopedPosition) <= radius)
                    HighlightTile(loopedPosition, color);
            }
        }
    }
    #endregion

    public bool IsPodPlacementValid(Vector3 mousePosition, AnimalSpecies species)
    {
        Vector3Int gridPosition = WorldToCell(mousePosition);
        return this.CheckSurroundingTerrain(gridPosition, species);
    }

    public bool IsFoodPlacementValid(Vector3 mousePosition, Item selectedItem = null, FoodSourceSpecies species = null)
    {
        if (selectedItem)
            species = SpeciesReferenceData.FoodSources[selectedItem.ID];

        Vector3Int gridPosition = WorldToCell(mousePosition);
        return CheckSurroudingTiles(gridPosition, species);
    }

    public void updateVisualPlacement(Vector3Int gridPosition, Item selectedItem)
    {
        if (SpeciesReferenceData.FoodSources.ContainsKey(selectedItem.ID))
        {
            FoodSourceSpecies species = SpeciesReferenceData.FoodSources[selectedItem.ID];
            CheckSurroudingTiles(gridPosition, species);
        }
        else if (SpeciesReferenceData.AnimalSpecies.ContainsKey(selectedItem.ID))
        {
            AnimalSpecies species = SpeciesReferenceData.AnimalSpecies[selectedItem.ID];
            CheckSurroundingTerrain(gridPosition, species);
        }
        else
        {
            // TODO figure out how to determine if tile is placable
            // gridOverlay.HighlightTile(gridPosition, Color.green);
        }
    }

    private bool CheckSurroundingTerrain(Vector3Int cellPosition, AnimalSpecies selectedSpecies)
    {
        Vector3Int pos;
        GameTile tile;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                pos = cellPosition;
                pos.x += x;
                pos.y += y;
                tile = GetGameTileAt(pos);
                if (tile == null)
                {
                    return false;
                }
                
                HighlightTile(pos, selectedSpecies.AccessibleTerrain.Contains(tile.type) ? Color.green : Color.red);
            }
        }
        tile = GetGameTileAt(cellPosition);
        return selectedSpecies.AccessibleTerrain.Contains(tile.type);
    }

    private bool CheckSurroudingTiles(Vector3Int cellPosition, FoodSourceSpecies species)
    {
        // size 1 -> rad 0, size 3 -> rad 1 ...
        int radius = species.Size / 2;
        Vector3Int pos;
        bool isValid = true;
        int offset = 0;
        // Size is even, offset by 1
        if (species.Size % 2 == 0)
        {
            offset = 1;
        }
        // Check if the whole object is in bounds
        for (int x = (-1 - offset) * (radius - offset); x <= radius; x++)
        {
            for (int y = (-1 - offset) * (radius - offset); y <= radius; y++)
            {
                pos = cellPosition;
                pos.x += x;
                pos.y += y;
                if (!IsFoodPlacementValid(pos, species))
                {
                    isValid = false;
                    HighlightTile(pos, Color.red);
                }
                else
                {
                    HighlightTile(pos, Color.green);
                }
            }
        }
        return isValid;
    }

    public bool IsOnWall(Vector3Int pos)
    {
        // Prevent placing on walls
        GameTile selectedTile = GetGameTileAt(pos);
        if (selectedTile != null && selectedTile.type.Equals(TileType.Wall))
        {
            return true;
        }
        return false;
    }

    // helper function that checks the validity at one tile
    private bool IsFoodPlacementValid(Vector3Int pos, FoodSourceSpecies species)
    {
        if (!IsWithinGridBounds(pos))
        {
            return false;
        }

        // Prevent placing on items already there.
        CellData cellData = CellGrid[pos.x, pos.y];
        if (cellData.ContainsFood)
        {
            return false;
        }
        if (IsOnWall(pos)) return false;
        if (this.buildBufferManager.IsConstructing(pos.x, pos.y))
        {
            return false;
        }
        GameTile selectedTile = GetGameTileAt(pos);

        if (selectedTile)
        {
            // Make sure the tile is acceptable
            foreach (TileType acceptablTerrain in species.AccessibleTerrain)
            {
                if (selectedTile.type.Equals(acceptablTerrain))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool isCellinGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < ReserveWidth && y < ReserveHeight;
    }

    public bool IsWithinGridBounds(Vector3 mousePosition)
    {
        Vector3Int loc = Grid.WorldToCell(mousePosition);

        return isCellinGrid(loc.x, loc.y);
    }

    public void UpdateAnimalCellGrid()
    {
        // Reset previous locations
        for (int i=0; i<this.CellGrid.GetLength(0); i++)
        {
            for (int j=0; j<this.CellGrid.GetLength(1); j++)
            {
                this.CellGrid[i, j].ContainsAnimal = false;
                this.CellGrid[i, j].HomeLocation = false;
                this.CellGrid[i, j].ContainsLiquid = false;
            }
        }
        // Could be broken up for better efficiency since iterating through population twice
        // this.PopulationHomeLocations = this.RecalculateHomeLocation();
        // Update locations and grab reference to animal GameObject (for future use)
        foreach (Population population in this.PopulationManager.Populations)
        {
            foreach (GameObject animal in population.AnimalPopulation)
            {
                Vector3Int animalLocation = this.Grid.WorldToCell(animal.transform.position);
                this.CellGrid[animalLocation.x, animalLocation.y].ContainsAnimal = true;
                this.CellGrid[animalLocation.x, animalLocation.y].Animal = animal;
            }
        }
    }

    public Vector3Int FindClosestLiquidSource(Population population, GameObject animal)
    {
        Vector3Int itemLocation = new Vector3Int(-1, -1, -1);
        float closestDistance = 10000f;
        float localDistance = 0f;
        for (int x = 0; x < this.CellGrid.GetLength(0); x++)
        {
            for (int y = 0; y < this.CellGrid.GetLength(1); y++)
            {
                // if contains liquid tile, check neighbors accessibility
                GameTile tile = GetGameTileAt(new Vector3Int(x, y, 0));
                if (tile != null && tile.type == TileType.Liquid)
                {
                    this.CellGrid[x, y].ContainsLiquid = true;
                    localDistance = Vector2.Distance(animal.transform.position, new Vector2(x, y));

                    if (population.Grid.IsAccessible(x + 1, y))
                    {
                        if (localDistance < closestDistance)
                        {
                            closestDistance = localDistance;
                            itemLocation = new Vector3Int(x + 1, y, 0);
                        }
                    }
                    if (population.Grid.IsAccessible(x - 1, y))
                    {
                        if (localDistance < closestDistance)
                        {
                            closestDistance = localDistance;
                            itemLocation = new Vector3Int(x - 1, y, 0);
                        }
                    }
                    if (population.Grid.IsAccessible(x, y + 1))
                    {
                        if (localDistance < closestDistance)
                        {
                            closestDistance = localDistance;
                            itemLocation = new Vector3Int(x, y + 1, 0);
                        }
                    }
                    if (population.Grid.IsAccessible(x, y - 1))
                    {
                        if (localDistance < closestDistance)
                        {
                            closestDistance = localDistance;
                            itemLocation = new Vector3Int(x, y - 1, 0);
                        }
                    }
                    
                }
            }
        }
        return itemLocation;
    }

    // Will need to make the grid the size of the max tilemap size
    public AnimalPathfinding.Grid GetGridWithAccess(Population population)
    {
        // Debug.Log("Setting up pathfinding grid");
        bool[,] tileGrid = new bool[ReserveWidth, ReserveHeight];
        for (int x=0; x<ReserveWidth; x++)
        {
            for (int y=0; y<ReserveHeight; y++)
            {
                tileGrid[x, y] = RPM.CanAccess(population, new Vector3Int(x, y, 0));
            }
        }
        return new AnimalPathfinding.Grid(tileGrid, this.Grid);
    }

    public void AddFood(Vector3Int gridPosition, int size, GameObject foodSource)
    {
        int radius = size / 2;
        Vector3Int pos;
        int offset = 0;
        if (size % 2 == 0)
        {
            offset = 1;
        }
        // Check if the whole object is in bounds
        for (int x = (-1) * (radius - offset); x <= radius; x++)
        {
            for (int y = (-1) * (radius - offset); y <= radius; y++)
            {
                pos = gridPosition;
                pos.x += x;
                pos.y += y;
                CellGrid[pos.x, pos.y].ContainsFood = true;
                CellGrid[pos.x, pos.y].Food = foodSource;
            }
        }
    }

    // Remove the food source on this tile, a little inefficient
    public void RemoveFood(Vector3Int gridPosition)
    {
        Vector3Int pos = gridPosition;
        if (!CellGrid[pos.x, pos.y].ContainsFood) return;

        GameObject foodSource = CellGrid[pos.x, pos.y].Food;
        int size = foodSource.GetComponent<FoodSource>().Species.Size;

        for (int dx = -size; dx < size; dx++)
        {
            for (int dy = -size; dy < size; dy++)
            {
                CellData cell = CellGrid[pos.x + dx, pos.y + dy];
                if (cell.ContainsFood && ReferenceEquals(cell.Food, foodSource))
                {
                    CellGrid[pos.x + dx, pos.y + dy].ContainsFood = false;
                    CellGrid[pos.x + dx, pos.y + dy].Food = null;
                }
            }
        }
    }

    public bool HasTerrainChanged = false;
    public List<Vector3Int> changedTiles = new List<Vector3Int>();

    /// <summary>
    /// Convert a world position to cell positions on the grid.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return Grid.WorldToCell(worldPosition);
    }
    /// <summary>
    /// Returns all liquid tiles belong to the same liquid body at the given location
    /// </summary>
    /// <param name="location">Cell location of any liquid tile within the body to change</param>
    /// <returns></returns>
    public List<Vector3Int> GetLiquidBodyPositions(Vector3Int location)
    {
        List<Vector3Int> liquidBodyTiles = new List<Vector3Int>();
        GameTile terrainTile = GetGameTileAt(location);
        liquidBodyTiles.Add(location);
        GetNeighborLiquidLocations(location, terrainTile, liquidBodyTiles);
        return liquidBodyTiles;
    }

    private void GetNeighborLiquidLocations(Vector3Int location, GameTile tile, List<Vector3Int> liquidBodyTiles)
    {
        foreach (Vector3Int tileToCheck in FourNeighborTiles(location))
        {
            if (
                tile.targetTilemap.GetTile(tileToCheck) == tile &&
                GetTileContentsAt(tileToCheck, tile) != null &&
                !liquidBodyTiles.Contains(tileToCheck))
            {
                liquidBodyTiles.Add(tileToCheck);
                GetNeighborLiquidLocations(tileToCheck, tile, liquidBodyTiles);
            }
            GameTile thisTile = (GameTile)tile.targetTilemap.GetTile(tileToCheck);
            float[] contents = GetTileContentsAt(tileToCheck, tile);
        }
    }
    /// <summary>
    /// Change the composition of all connecting liquid tiles of the selected location
    /// </summary>
    /// <param name="cellPosition">Cell location of any liquid tile within the body to change </param>
    /// <param name="composition">Composition that will either be added or used to modify original composition</param>
    /// <param name="isSetting">When set to true, original composition will be replaced by input composition. When set to false, input composition will be added to original Composition</param>
    public void ChangeLiquidBodyComposition(Vector3Int cellPosition, float[] composition)
    {
        TileLayerManager tileLayerManager = GetGameTileAt(cellPosition).targetTilemap.GetComponent<TileLayerManager>();
        if (tileLayerManager.holdsContent)
        {
            tileLayerManager.ChangeComposition(cellPosition, composition);

            // Invoke event
            EventManager.Instance.InvokeEvent(EventType.LiquidChange, cellPosition);
            return;
        }
        Debug.LogError("Tile at position" + cellPosition + "does not hold content");
    }
    /// <summary>
    /// Returns TerrainTile(inherited from Tilebase) at given location of a cell within the Grid.
    /// </summary>
    /// <param name="cellLocation"> Position of the cell. </param>
    /// <returns></returns>
    public GameTile GetGameTileAt(Vector3Int cellLocation)
    {
        if (cellLocation.x < 0 || cellLocation.y < 0) // Tiles shouldn't be in negative coordinates
        {
            //Debug.Log("Trying accessing tiles at negative coordinate" + cellLocation);
            return null;
        }
        foreach (Tilemap tilemap in Tilemaps)
        {
            var returnedTile = tilemap.GetTile<GameTile>(cellLocation);
            if (returnedTile != null)
            {
                return returnedTile;
            }
        }
        //Debug.LogWarning("Tile does not exist at " + cellLocation);
        return null;
    }

    /// <summary>
    /// Whether a tile exists at given location, regardless to overlapping
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool TileExistsAtLocation(Vector3Int cellLocation, GameTile tile)
    {
        return tile.targetTilemap.GetTile(cellLocation) == tile;
    }

    /// <summary>
    /// Returns contents within a tile, e.g. Liquid Composition. If tile has no content, returns null.
    /// </summary>
    /// <param name="cellPosition"> Position of the cell. </param>
    /// <returns></returns>
    public float[] GetTileContentsAt(Vector3Int cellPosition, GameTile tile = null)
    {
        tile = tile == null ? GetGameTileAt(cellPosition) : tile;
        if (tile != null)
        {
            TileLayerManager tileLayerManager = tile.targetTilemap.GetComponent<TileLayerManager>();
            if (tileLayerManager.holdsContent)
            {
                return tileLayerManager.GetLiquidBodyAt(cellPosition).contents;
            }
            else
            {
                return null;
            }
        }
        return null;
    }
    public LiquidBody GetLiquidBodyAt(Vector3Int cellPosition)
    {
        GameTile tile = this.GetGameTileAt(cellPosition);
        if (tile != null)
        {
            return tile.targetTilemap.GetComponent<TileLayerManager>().GetLiquidBodyAt(cellPosition);
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// Returns the cell location of the tile closest to the given center. List contains multiple if more than one tile at same distance.
    /// </summary>
    /// <param name="centerCellLocation">The cell location to calculate distance to</param>
    /// <param name="tile">The tile of interest</param>
    /// <param name="scanRange">1/2 side length of the scaned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
    public List<Vector3Int> CellLocationsOfClosestTiles(Vector3Int centerCellLocation, GameTile tile, int scanRange = 8, bool isCircleMode = false)
    {
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
                if (GetGameTileAt(centerCellLocation) == tile)
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
    public Dictionary<float[], float> DistancesToClosestTilesOfEachBody(Vector3Int centerCellLocation, GameTile tile, int scanRange = 8, bool isCircleMode = false)
    {
        int[] distance3 = new int[3];
        int i = 0;
        int posX = 0;
        int posY = 0;
        Dictionary<float[], float> compositionDistancePairs = new Dictionary<float[], float>();
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
                if (GetGameTileAt(centerCellLocation) == tile)
                {
                    compositionDistancePairs.Add(GetTileContentsAt(centerCellLocation, tile), distance);
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
                        float[] contents = GetTileContentsAt(cellLocation, tile);
                        if (!compositionDistancePairs.Keys.Contains(contents))
                        {
                            compositionDistancePairs.Add(GetTileContentsAt(cellLocation, tile), distance);
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
                        float[] contents = GetTileContentsAt(cellLocation, tile);
                        if (!compositionDistancePairs.Keys.Contains(contents))
                        {
                            compositionDistancePairs.Add(GetTileContentsAt(cellLocation, tile), distance);
                        }
                    }
                }
                posY++;
            }
        }
        return compositionDistancePairs;
    }

    /// <summary>
    /// Returns distance of closet tile from a given cell position. Returns -1 if not found.
    /// </summary>
    /// <param name="centerCellLocation">The cell location to calculate distance to</param>
    /// <param name="tile">The tile of interest</param>
    /// <param name="scanRange">1/2 side length of the scanned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
    public float DistanceToClosestTile(Vector3Int centerCellLocation, GameTile tile, int scanRange = 8, bool isCircleMode = false)
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
                if (GetGameTileAt(centerCellLocation) == tile)
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
    /// TODO not working, don't use until fixed
    public List<Vector3Int> AllCellLocationsinRange(Vector3Int centerCellLocation, int scanRange)
    {
        List<Vector3Int> tileLocations = new List<Vector3Int>();
        Vector3Int scanLocation = new Vector3Int(0, 0, centerCellLocation.z);
        foreach (int x in Range(-scanRange, scanRange))
        {
            foreach (int y in Range(-scanRange, scanRange))
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                if (distance > scanRange)
                {
                    continue;
                }
                scanLocation.x = x + centerCellLocation.x;
                scanLocation.y = y + centerCellLocation.y;
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
    /// <param name="scanRange">1/2 side length of the scanned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
    public List<Vector3Int> AllCellLocationsOfTileInRange(Vector3Int centerCellLocation, int scanRange, GameTile tile, bool isCircleMode = false)
    {
        List<Vector3Int> tileLocations = new List<Vector3Int>();
        Vector3Int scanLocation = new Vector3Int(0, 0, centerCellLocation.z);
        foreach (int x in Range(-scanRange, scanRange))
        {
            foreach (int y in Range(-scanRange, scanRange))
            {
                if (isCircleMode)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance > scanRange)
                    {
                        continue;
                    }
                }
                scanLocation.x = x + centerCellLocation.x;
                scanLocation.y = y + centerCellLocation.y;
                if (GetGameTileAt(scanLocation) == tile)
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
        foreach (int x in Range(-scanRange, scanRange))
        {
            foreach (int y in Range(-scanRange, scanRange))
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                if (distance > scanRange)
                {
                    continue;
                }

                scanLocation.x = x + centerCellLocation.x;
                scanLocation.y = y + centerCellLocation.y;

                GameTile tile = GetGameTileAt(scanLocation);
                if (tile)
                {
                    typesOfTileWithinRadius[(int)tile.type]++;
                }
            }
        }
        return typesOfTileWithinRadius;
    }

    /// <summary>
    /// Scan from all the liquid tiles within a radius range and return all different liquid compositions
    /// </summary>
    /// <param name="centerCellLocation">The location of the center cell</param>
    /// <param name="scanRange">The radius range to look for</param>
    /// <returns>A list of the compositions, null is there is no liquid within range</returns>
    public List<float[]> GetLiquidCompositionWithinRange(Vector3Int centerCellLocation, int scanRange)
    {
        List<float[]> liquidCompositions = new List<float[]>();

        Vector3Int scanLocation = new Vector3Int(0, 0, centerCellLocation.z);
        foreach (int x in Range(-scanRange, scanRange))
        {
            foreach (int y in Range(-scanRange, scanRange))
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                if (distance > scanRange)
                {
                    continue;
                }

                scanLocation.x = x + centerCellLocation.x;
                scanLocation.y = y + centerCellLocation.y;
                LiquidBody liquid = this.GetLiquidBodyAt(scanLocation);
                if (liquid != null)
                {
                    liquidCompositions.Add(liquid.contents);

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
    /// <param name="scanRange">1/2 side length of the scanned square (radius if isCircleMode = true)</param>
    /// <param name="isCircleMode">Enable circular scan. Default to false, scans a square of side length of scanRange * 2 + 1</param>
    /// <returns></returns>
    public bool IsAnyTileInRange(Vector3Int centerCellLocation, int scanRange, GameTile tile, bool isCircleMode = false)
    {
        if (DistanceToClosestTile(centerCellLocation, tile, scanRange, isCircleMode) == -1)
        {
            return false;
        }
        return true;
    }

    private bool IsTileInAnyOfFour(int distanceX, int distanceY, Vector3Int subjectCellLocation, GameTile tile)
    {
        Vector3Int cell_1 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_2 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        Vector3Int cell_3 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_4 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        if (GetGameTileAt(cell_1) == tile ||
           GetGameTileAt(cell_2) == tile ||
           GetGameTileAt(cell_3) == tile ||
           GetGameTileAt(cell_4) == tile)
        {
            return true;
        }
        return false;
    }

    private bool IsTileInAnyOfEight(int distanceX, int distanceY, Vector3Int subjectCellLocation, GameTile tile)
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
            if (GetGameTileAt(cell_1) == tile ||
               GetGameTileAt(cell_2) == tile ||
               GetGameTileAt(cell_3) == tile ||
               GetGameTileAt(cell_4) == tile)
            {
                return true;
            }
        }
        return false;
    }

    private List<Vector3Int> TileCellLocationsInFour(int distanceX, int distanceY, Vector3Int subjectCellLocation, GameTile tile)
    {
        Vector3Int cell_1 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_2 = new Vector3Int(subjectCellLocation.x + distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        Vector3Int cell_3 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y + distanceY, subjectCellLocation.z);
        Vector3Int cell_4 = new Vector3Int(subjectCellLocation.x - distanceX, subjectCellLocation.y - distanceY, subjectCellLocation.z);
        List<Vector3Int> cells = new List<Vector3Int> { cell_1, cell_2, cell_3, cell_4 };
        List<Vector3Int> results = new List<Vector3Int>();
        foreach (Vector3Int cell in cells)
        {
            if (GetGameTileAt(cell) == tile)
            {
                results.Add(cell);
            }
        }
        return results;
    }

    private List<Vector3Int> TileCellLocationsInEight(int distanceX, int distanceY, Vector3Int subjectCellLocation, GameTile tile)
    {
        Vector3Int cell_1 = new Vector3Int(subjectCellLocation.x + distanceY, subjectCellLocation.y + distanceX, subjectCellLocation.z);
        Vector3Int cell_2 = new Vector3Int(subjectCellLocation.x - distanceY, subjectCellLocation.y + distanceX, subjectCellLocation.z);
        Vector3Int cell_3 = new Vector3Int(subjectCellLocation.x + distanceY, subjectCellLocation.y - distanceX, subjectCellLocation.z);
        Vector3Int cell_4 = new Vector3Int(subjectCellLocation.x - distanceY, subjectCellLocation.y - distanceX, subjectCellLocation.z);
        List<Vector3Int> cells = new List<Vector3Int> { cell_1, cell_2, cell_3, cell_4 };
        List<Vector3Int> results = new List<Vector3Int>();
        foreach (Vector3Int cell in cells)
        {
            if (GetGameTileAt(cell) == tile)
            {
                results.Add(cell);
            }
        }
        results.AddRange(TileCellLocationsInFour(distanceX, distanceY, subjectCellLocation, tile));
        return results;
    }

    // Used to assign which layer 
    public enum TileLayer
    {
        Terrain,
        Structures
    }

    // note that this will not work on web browsers, will need to find alternate solution
    private void OnApplicationQuit()
    {
        // turn off grid toggle no matter what (material is permanently changed)
        foreach (Tilemap t in Tilemaps)
            t.gameObject.GetComponent<TilemapRenderer>().sharedMaterial.SetFloat("_GridOverlayToggle", 0);
    }

    public static Vector3Int SignsVector3(Vector3 vector)
    {
        Vector3Int result = new Vector3Int()
        {
            x = vector.x == 0 ? 0 : (int)(Mathf.Sign(vector.x)),
            y = vector.y == 0 ? 0 : (int)(Mathf.Sign(vector.y)),
            z = vector.z == 0 ? 0 : (int)(Mathf.Sign(vector.z))
        };
        return result;
    }

    public static Vector3Int SignsRoundToIntVector3(Vector3 vector)
    {
        Vector3Int signs = SignsVector3(vector);
        Vector3Int result = new Vector3Int()
        {
            x = signs.x < 0 ? Mathf.FloorToInt(vector.x) : Mathf.CeilToInt(vector.x),
            y = signs.y < 0 ? Mathf.FloorToInt(vector.y) : Mathf.CeilToInt(vector.y),
            z = signs.z < 0 ? Mathf.FloorToInt(vector.z) : Mathf.CeilToInt(vector.z),
        };

        return result;
    }

    /// <summary>
    /// Rounds down or up whichever way is further away from the origin provided.
    /// </summary>
    /// <param name="vector"> The vector to round. </param>
    /// <param name="origin"> The origin that the vector rounds with respect to. </param>
    /// <returns></returns>
    public static Vector3Int SignsRoundToIntVector3(Vector3 vector, Vector3Int origin)
    {
        Vector3Int result = SignsRoundToIntVector3(vector - origin) + origin;

        return result;
    }

    /// <summary>
    /// Returns int from start to end (inclusive), default stepping is 1.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static IEnumerable<int> Range(int start, int end, int step = 1)
    {
        if (start < end)
        {
            for (int i = start; i <= end; i += step)
            {
                yield return i;
            }
        }
        else
        {
            for (int i = start; i >= end; i -= step)
                yield return i;
        }
    }
    /// <summary>
    /// Returns FLOAT from start to end (inclusive), default stepping is 1.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static IEnumerable<float> RangeFloat(float start, float end, float step = 1)
    {
        if (start < end)
        {
            for (float i = start; i <= end; i += step)
            {
                yield return i;
            }
        }
        else
        {
            for (float i = start; i >= end; i -= step)
                yield return i;
        }
    }
    /// <summary>
    /// Returns four cell locations next to the given cell location
    /// </summary>
    /// <param name="cellLocation"></param>
    /// <returns></returns>
    public static List<Vector3Int> FourNeighborTiles(Vector3Int cellLocation)
    {
        List<Vector3Int> fourNeighborTiles = new List<Vector3Int>();
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z));
        fourNeighborTiles.Add(new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z));
        return fourNeighborTiles;
    }
    /// <summary>
    /// Round a number towards zero
    /// </summary>
    /// <param name="n"> Number to round</param>
    /// <returns></returns>
    public static int RoundTowardsZeroInt(float n)
    {
        if (n > 0)
        {
            return Mathf.FloorToInt(n);
        }
        else
        {
            return Mathf.CeilToInt(n);
        }
    }
    /// <summary>
    /// Round a number towards zero, returns int
    /// </summary>
    /// <param name="n"> Number to round </param>
    /// <returns></returns>
    public static int RoundAwayFromZeroInt(float n)
    {
        if (n < 0)
        {
            return Mathf.FloorToInt(n);
        }
        else
        {
            return Mathf.CeilToInt(n);
        }
    }
    /// <summary>
    /// Increase the magnitude (absolute value) of a number
    /// </summary>
    /// <param name="n"> Number to increase </param>
    /// <param name="increment"> Increment </param>
    /// <returns></returns>
    public static float IncreaseMagnitude(float n, float increment)
    {
        if (n > 0)
        {
            return n += increment;
        }
        else
        {
            return n -= increment;
        }
    }
    public static int IncreaseMagnitudeInt(int n, int increment)
    {
        if (n > 0)
        {
            return n += increment;
        }
        else
        {
            return n -= increment;
        }

    }

    public struct CellData
    {
        public CellData(bool inBounds)
        {
            this.ContainsFood = false;
            this.ContainsAnimal = false;
            this.Food = null;
            this.Animal = null;
            this.Machine = null;
            this.ContainsLiquid = false;
            this.HomeLocation = false;
            this.OutOfBounds = !inBounds;
        }

        public bool ContainsLiquid { get; set; }
        public GameObject Machine { get; set; }
        public bool ContainsFood { get; set; }
        public GameObject Food { get; set; }
        public bool ContainsAnimal { get; set; }
        public GameObject Animal { get; set; }
        public bool HomeLocation { get; set; }
        public bool OutOfBounds { get; set; }
    }
}
