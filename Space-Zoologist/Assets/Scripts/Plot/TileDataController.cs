using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

// Setup function to give back home locations of given population
/// <summary>
/// Translates the tilemap into a 2d array for keeping track of object locations.
/// </summary>
/// PlaceableArea transparency can be increased or decreased when adding it
public class TileDataController : MonoBehaviour
{
    #region Enumerations
    private enum Direction2D { X, Y }
    private enum CellStatus { Other, Self, Walked }
    private enum MoveType { Towards, Parallel1, Parallel2, Away }
    // using prime numbers instead of binary for flagging LMAO
    public enum TileFlag
    {
        HIGHLIGHT_FLAG = 0x2
    }
    private const float FLAG_VALUE_MULTIPLIER = 256f;
    #endregion

    #region Component References
    [SerializeField] private Grid Grid = default;
    [SerializeField] private ReservePartitionManager RPM = default;
    [SerializeField] private PopulationManager PopulationManager = default;
    [SerializeField] public Tilemap Tilemap;
    #endregion

    #region UI
    [SerializeField] Button NextDayButton = default;
    #endregion

    [Header("Used to define 2d array")]
    [SerializeField] private int ReserveWidth = default;
    [SerializeField] private int ReserveHeight = default;
    // grid is accessed using [y, x] due to 2d array structure
    private TileData[,] TileDataGrid = default;
    private List<ConstructionCluster> ConstructionClusters;
    public Vector3Int startTile = default;
    public List<LiquidBody> previewBodies { get; private set; } = new List<LiquidBody>();
    public HashSet<Vector3Int> ChangedTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> RemovedTiles = new HashSet<Vector3Int>();
    private List<Vector3Int> HighlightedTiles = new List<Vector3Int>();
    [SerializeField] private int quickCheckIterations = 6; //Number of tiles to quick check, if can't reach another tile within this many walks, try to generate new body by performing full check
                                                           // Increment by 2 makes a difference. I.E. even numbers, at least 6 to account for any missing tile in 8 surrounding tiles

    public bool IsConstructing(int x, int y) => (y < TileDataGrid.GetLength(0) && x < TileDataGrid.GetLength(1)) ? TileDataGrid[y, x].isConstructing : false;
    private Action constructionFinishedCallback = null;

    private Texture2D TilemapTexture;
    [SerializeField] private GameObject bufferGameObject;
    private Texture2D BufferTexture;
    private Texture2D BufferCenterTexture;

    public bool HasTerrainChanged = false;
    public bool IsDrafting { get; private set; }

    #region Monobehaviour Callbacks

    private void Start()
    {
        try
        {
            EventManager.Instance.SubscribeToEvent(EventType.StoreOpened, () =>
            {
                this.ChangedTiles.Clear();
            });

            List<Vector3Int> changedTilesNoWall = new List<Vector3Int>();
            foreach (Vector3Int tilePosition in ChangedTiles)
            {
                if (this.GetGameTileAt(tilePosition).type != TileType.Wall)
                    changedTilesNoWall.Add(tilePosition);
            }

            EventManager.Instance.SubscribeToEvent(EventType.StoreClosed, () =>
            {
                // Invoke event and pass the changed tiles that are not walls
                EventManager.Instance.InvokeEvent(EventType.TerrainChange, changedTilesNoWall);
            });
        }
        catch { };
    }

    // note that this will not work on web browsers, will need to find alternate solution
    private void OnApplicationQuit()
    {
        // turn off grid toggle no matter what (material is permanently changed)
        Tilemap.GetComponent<TilemapRenderer>().sharedMaterial.SetFloat("_GridOverlayToggle", 0);
    }
    #endregion

    #region I/O
    public void ParseSerializedGrid(SerializedGrid serializedGrid, GameTile[] gameTiles)
    {
        this.previewBodies = new List<LiquidBody>();
        this.RemovedTiles = new HashSet<Vector3Int>();
        this.Tilemap.ClearAllTiles();
        ConstructionClusters = new List<ConstructionCluster>();

        if (gameTiles == null)
            return;

        // set grid dimensions
        ReserveWidth = serializedGrid.width;
        ReserveHeight = serializedGrid.height;
        this.TileDataGrid = new TileData[this.ReserveHeight, this.ReserveWidth];

        // set up the information textures
        TilemapTexture = new Texture2D(ReserveWidth + 1, ReserveHeight + 1);
        // make black texture
        for (int i = 0; i < ReserveWidth; ++i)
        {
            for (int j = 0; j < ReserveHeight; ++j)
                TilemapTexture.SetPixel(i, j, new Color(0, 0, 0, 0));
        }
        TilemapTexture.filterMode = FilterMode.Point;
        TilemapTexture.wrapMode = TextureWrapMode.Repeat;

        Vector3Int tilePosition = new Vector3Int();
        Dictionary<int, HashSet<Vector3Int>> liquidbodyIDToTiles = new Dictionary<int, HashSet<Vector3Int>>();

        foreach (SerializedLiquidBody serializedLiquidBody in serializedGrid.serializedTilemap.SerializedLiquidBodies)
            liquidbodyIDToTiles.Add(serializedLiquidBody.BodyID, new HashSet<Vector3Int>());

        foreach (SerializedTileData serializedTileData in serializedGrid.serializedTilemap.SerializedTileDatas)
        {
            // if the tile id is negative
            if (serializedTileData.TileID == -1)
            {
                for (int i = 0; i < serializedTileData.Repetitions; ++i)
                {
                    TileDataGrid[tilePosition.y, tilePosition.x] = new TileData(tilePosition, null);
                    TileDataGrid[tilePosition.y, tilePosition.x].isTilePlaceable = serializedTileData.Placeable;

                    // move x over first
                    tilePosition.x += 1;

                    // then add to the y after if overflow
                    if (tilePosition.x >= ReserveWidth)
                    {
                        tilePosition.y += 1;
                        tilePosition.x = 0;
                    }
                }
            }
            else
            {
                // set the starting tile here for flood filling
                if (startTile == default && serializedTileData.TileID != (int)TileType.Wall)
                    startTile = tilePosition;

                // search through game tiles for correct tile
                foreach (GameTile gameTile in gameTiles)
                {
                    if ((int)gameTile.type == serializedTileData.TileID)
                    {
                        // add tiles based on repetitions of the same tile
                        for (int i = 0; i < serializedTileData.Repetitions; ++i)
                        {
                            // manually add the tile (may turn into a method later)
                            TileDataGrid[tilePosition.y, tilePosition.x] = new TileData(tilePosition, gameTile);
                            TileDataGrid[tilePosition.y, tilePosition.x].isTilePlaceable = serializedTileData.Placeable;

                            // if it is a liquid, add it to the dictionary
                            if (gameTile.type == TileType.Liquid)
                                liquidbodyIDToTiles[serializedTileData.LiquidBodyID].Add(tilePosition);

                            // set the tile type in the red channel
                            Color pixelColor = TilemapTexture.GetPixel(tilePosition.x, tilePosition.y);
                            // add 1 to ensure a null tile type at 0
                            pixelColor.r = (serializedTileData.TileID + 1) / FLAG_VALUE_MULTIPLIER;
                            TilemapTexture.SetPixel(tilePosition.x, tilePosition.y, pixelColor);

                            // move the tile position along
                            tilePosition.x += 1;

                            // then add to the y after if overflow
                            if (tilePosition.x >= ReserveWidth)
                            {
                                tilePosition.y += 1;
                                tilePosition.x = 0;
                            }
                        }
                    }
                }
            }
        }

        // TODO: put this somewhere else
        LiquidbodyController.Instance.Initialize();

        foreach (SerializedLiquidBody serializedLiquidBody in serializedGrid.serializedTilemap.SerializedLiquidBodies)
        {
            // create the liquidbody based on previously parsed tiles
            LiquidBody liquidBody = new LiquidBody(liquidbodyIDToTiles[serializedLiquidBody.BodyID], serializedLiquidBody.Contents, serializedLiquidBody.BodyID);

            // add it to the list
            LiquidbodyController.Instance.AddLiquidBody(liquidBody);
        }

        // generate buffer plane
        Mesh bufferMesh = bufferGameObject.GetComponent<MeshFilter>().mesh;
        bufferMesh.Clear();
        Vector3[] bufferMeshVertices = new Vector3[4];

        bufferMeshVertices[0] = new Vector3(0, 0, 0);
        bufferMeshVertices[1] = new Vector3(ReserveWidth, 0, 0);
        bufferMeshVertices[2] = new Vector3(0, 0, ReserveHeight);
        bufferMeshVertices[3] = new Vector3(ReserveWidth, 0, ReserveHeight);

        bufferMesh.vertices = bufferMeshVertices;
        bufferMesh.triangles = new int[]
        {
            2, 1, 0,
            1, 2, 3
        };
        bufferMesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        bufferMesh.RecalculateNormals();

        BufferTexture = new Texture2D(ReserveWidth, ReserveHeight);
        BufferCenterTexture = new Texture2D(ReserveWidth, ReserveHeight);
        for (int i = 0; i < ReserveWidth; ++i)
        {
            for (int j = 0; j < ReserveWidth; ++j)
            {
                BufferTexture.SetPixel(i, j, new Color(0, 0, 0, 0));
                BufferCenterTexture.SetPixel(i, j, new Color(0, 0, 0, 0));
            }
        }
        
        BufferTexture.filterMode = FilterMode.Point;
        BufferTexture.wrapMode = TextureWrapMode.Repeat;
        BufferCenterTexture.filterMode = FilterMode.Point;
        BufferCenterTexture.wrapMode = TextureWrapMode.Repeat;

        BufferTexture.Apply();
        BufferCenterTexture.Apply();
        Material bufferMaterial = bufferGameObject.GetComponent<MeshRenderer>().material;
        bufferMaterial.SetTexture("_MainTex", BufferTexture);
        bufferMaterial.SetTexture("_CenterTex", BufferCenterTexture);
        bufferGameObject.GetComponent<MeshRenderer>().sortingOrder = 4;


        // add texture to material
        TilemapTexture.Apply();
        TilemapRenderer renderer = Tilemap.GetComponent<TilemapRenderer>();
        renderer.sharedMaterial.SetTexture("_GridInfoTex", TilemapTexture);
        Tilemap.GetComponent<TilemapRenderer>().sharedMaterial.SetVector("_GridTexDim", new Vector2(ReserveWidth, ReserveHeight));
        HighlightedTiles = new List<Vector3Int>();

        for (int i = 0; i < ReserveWidth; ++i)
        {
            for (int j = 0; j < ReserveHeight; ++j)
            {
                ApplyChangeToTilemapTexture(new Vector3Int(i, j, 0));
            }
        }
    }

    public SerializedTilemap SerializedTilemap()
    {
        // Debug.Log("Serialize " + this.tilemap.name);
        // temporarily make new list until full implementation
        return new SerializedTilemap("Tilemap", TileDataGrid, ReserveWidth, ReserveHeight, LiquidbodyController.Instance.liquidBodies);
    }
    #endregion

    #region Tile Accessors

    /// <summary>
    /// Method to access the TileDataGrid without confusion due to 2d array behaviour.
    /// </summary>
    /// <param name="tilePosition">Vector of tile position.</param>
    /// <returns>Tile data at that position.</returns>
    public TileData GetTileData(Vector3Int tilePosition)
    {
        if (IsWithinGridBounds(tilePosition)) {
            TileData td = TileDataGrid[tilePosition.y, tilePosition.x];

            if (td != null)
                return td;
        }

        return null;
    }

    public void SetTile(Vector3Int tilePosition, GameTile tile, bool godmode = false)
    {
        TileData tileData = GetTileData(tilePosition);

        if (godmode)
        {
            if (IsWithinGridBounds(tilePosition))
            {
                if (tile.type == TileType.Liquid)
                {
                    // TODO: liquid implementation
                    //tileData.PreviewLiquidBody(MergeLiquidBodies(tilePosition, tile));
                    LiquidbodyController.Instance.AddLiquidContentsAt(tilePosition, tile.defaultContents);
                }
                if (tile.type != TileType.Wall)
                    TileDataGrid[tilePosition.y, tilePosition.x].isTilePlaceable = true;
                tileData.PreviewReplacement(tile);
                ChangedTiles.Add(tilePosition);
                ApplyChangeToTilemapTexture(tilePosition);
            }
            else if ((tilePosition.x >= ReserveWidth && tilePosition.y > 0) ||
                (tilePosition.x > 0 && tilePosition.y >= ReserveHeight))
            {
                // if it isn't within grid bounds and is bigger than the current reserve size
                // change the array such that it will now contain the new tile
                TileData[,] TileDataGridCopy = TileDataGrid.Clone() as TileData[,];

                // get new maximum boundaries
                int maxWidth = tilePosition.x >= ReserveWidth ? tilePosition.x + 1 : ReserveWidth;
                int maxHeight = tilePosition.y >= ReserveHeight ? tilePosition.y + 1 : ReserveHeight;
                // create the new array and texture
                TileDataGrid = new TileData[maxHeight, maxWidth];
                Texture2D newTilemapTexture = new Texture2D(maxWidth + 1, maxHeight + 1);

                // fill the array and texture with values
                for (int i = 0; i < TileDataGrid.GetLength(1); ++i)
                {
                    for (int j = 0; j < TileDataGrid.GetLength(0); ++j)
                    {
                        if (i < ReserveWidth && j < ReserveHeight)
                        {
                            TileDataGrid[j, i] = TileDataGridCopy[j, i];
                            newTilemapTexture.SetPixel(i, j, TilemapTexture.GetPixel(i, j));
                        }
                        else
                        {
                            TileDataGrid[j, i] = new TileData(new Vector3Int(i, j, 0));
                            newTilemapTexture.SetPixel(i, j, new Color(0, 0, 0, 0));
                        }
                    }
                }

                // update old values
                ReserveWidth = maxWidth;
                ReserveHeight = maxHeight;
                TileDataGrid[tilePosition.y, tilePosition.x] = new TileData(tilePosition);
                // set placeable here, but idk what to do lol
                if (tile.type != TileType.Wall)
                    TileDataGrid[tilePosition.y, tilePosition.x].isTilePlaceable = true;

                tileData = GetTileData(tilePosition);

                // add texture to the buffer
                newTilemapTexture.filterMode = FilterMode.Point;
                newTilemapTexture.wrapMode = TextureWrapMode.Repeat;
                newTilemapTexture.Apply();
                TilemapTexture = newTilemapTexture;

                TilemapRenderer renderer = Tilemap.GetComponent<TilemapRenderer>();
                renderer.sharedMaterial.SetTexture("_GridInfoTex", TilemapTexture);
                Tilemap.GetComponent<TilemapRenderer>().sharedMaterial.SetVector("_GridTexDim", new Vector2(ReserveWidth, ReserveHeight));

                if (tile.type == TileType.Liquid)
                    // tileData.PreviewLiquidBody(MergeLiquidBodies(tilePosition, tile));
                    LiquidbodyController.Instance.AddLiquidContentsAt(tilePosition, tile.defaultContents, false);
                else
                    LiquidbodyController.Instance.RemoveLiquidContentsFromLiquidbodyAt(tilePosition);
                tileData.PreviewReplacement(tile);
                ChangedTiles.Add(tilePosition);
                ApplyChangeToTilemapTexture(tilePosition);
            }
        }
        else
        {
            if (tileData != null && (tileData.isTilePlaceable || GameManager.Instance.LevelData.WallBreakable))
            {
                if (tile.type == TileType.Liquid)
                {
                    // tileData.PreviewLiquidBody(MergeLiquidBodies(tilePosition, tile));
                    // tileData.contents = tile.defaultContents;
                    LiquidbodyController.Instance.AddLiquidContentsAt(tilePosition, tile.defaultContents);
                }
                tileData.PreviewReplacement(tile);
                ChangedTiles.Add(tilePosition);
                ApplyChangeToTilemapTexture(tilePosition);
            }
        }
    }
    public void RemoveTile(Vector3Int tilePosition)
    {
        TileData tileData = GetTileData(tilePosition);

        if (tileData != null)
        {
            RemovedTiles.Add(tilePosition);

            // resize the array if row/column is empty and is in outer ranges (may work on later)

            tileData.isTilePlaceable = false;
            if (tileData.currentTile.type == TileType.Liquid)
            {
                //DivideLiquidBody(tilePosition);
                LiquidbodyController.Instance.RemoveLiquidContentsFromLiquidbodyAt(tilePosition);
            }
            tileData.PreviewReplacement(null);
            ChangedTiles.Add(tilePosition);
            ApplyChangeToTilemapTexture(tilePosition);
        }
    }


    /// <summary>
    /// Returns TerrainTile(inherited from Tilebase) at given location of a cell within the Grid.
    /// </summary>
    /// <param name="cellLocation"> Position of the cell. </param>
    /// <returns></returns>
    public GameTile GetGameTileAt(Vector3Int cellLocation)
    {
        if (IsWithinGridBounds(cellLocation))
            return TileDataGrid[cellLocation.y, cellLocation.x].currentTile;//Tilemap.GetTile<GameTile>(cellLocation); removed because tilemaps cannot handle that many tiles

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
        return Tilemap.GetTile(cellLocation) == tile;
    }

    public void AddFoodReferenceToTile(Vector3Int gridPosition, Vector2Int size, GameObject foodSource)
    {
        Vector3Int pos;
        // Check if the whole object is in bounds
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                pos = gridPosition;
                pos.x += x;
                pos.y += y;

                if (GetTileData(pos) != null)
                    GetTileData(pos).Food = foodSource;
            }
        }
    }

    // Remove the food source on this tile, a little inefficient
    public void RemoveFoodReference(Vector3Int gridPosition)
    {
        Vector3Int pos = gridPosition;
        TileData tileData = GetTileData(pos);
        if (tileData == null) return;
        GameObject foodSource = GetTileData(pos).Food;
        if (!foodSource) return;

        Vector2Int size = foodSource.GetComponent<FoodSource>().Species.Size;

        for (int dx = -size.x; dx < size.x; dx++)
        {
            for (int dy = -size.y; dy < size.y; dy++)
            {
                TileData cell = GetTileData(new Vector3Int(pos.x + dx, pos.y + dy, 0));
                if (cell.Food && ReferenceEquals(cell.Food, foodSource))
                {
                    cell.Food = null;
                }
            }
        }
    }
    #endregion

    #region Drafting Functions
    public void ToggleDrafting()
    {
        if (!IsDrafting)
        {
            StartDrafting();
        }
        else
        {
            FinishDrafting();
        }
    }

    public void StartDrafting()
    {
        IsDrafting = true;
        GameManager.Instance.TryToPause("GridSystemDrafting");
        GameManager.Instance.m_inspector.ResetSelection();
        UpdateUI(false);
    }

    public void FinishDrafting()
    {
        IsDrafting = false;
        GameManager.Instance.TryToUnpause("GridSystemDrafting");
        UpdateUI(true);
    }

    private void UpdateUI(bool onOff)
    {
        GameManager.Instance.m_playerController.CanUseIngameControls = onOff;
        NextDayButton.interactable = onOff;
    }
    #endregion

    #region Building Functions
    public void CreateUnitBuffer(Vector2Int pos, int timeToComplete, ConstructionCluster.ConstructionType constructionType, int progress = -1)
    {
        if (timeToComplete <= 0)
        {
            Debug.LogError("Time to complete cannot be a negative value!");
            return;
        }

        // find the total cluster that are adjacent
        List<ConstructionCluster> adjacentClusters = new List<ConstructionCluster>();
        foreach (ConstructionCluster cluster in ConstructionClusters)
        {
            if (cluster.IsTileAdjacent(pos))
                adjacentClusters.Add(cluster);
        }
        
        switch (adjacentClusters.Count())
        {
            case 0:
                // nothing adjacent, make a new cluster
                ConstructionCluster newCluster = new ConstructionCluster(pos, timeToComplete, constructionType);
                ConstructionClusters.Add(newCluster);
                BufferCenterTexture.SetPixel(newCluster.CenterPosition.x, newCluster.CenterPosition.y, new Color(0, 0, 0, 1));
                break;
            case 1:
                // check if the timing and type is the same (tile), and if so add it
                if (adjacentClusters[0].IsMatching(timeToComplete, constructionType) && constructionType == ConstructionCluster.ConstructionType.TILE)
                {
                    // remove the original center
                    Vector2Int clusterCenter = adjacentClusters[0].CenterPosition;
                    BufferCenterTexture.SetPixel(clusterCenter.x, clusterCenter.y, new Color(0, 0, 0, 0));

                    // add the tile to the cluster
                    adjacentClusters[0].AddTilePosition(pos);

                    // update the new center
                    clusterCenter = adjacentClusters[0].CenterPosition;
                    BufferCenterTexture.SetPixel(clusterCenter.x, clusterCenter.y, new Color(0, 0, 0, 1));
                }
                else
                {
                    newCluster = new ConstructionCluster(pos, timeToComplete, constructionType);
                    ConstructionClusters.Add(newCluster);
                    BufferCenterTexture.SetPixel(newCluster.CenterPosition.x, newCluster.CenterPosition.y, new Color(0, 0, 0, 1));
                }
                break;
            default:
                // remove any clusters that aren't matching
                List<ConstructionCluster> matchingClusters = new List<ConstructionCluster>();
                foreach (ConstructionCluster cluster in adjacentClusters)
                {
                    if (cluster.IsMatching(timeToComplete, constructionType) && constructionType == ConstructionCluster.ConstructionType.TILE)
                        matchingClusters.Add(cluster);
                }

                // merge the rest
                if (matchingClusters.Count() != 0)
                {
                    ConstructionCluster sourceCluster = matchingClusters[0];
                    matchingClusters.Remove(sourceCluster);
                    Vector2Int clusterCenter = sourceCluster.CenterPosition;
                    BufferCenterTexture.SetPixel(clusterCenter.x, clusterCenter.y, new Color(0, 0, 0, 0));

                    foreach (ConstructionCluster cluster in matchingClusters)
                    {
                        // merge into source
                        sourceCluster.MergeCluster(cluster);
                        // remove from overall
                        ConstructionClusters.Remove(cluster);
                        // remove the flag for the center
                        clusterCenter = cluster.CenterPosition;
                        BufferCenterTexture.SetPixel(clusterCenter.x, clusterCenter.y, new Color(0, 0, 0, 0));
                    }

                    // finally add the tile
                    sourceCluster.AddTilePosition(pos);

                    // update the new center
                    clusterCenter = sourceCluster.CenterPosition;
                    BufferCenterTexture.SetPixel(clusterCenter.x, clusterCenter.y, new Color(0, 0, 0, 1));
                }
                else
                {
                    newCluster = new ConstructionCluster(pos, timeToComplete, constructionType);
                    ConstructionClusters.Add(newCluster);
                    BufferCenterTexture.SetPixel(newCluster.CenterPosition.x, newCluster.CenterPosition.y, new Color(0, 0, 0, 1));
                }
                break;
        }

        // update buffer texture
        Color bufferColorInformation = new Color(
                (float)((int)constructionType) / FLAG_VALUE_MULTIPLIER,       // which construction type it is
                0,
                0,                                                          // the progress towards target
                (float)timeToComplete / FLAG_VALUE_MULTIPLIER                         // the total time
            );
        BufferTexture.SetPixel(pos.x, pos.y, bufferColorInformation);
        BufferTexture.Apply();
        BufferCenterTexture.Apply();
        Material bufferMaterial = bufferGameObject.GetComponent<MeshRenderer>().material;
        bufferMaterial.SetTexture("_MainTex", BufferTexture);
        bufferMaterial.SetTexture("_CenterTex", BufferCenterTexture);

        TileDataGrid[pos.y, pos.x].isConstructing = true;
    }
    public void ConstructionFinishedCallback(Action action)
    {
        constructionFinishedCallback += action;
    }
    public void CreateRectangleBuffer(Vector2Int pos, int time, Vector2Int size, ConstructionCluster.ConstructionType type, int buildProgress = 0)
    {
        // only considering non food sources right now
        if (type != ConstructionCluster.ConstructionType.TILE)
        {
            // first find all positions
            List<Vector2Int> bufferPositions = new List<Vector2Int>();
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Vector2Int bufferPosition = new Vector2Int(pos.x + i, pos.y + j);

                    TileDataGrid[bufferPosition.y, bufferPosition.x].isConstructing = true;

                    Color bufferColorInformation = new Color(
                        (float)((int)type) / FLAG_VALUE_MULTIPLIER,                 // which construction type it is
                        0,                                                          // (currently not being used)
                        (float)buildProgress / FLAG_VALUE_MULTIPLIER,               // the progress towards target
                        (float)time / FLAG_VALUE_MULTIPLIER                         // the total time
                    );


                    Color bufferColorExtraInformation = new Color(
                        (float)(i + j * size.x + 1) / FLAG_VALUE_MULTIPLIER,        // which number tile it is [1, size.x * size.y]
                        (float) size.x / FLAG_VALUE_MULTIPLIER,                     // food width
                        (float) size.y / FLAG_VALUE_MULTIPLIER,                     // food height
                        0
                    ); 

                    BufferTexture.SetPixel(bufferPosition.x, bufferPosition.y, bufferColorInformation);
                    BufferCenterTexture.SetPixel(bufferPosition.x, bufferPosition.y, bufferColorExtraInformation);
                    bufferPositions.Add(bufferPosition);
                }
            }

            // then actually create the buffer, regardless of where it actually is
            ConstructionCluster newCluster = new ConstructionCluster(bufferPositions, time, type, buildProgress);
            ConstructionClusters.Add(newCluster);
        }

        BufferTexture.Apply();
        BufferCenterTexture.Apply();
        Material bufferMaterial = bufferGameObject.GetComponent<MeshRenderer>().material;
        bufferMaterial.SetTexture("_MainTex", BufferTexture);
        bufferMaterial.SetTexture("_CenterTex", BufferCenterTexture);
    }

    public void RemoveBuffer(Vector2Int pos, int sizeX = 1, int sizeY = 1)
    {
        if (!TileDataGrid[pos.y, pos.x].isConstructing)
        {
            Debug.Log("No construction buffer to remove, proceeding");
            return;
        }
        List<Vector3Int> changedTiles = new List<Vector3Int>();
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                Vector2Int removeBufferPosition = new Vector2Int(pos.x + i, pos.y + j);

                // if the cluster is empty then remove it
                ConstructionCluster clusterToRemove = null;
                foreach (ConstructionCluster cluster in ConstructionClusters)
                {
                    if (cluster.ConstructionTilePositions.Contains(removeBufferPosition))
                    {
                        // remove original center
                        BufferCenterTexture.SetPixel(cluster.CenterPosition.x, cluster.CenterPosition.y, new Color(0, 0, 0, 0));
                        cluster.RemoveTilePosition(removeBufferPosition);
                        // set the new center
                        BufferCenterTexture.SetPixel(cluster.CenterPosition.x, cluster.CenterPosition.y, new Color(0, 0, 0, 1));
                        if (cluster.ConstructionTilePositions.Count <= 0)
                            clusterToRemove = cluster;
                    }
                }

                // specifically for large food sources
                if (sizeX > 1 || sizeY > 1)
                    BufferCenterTexture.SetPixel(removeBufferPosition.x, removeBufferPosition.y, new Color(0, 0, 0, 0));

                if (clusterToRemove != null)
                {
                    BufferCenterTexture.SetPixel(clusterToRemove.CenterPosition.x, clusterToRemove.CenterPosition.y, new Color(0, 0, 0, 0));
                    ConstructionClusters.Remove(clusterToRemove);
                }

                // update the buffer texture
                BufferTexture.SetPixel(removeBufferPosition.x, removeBufferPosition.y, new Color(0, 0, 0, 0));
                BufferTexture.Apply();
                BufferCenterTexture.Apply();
                Material bufferMaterial = bufferGameObject.GetComponent<MeshRenderer>().material;
                bufferMaterial.SetTexture("_MainTex", BufferTexture);
                bufferMaterial.SetTexture("_CenterTex", BufferCenterTexture);

                TileDataGrid[removeBufferPosition.y, removeBufferPosition.x].isConstructing = false;

                changedTiles.Add((Vector3Int)removeBufferPosition);
            }
        }

        //Report updates to RPM
        if (changedTiles.Count > 0)
        {
            GameManager.Instance.m_reservePartitionManager.UpdateAccessMapChangedAt(changedTiles);
        }
    }
    public void CountDown()
    {
        List<Vector3Int> changedTiles = new List<Vector3Int>();

        List<ConstructionCluster> finishedClusters = new List<ConstructionCluster>();

        foreach (ConstructionCluster cluster in ConstructionClusters)
        {
            // do the countdown
            cluster.CountDown();

            if (cluster.IsFinished())
            {
                finishedClusters.Add(cluster);
                // remove the information from textures
                foreach (Vector2Int bufferPosition in cluster.ConstructionTilePositions)
                {
                    changedTiles.Add((Vector3Int)bufferPosition);
                    BufferTexture.SetPixel(bufferPosition.x, bufferPosition.y, new Color(0, 0, 0, 0));
                    BufferCenterTexture.SetPixel(bufferPosition.x, bufferPosition.y, new Color(0, 0, 0, 0));
                    TileDataGrid[bufferPosition.y, bufferPosition.x].isConstructing = false;

                    LiquidbodyController.Instance.RemoveLiquidContentsFromLiquidbodyAt((Vector3Int)bufferPosition);
                }

                LiquidbodyController.Instance.MergeConstructingTiles();

                if (constructionFinishedCallback != null)
                    constructionFinishedCallback();
            }
            else
            {
                // update the textures
                foreach (Vector2Int tilePosition in cluster.ConstructionTilePositions)
                {
                    Color bufferColorInformation = BufferTexture.GetPixel(tilePosition.x, tilePosition.y);
                    bufferColorInformation.b = (bufferColorInformation.b * FLAG_VALUE_MULTIPLIER + 1) / FLAG_VALUE_MULTIPLIER;
                    BufferTexture.SetPixel(tilePosition.x, tilePosition.y, bufferColorInformation);
                }
            }
        }

        foreach (ConstructionCluster cluster in finishedClusters)
            ConstructionClusters.Remove(cluster);

        BufferTexture.Apply();
        BufferCenterTexture.Apply();
        Material bufferMaterial = bufferGameObject.GetComponent<MeshRenderer>().material;
        bufferMaterial.SetTexture("_MainTex", BufferTexture);
        bufferMaterial.SetTexture("_CenterTex", BufferCenterTexture);

        //Report updates to RPM
        if (changedTiles.Count > 0)
        {
            GameManager.Instance.m_reservePartitionManager.UpdateAccessMapChangedAt(changedTiles);
        }
    }
    #endregion

    // no idea either
    private void ClearAll()
    {
        this.RemovedTiles = new HashSet<Vector3Int>();
        this.ChangedTiles = new HashSet<Vector3Int>();
        this.previewBodies = new List<LiquidBody>();
    }
    public void ApplyChangeToTilemapTexture(Vector3Int tilePosition)
    {
        TileData data = GetTileData(tilePosition);

        if (data == null || data.currentTile == null)
        {
            Color tilePixel = TilemapTexture.GetPixel(tilePosition.x, tilePosition.y);
            tilePixel.r = 0;
            TilemapTexture.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
            TilemapTexture.Apply();

            Tilemap.SetTile(tilePosition, null);
        }
        else
        {
            Color tilePixel = TilemapTexture.GetPixel(tilePosition.x, tilePosition.y);
            tilePixel.r = ((int)data.currentTile.type + 1) / FLAG_VALUE_MULTIPLIER;
            TilemapTexture.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
            TilemapTexture.Apply();

            Tilemap.SetTile(tilePosition, data.currentTile);
            Tilemap.SetTileFlags(tilePosition, TileFlags.None);
        }
    }

    #region Traversal Methods
    // TODO not performing as expected drawing left to right, also does not repathfind when stuck. 
    private bool QuickContinuityTest(Vector3Int origin, Vector3Int start, Vector3Int stop, CellStatus[,] historyArray) 
    {
        int iterations = 0;
        int maxDisplacement = this.quickCheckIterations / 2 - 1; // TODO optimization, when trying to access outside possible bound, terminate immediately
        int[] displacement = new int[2] { start.x - origin.x, start.y - origin.y };
        //Debug.Log("Starting point: " + displacement[0].ToString() + displacement[1].ToString());
        int[] targetDisplacement = new int[2] { stop.x - origin.x, stop.y - origin.y };
        TranslateDisplacementToArray(displacement, historyArray);
        while (iterations < this.quickCheckIterations)
        {
            if (displacement[0] == targetDisplacement[0] && displacement[1] == targetDisplacement[1]) // Success
            {
                return true;
            }
            // Try move towards target
            Direction2D direction = Mathf.Abs(displacement[0]) < Mathf.Abs(displacement[1]) ? Direction2D.X : Direction2D.Y; // Move to direction with smaller displacement
            // TODO refine path finding, repathfind in a different order to avoid dead ends if steps not depleted, especially change the order of the parallel movements.
            //Move Towards target direction
            if (CanMove(displacement, targetDisplacement, direction, historyArray, MoveType.Towards) || CanMove(displacement, targetDisplacement, SwitchDirection(direction), historyArray, MoveType.Towards))
            {
                iterations++;
                continue;
            }
            //Move in parallel
            if (CanMove(displacement, targetDisplacement, direction, historyArray, MoveType.Parallel1) || CanMove(displacement, targetDisplacement, SwitchDirection(direction), historyArray, MoveType.Parallel1))
            {
                iterations++;
                continue;
            }
            //Move Away from target direction
            if (CanMove(displacement, targetDisplacement, direction, historyArray, MoveType.Away) || CanMove(displacement, targetDisplacement, SwitchDirection(direction), historyArray, MoveType.Away))
            {
                iterations++;
                continue;
            }
            //Debug.Log("stuck");
            return false; //Stuck in somewhere
        }
        //Debug.Log("max iterations");
        return false;
    }
    private bool CanMove(int[] displacement, int[] targetDisplacement, Direction2D direction, CellStatus[,] historyArray, MoveType moveType)
    {
        int adder;
        switch (moveType)
        {
            case MoveType.Towards:
                adder = PathfindTowards(displacement[(int)direction], targetDisplacement[(int)direction]);
                break;
            case MoveType.Away:
                adder = -PathfindTowards(displacement[(int)direction], targetDisplacement[(int)direction]);
                break;
            case MoveType.Parallel1:
                adder = 1;
                break;
            default:
                adder = -1;
                break;
        }
        if (adder == 0)
        {
            return false;
        }
        int[] newDisp = new int[2] { displacement[0], displacement[1] };
        newDisp[(int)direction] += adder;
        if (TranslateDisplacementToArray(newDisp, historyArray))
        {
            displacement[(int)direction] += adder;
            //Debug.Log("passed");
            return true;
        }
        if (moveType == MoveType.Parallel1)
        {
            return CanMove(displacement, targetDisplacement, direction, historyArray, MoveType.Parallel2);
        }
        return false;
    }
    private int PathfindTowards(int start, int end)
    {
        if (start > end)
        {
            return -1;
        }
        if (start < end)
        {
            return 1;
        }
        return 0;
    }
    /// <summary>
    /// If successfully updated history array, then returns true, otherwise returns false
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="cellPosition"></param>
    /// <param name="historyArray"></param>
    /// <returns></returns>
    private bool TranslateDisplacementToArray(int[] displacement, CellStatus[,] historyArray)
    {
        int maxDisplacement = quickCheckIterations / 2 - 1;
        int x = displacement[0] + maxDisplacement;
        int y = displacement[1] + maxDisplacement;
        if (x < 0 || x >= quickCheckIterations - 1 || y < 0 || y >= quickCheckIterations - 1) // Out of bound
        {
            return false;
        }
        if (historyArray[x, y] != CellStatus.Self)
        {
            return false;
        }
        historyArray[x, y] = CellStatus.Walked;
        return true;
    }

    private void ResetHistoryArray(CellStatus[,] historyArray)
    {
        for (int i = 0; i < quickCheckIterations - 1; i++)
        {
            for (int j = 0; j < quickCheckIterations - 1; j++)
            {
                if (historyArray[i, j] == CellStatus.Walked)
                {
                    historyArray[i, j] = CellStatus.Self;
                }
            }
        }
    }
    private CellStatus[,] InitializeHistoryArray(Vector3Int originPosition, HashSet<Vector3Int> selfTiles)
    {
        CellStatus[,] historyArray = new CellStatus[quickCheckIterations - 1, quickCheckIterations - 1];
        Vector3Int startPoint = new Vector3Int(originPosition.x - quickCheckIterations / 2 + 1, originPosition.y - quickCheckIterations / 2 + 1, originPosition.z);
        Vector3Int cellToScan = new Vector3Int(startPoint.x, startPoint.y, startPoint.z);
        for (int i = 0; i < quickCheckIterations - 1; i++)
        {
            cellToScan.x = startPoint.x;
            cellToScan.x += i;
            for (int j = 0; j < quickCheckIterations - 1; j++)
            {
                cellToScan.y = startPoint.y;
                cellToScan.y += j;
                if (selfTiles.Contains(cellToScan))
                {
                    historyArray[i, j] = CellStatus.Self;
                }
            }
        }
        return historyArray;
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
        Color tilePixel = TilemapTexture.GetPixel(tilePosition.x, tilePosition.y);

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

        tilemap.SetColor(tilePosition, color);

        tilePixel.a = (float)alphaBitMask / FLAG_VALUE_MULTIPLIER;
        TilemapTexture.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
        TilemapTexture.Apply();

        // add to propertyblock
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        renderer.sharedMaterial.SetTexture("_GridInfoTex", TilemapTexture);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tilemap">Tilemap to be affected (mostly for terrain).</param>
    /// <param name="tilePosition">Position of tile in grid space.</param>
    /// <param name="flag">Compiled flags to be set.</param>
    public void RemoveFlagsFromTileTexture(Tilemap tilemap, Vector3Int tilePosition, TileFlag flag)
    {
        Color tilePixel = TilemapTexture.GetPixel(tilePosition.x, tilePosition.y);

        int alphaBitMask = (int)(tilePixel.a * FLAG_VALUE_MULTIPLIER);
        int flagInt = (int)flag;

        // return if flag is not there
        if (alphaBitMask == 0 || alphaBitMask % flagInt != 0)
            return;

        alphaBitMask /= flagInt;

        if (flag == TileFlag.HIGHLIGHT_FLAG)
            tilemap.SetColor(tilePosition, Color.white);

        tilePixel.a = (float)alphaBitMask / FLAG_VALUE_MULTIPLIER;
        TilemapTexture.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
        TilemapTexture.Apply();

        // add to propertyblock
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        renderer.sharedMaterial.SetTexture("_GridInfoTex", TilemapTexture);
    }
    public void ToggleGridOverlay()
    {
        // toggle using shader here
        float currentToggleValue = Tilemap.GetComponent<TilemapRenderer>().sharedMaterial.GetFloat("_GridOverlayToggle");
        SetGridOverlay(!(Mathf.RoundToInt(currentToggleValue) == 0));
    }
    public void SetGridOverlay(bool overlay)
    {
        Material tilemapMaterial = Tilemap.GetComponent<TilemapRenderer>().sharedMaterial;
        // Set the float on the shared material based on the bool passed in
        if (overlay) tilemapMaterial.SetFloat("_GridOverlayToggle", 1);
        else tilemapMaterial.SetFloat("_GridOverlayToggle", 0);
    }

    public void ClearHighlights()
    {
        foreach (Vector3Int tilePosition in HighlightedTiles)
            RemoveFlagsFromTileTexture(Tilemap, tilePosition, TileFlag.HIGHLIGHT_FLAG);

        HighlightedTiles.Clear();
    }

    public void HighlightTile(Vector3Int tilePosition, Color color)
    {
        if (!HighlightedTiles.Contains(tilePosition))
        {
            HighlightedTiles.Add(tilePosition);
            
            ApplyFlagToTileTexture(Tilemap, tilePosition, TileFlag.HIGHLIGHT_FLAG, color);
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
                if (Mathf.Abs(loopedPosition.x - tilePosition.x) + Mathf.Abs(loopedPosition.y - tilePosition.y) <= radius)
                    HighlightTile(loopedPosition, color);
            }
        }
    }
    #endregion

    #region Boolean Methods and Validation

    public bool IsPodPlacementValid(Vector3 mousePosition, AnimalSpecies species)
    {
        Vector3Int gridPosition = WorldToCell(mousePosition);
        return this.CheckSurroundingTerrain(gridPosition, species);
    }

    public bool IsFoodPlacementValid(Vector3 mousePosition, Item selectedItem = null, FoodSourceSpecies species = null)
    {
        if (selectedItem)
            species = GameManager.Instance.FoodSources[selectedItem.ID];

        Vector3Int gridPosition = WorldToCell(mousePosition);
        return CheckSurroundingTiles(gridPosition, species);
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

    private bool CheckSurroundingTiles(Vector3Int cellPosition, FoodSourceSpecies species)
    {
        Vector3Int pos;
        bool isValid = true;
        // Size is even, offset by 1
        // Check if the whole object is in bounds
        //print(Mathf.CeilToInt(-(float)species.Size.x / 2));

        for (int x = Mathf.CeilToInt(-(float)(species.Size.x - 1) / 2); x <= species.Size.x / 2; x++)
        {
            for (int y = Mathf.CeilToInt(-(float)(species.Size.y - 1) / 2); y <= species.Size.y / 2; y++)
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
        TileData tileData = GetTileData(pos);
        if (tileData == null || tileData.Food)
        {
            return false;
        }
        if (IsOnWall(pos)) return false;
        if (IsConstructing(pos.x, pos.y))
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

    public bool IsCellinGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < ReserveWidth && y < ReserveHeight;
    }

    public bool IsWithinGridBounds(Vector3 mousePosition)
    {
        Vector3Int loc = new Vector3Int((int)mousePosition.x, (int)mousePosition.y, (int)mousePosition.z);//Grid.WorldToCell(mousePosition); old implementation that causes stack overflow

        return IsCellinGrid(loc.x, loc.y);
    }
    #endregion

    // figure out if this still works
    public void UpdateAnimalCellGrid()
    {
        for (int i = 0; i < this.TileDataGrid.GetLength(0); i++)
        {
            for (int j = 0; j < this.TileDataGrid.GetLength(1); j++)
            {
                if (this.TileDataGrid[i, j] != null)
                {
                    this.TileDataGrid[i, j].Animal = null;
                }
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
                GetTileData(animalLocation).Animal = animal;
            }
        }
    }

    // this too
    public void updateVisualPlacement(Vector3Int gridPosition, Item selectedItem)
    {
        if (GameManager.Instance.FoodSources.ContainsKey(selectedItem.ID))
        {
            FoodSourceSpecies species = GameManager.Instance.FoodSources[selectedItem.ID];
            CheckSurroundingTiles(gridPosition, species);
        }
        else if (GameManager.Instance.AnimalSpecies.ContainsKey(selectedItem.ID))
        {
            AnimalSpecies species = GameManager.Instance.AnimalSpecies[selectedItem.ID];
            CheckSurroundingTerrain(gridPosition, species);
        }
        else
        {
            // TODO figure out how to determine if tile is placable
            HighlightTile(gridPosition, Color.green);
        }
    }

    #region Tile Utility and Calculations
    public Vector3Int GetReserveDimensions()
    {
        return new Vector3Int(ReserveWidth, ReserveHeight, 0);
    }


    public Vector3Int FindClosestLiquidSource(Population population, GameObject animal)
    {
        Vector3Int itemLocation = new Vector3Int(-1, -1, -1);
        float closestDistance = 10000f;
        float localDistance = 0f;
        for (int x = 0; x < ReserveWidth; x++)
        {
            for (int y = 0; y < ReserveHeight; y++)
            {
                // if contains liquid tile, check neighbors accessibility
                GameTile tile = GetGameTileAt(new Vector3Int(x, y, 0));
                if (tile != null && tile.type == TileType.Liquid)
                {
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
        for (int x = 0; x < ReserveWidth; x++)
        {
            for (int y = 0; y < ReserveHeight; y++)
            {
                tileGrid[x, y] = RPM.CanAccess(population, new Vector3Int(x, y, 0));
            }
        }
        return new AnimalPathfinding.Grid(tileGrid, this.Grid);
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
                    if (LiquidbodyController.Instance.GetLiquidContentsAt(centerCellLocation, out float[] liquidContents, out bool constructing))
                    {
                        compositionDistancePairs.Add(liquidContents, distance);
                    }
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
                        if (LiquidbodyController.Instance.GetLiquidContentsAt(centerCellLocation, out float[] liquidContents, out bool constructing))
                        {
                            if (!compositionDistancePairs.Keys.Contains(liquidContents))
                            {
                                compositionDistancePairs.Add(liquidContents, distance);
                            }
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
                        if (LiquidbodyController.Instance.GetLiquidContentsAt(centerCellLocation, out float[] liquidContents, out bool constructing))
                        {
                            if (!compositionDistancePairs.Keys.Contains(liquidContents))
                            {
                                compositionDistancePairs.Add(liquidContents, distance);
                            }
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

    public int[] CountOfTilesInArea(Vector3Int centerCellLocation, int size)
    {
        int[] typesOfTileWithinRadius = new int[(int)TileType.TypesOfTiles];
        int radius = size / 2;
        Vector3Int pos;
        int offset = 0;
        if (size % 2 == 0)
        {
            centerCellLocation = new Vector3Int(centerCellLocation.x - 1, centerCellLocation.y - 1, 0);
            offset = 1;
        }
        // Check if the whole object is in bounds
        for (int x = (-1) * (radius - offset); x <= radius; x++)
        {
            for (int y = (-1) * (radius - offset); y <= radius; y++)
            {
                pos = centerCellLocation;
                pos.x += x;
                pos.y += y;
                GameTile tile = GetGameTileAt(pos);
                if (tile)
                {
                    typesOfTileWithinRadius[(int)tile.type]++;
                }
            }
        }
        return typesOfTileWithinRadius;
    }

    public int[] CountOfTilesUnderSpecies(Vector3Int cellPosition, FoodSourceSpecies species)
    {
        int[] typesOfTileWithinRadius = new int[(int)TileType.TypesOfTiles];
        Vector3Int pos;
        int xOffset = 0;
        int yOffset = 0;

        if (species.Size.x % 2 == 0)
        {
            cellPosition = new Vector3Int(cellPosition.x - 1, cellPosition.y, 0);
            xOffset = 1;
        }

        if (species.Size.y % 2 == 0)
        {
            cellPosition = new Vector3Int(cellPosition.x, cellPosition.y - 1, 0);
            yOffset = 1;
        }

        for (int x = (-1) * (species.Size.x - xOffset - 1); x < species.Size.x; x++)
        {
            for (int y = (-1) * (species.Size.y - yOffset - 1); y < species.Size.y; y++)
            {
                pos = cellPosition;
                pos.x += x;
                pos.y += y;
                
                GameTile tile = GetGameTileAt(pos);
                if (tile)
                {
                    typesOfTileWithinRadius[(int)tile.type]++;
                }
            }
        }
        return typesOfTileWithinRadius;
    }

    /// <summary>
    /// Scan from all the liquid tiles within a given taxicab distance and return all different liquid compositions
    /// </summary>
    /// <param name="topRightCorner">The location of the top right corner of the foodSource</param> (cause for some reason that's how foodSources work)
    /// <param name="scanRange">The radius range to look for</param>
    /// <param name="scanRing">Scans the full circle when false. Scans only the outermost ring when true.</param>
    /// <returns>A list of the compositions, can have a length of 0</returns>
    public List<float[]> GetLiquidCompositionWithinRange(Vector3Int topRightCorner, Vector2Int foodSourceSize, int scanRange, bool scanRing = false)
    {
        List<float[]> liquidCompositions = new List<float[]>();

        Vector3Int scanLocation = new Vector3Int(0, 0, topRightCorner.z);
        foreach (int x in Range(-scanRange - (foodSourceSize.x - 1), scanRange))
        {
            foreach (int y in Range(-scanRange - (foodSourceSize.y - 1), scanRange))
            {
                int xDistance;
                if(x < 0)
                {
                    if(x < -(foodSourceSize.x - 1))
                        xDistance = Mathf.Abs(x) - (foodSourceSize.x - 1);
                    else
                        xDistance = 0;
                }
                else
                    xDistance = x;

                int yDistance;
                if(y < 0)
                {
                    if(y < -(foodSourceSize.y - 1))
                        yDistance = Mathf.Abs(y) - (foodSourceSize.y - 1);
                    else
                        yDistance = 0;
                }
                else
                    yDistance = y;

                if (xDistance + yDistance > scanRange)
                {
                    continue;
                }

                if(scanRing && (xDistance + yDistance) <= scanRange - 1)
                {
                    continue;
                }

                scanLocation.x = x + topRightCorner.x;
                scanLocation.y = y + topRightCorner.y;

                if (LiquidbodyController.Instance.GetLiquidContentsAt(scanLocation, out float[] liquidContents, out bool constructing))
                {
                    liquidCompositions.Add(liquidContents);
                }
            }
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
    #endregion

    #region Math Utility
    private Direction2D SwitchDirection(Direction2D direction)
    {
        return direction == Direction2D.X ? Direction2D.Y : Direction2D.X;
    }

    /// <summary>
    /// Convert a world position to cell positions on the grid.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return Grid.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int worldPosition)
    {
        return Grid.CellToWorld(worldPosition);
    }

    public ConstructionCluster GetConstructionClusterAtPosition(Vector3Int position)
    {
        TileData tile = GetTileData(position);
        if(!tile.isConstructing)
            return null;

        Vector2Int gridPosition = new Vector2Int(position.x, position.y);
        foreach(ConstructionCluster cluster in ConstructionClusters)
        {
            if(cluster.ConstructionTilePositions.Contains(gridPosition))
                return cluster;
        }

        return null;
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
    #endregion


    public class ConstructionCluster
    {
        public enum ConstructionType { TREE, ONEFOOD, TILE }
        public List<Vector2Int> ConstructionTilePositions { get; private set; }
        public Vector2Int CenterPosition { get; private set; }
        public ConstructionType type { get; private set; }
        public int targetDays { get; private set; }
        public int currentDays { get; private set; }

        public ConstructionCluster(Vector2Int tilePosition, int targetDays, ConstructionType type, int buildProgress = 0)
        {
            ConstructionTilePositions = new List<Vector2Int>();
            ConstructionTilePositions.Add(tilePosition);
            CenterPosition = tilePosition;
            this.targetDays = targetDays;
            this.type = type;
            currentDays = buildProgress;
        }

        public ConstructionCluster(List<Vector2Int> tilePositions, int targetDays, ConstructionType type, int buildProgress = 0)
        {
            ConstructionTilePositions = tilePositions; 
            CenterPosition = FindCenter();
            this.targetDays = targetDays;
            this.type = type;
            currentDays = buildProgress;
        }

        public void MergeCluster(ConstructionCluster cluster)
        {
            foreach (Vector2Int tilePosition in cluster.ConstructionTilePositions)
            {
                if (!ConstructionTilePositions.Contains(tilePosition))
                    ConstructionTilePositions.Add(tilePosition);
            }

            CenterPosition = FindCenter();
        }

        public void AddTilePosition(Vector2Int tilePosition)
        {
            if (!ConstructionTilePositions.Contains(tilePosition))
            {
                ConstructionTilePositions.Add(tilePosition);
                CenterPosition = FindCenter();
            }
        }

        public void RemoveTilePosition(Vector2Int tilePostion)
        {
            if (ConstructionTilePositions.Contains(tilePostion))
            {
                ConstructionTilePositions.Remove(tilePostion);
                CenterPosition = FindCenter();
            }
        }

        private Vector2Int FindCenter()
        {
            // first find complete center, possibly outside of the listed tiles
            Vector2 trueCenter = new Vector2(0, 0);
            foreach (Vector2Int constructionTilePosition in ConstructionTilePositions)
                trueCenter += (Vector2)constructionTilePosition;
            trueCenter /= ConstructionTilePositions.Count();

            // find tile in the list that is closest to the true center
            Vector2Int tileClosestToTrueCenter = new Vector2Int(-100, -100);
            foreach (Vector2Int constructionTilePosition in ConstructionTilePositions)
            {
                // if the distance from the closest tile right now is less than the new tile
                if (Vector2.Distance((Vector2)tileClosestToTrueCenter, trueCenter) > Vector2.Distance((Vector2)constructionTilePosition, trueCenter))
                    // change the center to that
                    tileClosestToTrueCenter = constructionTilePosition;
            }

            return tileClosestToTrueCenter;
        }

        public bool IsTileAdjacent(Vector2Int tilePosition)
        {
            foreach (Vector2Int constructionTilePosition in ConstructionTilePositions)
            {
                if (Mathf.Abs(tilePosition.x - constructionTilePosition.x) + Mathf.Abs(tilePosition.y - constructionTilePosition.y) == 1)
                    return true;
            }
            return false;
        }

        public bool IsMatching(int targetDays, ConstructionType type)   {   return this.targetDays == targetDays && currentDays == 0 && this.type == type;  }
        public bool IsFinished()                    {   return currentDays >= targetDays;  }
        public void CountDown()                     {   this.currentDays += 1;  }
    }
}