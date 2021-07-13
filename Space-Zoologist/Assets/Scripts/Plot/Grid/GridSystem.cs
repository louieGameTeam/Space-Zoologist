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
public delegate void BodyEmptyCallback(LiquidBody liquidBody);
public class GridSystem : MonoBehaviour
{
    public enum AbsoluteDirection { Towards, Against, Parallel }

    private enum Direction2D { X, Y }
    private enum CellStatus { Other, Self, Walked }
    private enum MoveType { Towards, Parallel1, Parallel2, Away }
    public bool holdsContent;
    public Tilemap tilemap;
    public Dictionary<Vector3Int, TileData> positionsToTileData { get; private set; } = new Dictionary<Vector3Int, TileData>();
    private HashSet<Vector3Int> ChangedTiles = new HashSet<Vector3Int>();
    public HashSet<LiquidBody> liquidBodies { get; private set; } = new HashSet<LiquidBody>();
    public List<LiquidBody> previewBodies { get; private set; } = new List<LiquidBody>();
    private HashSet<Vector3Int> RemovedTiles = new HashSet<Vector3Int>();
    private BodyEmptyCallback bodyEmptyCallback;
    [SerializeField] private int quickCheckIterations = 6; //Number of tiles to quick check, if can't reach another tile within this many walks, try to generate new body by performing full check
                                                           // Increment by 2 makes a difference. I.E. even numbers, at least 6 to account for any missing tile in 8 surrounding tiles

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
    [SerializeField] public Tilemap Tilemap;
    private BuildBufferManager buildBufferManager;
    [Header("Used to define 2d array")]
    [SerializeField] public int ReserveWidth = default;
    [SerializeField] public int ReserveHeight = default;
    public Vector3Int startTile = default;
    // Food and home locations updated when added, animal locations updated when the store opens up.
    public CellData[,] CellGrid = default;

    List<Vector3Int> HighlightedTiles;
    private Texture2D TilemapTexture;

    #region Monobehaviour Callbacks
    private void Awake()
    {
        // set up the information textures
        TilemapTexture = new Texture2D(ReserveWidth, ReserveHeight);
        // make black texture
        for (int i = 0; i < ReserveWidth; ++i)
        {
            for (int j = 0; j < ReserveHeight; ++j)
                TilemapTexture.SetPixel(i, j, new Color(0, 0, 0, 0));
        }
        TilemapTexture.filterMode = FilterMode.Point;
        TilemapTexture.wrapMode = TextureWrapMode.Repeat;
        TilemapTexture.Apply();

        TilemapRenderer renderer = Tilemap.GetComponent<TilemapRenderer>();
        renderer.sharedMaterial.SetTexture("_GridInformationTexture", TilemapTexture);
        Tilemap.GetComponent<TilemapRenderer>().sharedMaterial.SetVector("_GridTextureDimensions", new Vector2(ReserveWidth, ReserveHeight));
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

        // tilelayermanager stuff
        InitializeTileLayerManager();
    }

    private void InitializeTileLayerManager()
    {
        this.bodyEmptyCallback = OnLiquidBodyEmpty;
        this.tilemap = this.gameObject.GetComponent<Tilemap>();
        this.positionsToTileData = new Dictionary<Vector3Int, TileData>();
        this.liquidBodies = new HashSet<LiquidBody>();
        this.previewBodies = new List<LiquidBody>();
        this.RemovedTiles = new HashSet<Vector3Int>();
        this.tilemap.ClearAllTiles();
    }

    private void Start()
    {
        this.buildBufferManager = FindObjectOfType<BuildBufferManager>();

        // temporary to show effect
        ApplyFlagToTileTexture(Tilemap, new Vector3Int(1, 1, 0), TileFlag.LIQUID_FLAG, Color.clear);
        ApplyFlagToTileTexture(Tilemap, new Vector3Int(1, 2, 0), TileFlag.LIQUID_FLAG, Color.clear);

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

    public LiquidBody GetLiquidBodyAt(Vector3Int cellPosition)
    {
        if (positionsToTileData.ContainsKey(cellPosition))
        {
            return positionsToTileData[cellPosition].currentLiquidBody;
        }
        return null;
    }
    public void ChangeComposition(Vector3Int cellPosition, float[] contents)
    {
        if (!this.holdsContent)
        {
            Debug.LogError("This tilemap does not hold content");
            return;
        }
        if (positionsToTileData.ContainsKey(cellPosition))
        {
            contents.CopyTo(positionsToTileData[cellPosition].currentLiquidBody.contents, 0);
            return;
        }
        Debug.LogError(cellPosition + "Does not have tile present");

    }
    public void ParseSerializedTilemap(SerializedTilemap serializedTilemap, GameTile[] gameTiles)
    {
        InitializeTileLayerManager();
        Dictionary<int, LiquidBody> bodyIDsToLiquidBodies = new Dictionary<int, LiquidBody>();
        foreach (SerializedLiquidBody serializedLiquidBody in serializedTilemap.SerializedLiquidBodies)
        {
            LiquidBody liquidBody = ParseSerializedLiquidBody(serializedLiquidBody);
            bodyIDsToLiquidBodies.Add(liquidBody.bodyID, liquidBody);
            this.liquidBodies.Add(liquidBody);
        }
        Dictionary<string, GameTile> namesToGameTiles = new Dictionary<string, GameTile>();
        foreach (SerializedTileData serializedTileData in serializedTilemap.SerializedTileDatas)
        {
            // parse this data in properly
            /*
            if (!namesToGameTiles.ContainsKey(serializedTileData.TileName))
            {
                foreach (GameTile gameTile in gameTiles)
                {
                    if (gameTile.name.Equals(serializedTileData.TileName))
                    {
                        namesToGameTiles.Add(serializedTileData.TileName, gameTile);
                        break;
                    }
                }
            }
            TileData tileData = ParseSerializedTileData(serializedTileData, namesToGameTiles[serializedTileData.TileName], bodyIDsToLiquidBodies);
            this.positionsToTileData.Add(tileData.tilePosition, tileData);
            
            ApplyChangesToTilemap(tileData.tilePosition);
            */
        }
    }
    private LiquidBody ParseSerializedLiquidBody(SerializedLiquidBody serializedLiquidBody)
    {
        HashSet<Vector3Int> tiles = new HashSet<Vector3Int>();
        if (serializedLiquidBody.BodyID == 0)
        {
            Debug.LogError("Liquid Body has body ID 0. Is temporary bodies being stored or no proper ID given to the bodies");
        }
        for (int i = 0; i < serializedLiquidBody.TilePositions.Length / 3; i++)
        {
            int index = i * 3;
            tiles.Add(new Vector3Int(serializedLiquidBody.TilePositions[index], serializedLiquidBody.TilePositions[index + 1], serializedLiquidBody.TilePositions[index + 2]));
        }
        return new LiquidBody(tiles, serializedLiquidBody.Contents, this.bodyEmptyCallback, serializedLiquidBody.BodyID);
    }
    /*
    private TileData ParseSerializedTileData(SerializedTileData serializedTileData, GameTile gameTile, Dictionary<int, LiquidBody> bodyIDsToLiquidBodies)
    {
        
        Vector3Int position = new Vector3Int(serializedTileData.TilePosition[0], serializedTileData.TilePosition[1], serializedTileData.TilePosition[2]);
        Color color = new Color(serializedTileData.Color[0], serializedTileData.Color[1], serializedTileData.Color[2], serializedTileData.Color[3]);
        LiquidBody liquidBody = null;
        if (serializedTileData.LiquidBodyID != 0)
        {
            liquidBody = bodyIDsToLiquidBodies[serializedTileData.LiquidBodyID];
        }
        
        return new TileData(gameTile, position, color, liquidBody);
    }*/

    public void UpdateContents(Vector3Int tilePosition, float[] contents)
    {
        if (!this.holdsContent)
        {
            Debug.LogError("Tile at this location does not hold contents");
            return;
        }
        this.positionsToTileData[tilePosition].currentLiquidBody.contents = contents;
        // TODO Update color
    }

    public void AddTile(Vector3Int cellPosition, GameTile tile)
    {

        if (!this.positionsToTileData.ContainsKey(cellPosition)) //Create empty
        {
            this.positionsToTileData.Add(cellPosition, new TileData(cellPosition));
        }
        if (this.holdsContent)
        {
            this.positionsToTileData[cellPosition].PreviewLiquidBody(MergeLiquidBodies(cellPosition, tile));
        }
        this.positionsToTileData[cellPosition].PreviewReplacement(tile);
        this.ChangedTiles.Add(cellPosition);
        this.ApplyChangesToTilemap(cellPosition);
        // TODO update color
    }
    public void RemoveTile(Vector3Int cellPosition)
    {
        if (this.positionsToTileData.ContainsKey(cellPosition))
        {
            this.RemovedTiles.Add(cellPosition);
            if (this.holdsContent)
            {
                this.DivideLiquidBody(cellPosition);
            }
            this.positionsToTileData[cellPosition].PreviewReplacement(null);
            this.ChangedTiles.Add(cellPosition);
            this.ApplyChangesToTilemap(cellPosition);
        }

    }
    public void ConfirmPlacement()
    {
        if (!this.holdsContent)
        {
            foreach (Vector3Int tile in this.ChangedTiles)
            {
                this.positionsToTileData[tile].ConfirmReplacement();
            }
        }
        foreach (LiquidBody previewLiquidBody in this.previewBodies) // If there is liquid body
        {
            foreach (Vector3Int tile in previewLiquidBody.tiles)
            {
                this.positionsToTileData[tile].ConfirmReplacement();
            }
            this.liquidBodies.ExceptWith(previewLiquidBody.referencedBodies);
            previewLiquidBody.ClearReferencedBodies();
            this.GenerateNewLiquidBodyID(previewLiquidBody);
            this.liquidBodies.Add(previewLiquidBody);
        }
        foreach (Vector3Int cellPosition in this.RemovedTiles)
        {
            this.positionsToTileData[cellPosition].ConfirmReplacement();
            this.positionsToTileData.Remove(cellPosition);
        }
        this.ClearAll();
    }
    public void Revert()
    {
        foreach (LiquidBody previewLiquidBody in this.previewBodies) // If there is liquid body
        {
            this.ChangedTiles.UnionWith(previewLiquidBody.tiles);
            previewLiquidBody.Clear();
        }
        foreach (Vector3Int changedTile in this.ChangedTiles)
        {
            this.positionsToTileData[changedTile].Revert();
            this.ApplyChangesToTilemap(changedTile);
            if (this.positionsToTileData[changedTile].currentTile == null)
            {
                this.positionsToTileData[changedTile].Clear();
                this.positionsToTileData.Remove(changedTile);
            }
        }
        this.ClearAll();
        //System.GC.Collect();
    }
    private void ClearAll()
    {
        this.RemovedTiles = new HashSet<Vector3Int>();
        this.ChangedTiles = new HashSet<Vector3Int>();
        this.previewBodies = new List<LiquidBody>();
    }
    private void ApplyChangesToTilemap(Vector3Int cellPosition)
    {
        TileData data = positionsToTileData[cellPosition];

        this.tilemap.SetTile(cellPosition, data.currentTile);
        this.tilemap.SetTileFlags(cellPosition, TileFlags.None);
        this.tilemap.SetColor(cellPosition, data.currentColor);
    }
    private void GenerateNewLiquidBodyID(LiquidBody previewLiquidBody)
    {
        int newID = 1;
        while (true)
        {
            bool isSame = false;
            foreach (LiquidBody liquidBody in this.liquidBodies)
            {
                if (liquidBody.bodyID == newID)
                {
                    isSame = true;
                }
            }
            if (!isSame)
            {
                previewLiquidBody.bodyID = newID;
                break;
            }
            newID++;
        }
    }

    public SerializedTilemap SerializedTilemap()
    {
        // Debug.Log("Serialize " + this.tilemap.name);
        // temporarily make new list until full implementation
        return new SerializedTilemap("Tilemap", new List<TileData>(), this.liquidBodies);
    }
    /// <summary>
    /// Merge Current Tile to existing liquid bodies
    /// </summary>
    /// <param name="cellPosition"></param>
    /// <param name="tile"></param>
    /// <param name="contents"></param>
    /// <returns></returns>
    private LiquidBody MergeLiquidBodies(Vector3Int cellPosition, GameTile tile)
    {
        if (!this.holdsContent)
        {
            return null;
        }
        HashSet<LiquidBody> neighborLiquidBodies = new HashSet<LiquidBody>();
        foreach (Vector3Int neighborCell in FourNeighborTileCellPositions(cellPosition))
        {
            if (positionsToTileData.ContainsKey(neighborCell) && positionsToTileData[neighborCell].currentTile == tile)
            {
                neighborLiquidBodies.Add(positionsToTileData[neighborCell].currentLiquidBody);
            }
        }
        switch (neighborLiquidBodies.Count)
        {
            case 0: // Create new body
                HashSet<Vector3Int> newBodyTiles = new HashSet<Vector3Int>();
                newBodyTiles.Add(cellPosition);
                LiquidBody newBody = new LiquidBody(newBodyTiles, tile.defaultContents, this.bodyEmptyCallback);
                this.previewBodies.Add(newBody);
                return newBody;

            case 1: // Extend the new one drawn, or extend existing body
                List<LiquidBody> liquidBodyL = neighborLiquidBodies.ToList();
                if (liquidBodyL[0].bodyID == 0) // Preview Liquid Body, newly placed tile
                {
                    liquidBodyL[0].AddTile(cellPosition);
                    return liquidBodyL[0];
                }
                LiquidBody extendedBody = new LiquidBody(liquidBodyL[0], cellPosition);
                foreach (Vector3Int position in extendedBody.tiles)
                {
                    this.positionsToTileData[position].PreviewLiquidBody(extendedBody);
                }
                this.previewBodies.Add(extendedBody);
                return extendedBody;

            default: // Merge Multiple bodies, including new bodies generated by the placement
                LiquidBody mergedBody = new LiquidBody(neighborLiquidBodies, bodyEmptyCallback);
                mergedBody.AddTile(cellPosition);
                this.previewBodies.Add(mergedBody);
                foreach (LiquidBody liquidBody in mergedBody.referencedBodies)
                {
                    if (liquidBody.bodyID == 0)
                    {
                        previewBodies.Remove(liquidBody);
                    }
                }
                UpdatePreviewLiquidBody(mergedBody);
                return mergedBody;
        }
    }
    private void DivideLiquidBody(Vector3Int cellPosition)
    {
        HashSet<Vector3Int> remainingTiles = new HashSet<Vector3Int>();
        remainingTiles.UnionWith(positionsToTileData[cellPosition].currentLiquidBody.tiles);
        //Debug.Log("Liquidbody tile count:" + remainingTiles.Count);
        remainingTiles.ExceptWith(RemovedTiles);
        /*        foreach(Vector3Int vector3Int in remainingTiles)
                {
                    Debug.Log("Remaining: " + vector3Int.ToString());
                }*/
        List<Vector3Int> neighborTiles = new List<Vector3Int>();
        foreach (Vector3Int neighborTile in FourNeighborTileCellPositions(cellPosition)) //Filter available liquid tiles
        {
            if (remainingTiles.Contains(neighborTile))
            {
                neighborTiles.Add(neighborTile);
            }
        }
        if (neighborTiles.Count <= 1) // No tile or only one tile. don't try anything
        {
            return;
        }
        bool isContinueous = true;
        CellStatus[,] historyArray = InitializeHistoryArray(cellPosition, remainingTiles); //Create a matrix as large as meaningful iterations can go, centered at the given position. 
        historyArray[quickCheckIterations / 2, quickCheckIterations / 2] = CellStatus.Other; //Make sure the origin is other. 
        List<Vector3Int> startingTiles = new List<Vector3Int>(neighborTiles); //Copy the list to use later
        while (neighborTiles.Count > 1)
        {
            if (QuickContinuityTest(cellPosition, neighborTiles[0], neighborTiles[1], historyArray))
            {
                neighborTiles.RemoveAt(0);
                ResetHistoryArray(historyArray);
                continue;
            }
            isContinueous = false;
            break;
        }
        Debug.Log("Quick continuity check result: " + isContinueous);
        if (!isContinueous) //perform complicated check and generate new bodies if necessary
        {
            List<LiquidBody> newBodies = GenerateDividedBodies(positionsToTileData[cellPosition].currentLiquidBody, cellPosition, startingTiles, remainingTiles);
            if (newBodies != null)
            {
                previewBodies.AddRange(newBodies);
                foreach (LiquidBody liquidBody in newBodies) // Remove Referenced Preview Body
                {
                    liquidBody.RemoveNestedReference();
                }
            }
        }
    }
    private List<LiquidBody> GenerateDividedBodies(LiquidBody dividedBody, Vector3Int dividePoint, List<Vector3Int> startingTiles, HashSet<Vector3Int> remainingTiles)
    {
        List<LiquidBody> newBodies = new List<LiquidBody>();
        foreach (Vector3Int tile in startingTiles)
        {
            bool skip = false;
            foreach (LiquidBody liquidBody in newBodies) //If a prior body generation included this tile, then skip generation using this tile
            {
                if (liquidBody.tiles.Contains(tile))
                {
                    skip = true;
                    Debug.Log(tile.ToString() + "skipped");
                    break;
                }
            }
            if (!skip)
            {
                newBodies.Add(new LiquidBody(dividedBody, remainingTiles, dividePoint, tile, bodyEmptyCallback));

                Debug.Log("Generated divided body at" + tile.ToString() + "divide point: " + dividePoint.ToString());
                /*                foreach (Vector3Int tile1 in newBodies.Last().tiles)
                                {
                                    Debug.Log("Body contains" + tile1.ToString());
                                }*/
            }
            if (remainingTiles.Count == 0)
            {
                Debug.Log("Tile depleted");
                break;
            }
        }
        if (newBodies.Count == 1) //They still belong to the same body, do not add new body
        {
            Debug.Log("Still same body");
            return null;
        }
        foreach (LiquidBody body in newBodies)
        {
            foreach (Vector3Int tile in body.tiles)
            {
                this.positionsToTileData[tile].PreviewLiquidBody(body);
            }
        }
        if (dividedBody.bodyID == 0)
        {
            dividedBody.callback.Invoke(dividedBody);
        }
        return newBodies;
    }
    private bool QuickContinuityTest(Vector3Int origin, Vector3Int start, Vector3Int stop, CellStatus[,] historyArray) // TODO not performing as expected drawing left to right, also does not repathfind when stuck. 
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
        //Debug.Log("Displacement: " + displacement[0].ToString() + displacement[1].ToString());
        //Debug.Log("Array pos: " + x.ToString() + y.ToString());
        if (x < 0 || x >= quickCheckIterations - 1 || y < 0 || y >= quickCheckIterations - 1) // Out of bound
        {
            return false;
        }
        //Debug.Log(historyArray[x, y].ToString());
        if (historyArray[x, y] != CellStatus.Self)
        {
            return false;
        }
        historyArray[x, y] = CellStatus.Walked;
        //Debug.Log("Walked");
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
    private Direction2D SwitchDirection(Direction2D direction)
    {
        return direction == Direction2D.X ? Direction2D.Y : Direction2D.X;
    }

    private void UpdatePreviewLiquidBody(LiquidBody liquidBody)
    {
        foreach (Vector3Int tileCellPosition in liquidBody.tiles)
        {
            this.positionsToTileData[tileCellPosition].PreviewLiquidBody(liquidBody);
        }
    }
    private List<Vector3Int> FourNeighborTileCellPositions(Vector3Int cellPosition)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        result.Add(new Vector3Int(cellPosition.x, cellPosition.y + 1, cellPosition.z));
        result.Add(new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z));
        result.Add(new Vector3Int(cellPosition.x, cellPosition.y - 1, cellPosition.z));
        result.Add(new Vector3Int(cellPosition.x - 1, cellPosition.y, cellPosition.z));
        return result;
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
    private void OnLiquidBodyEmpty(LiquidBody liquidBody)
    {
        if (liquidBodies.Remove(liquidBody))
        {
            Debug.Log("Liquid Body Removed");
        }
        if (previewBodies.Remove(liquidBody))
        {
            Debug.Log("Preview Body Removed");
        }
    }

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

        tilePixel.r = color.r;
        tilePixel.g = color.g;
        tilePixel.b = color.b;

        tilePixel.a = (float)alphaBitMask / FLAG_VALUE_MULTIPLIER;
        TilemapTexture.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
        TilemapTexture.Apply();

        // add to propertyblock
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        renderer.sharedMaterial.SetTexture("_GridInformationTexture", TilemapTexture);
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
        tilePixel.r = 0;
        tilePixel.g = 0;
        tilePixel.b = 0;

        tilePixel.a = (float)alphaBitMask / FLAG_VALUE_MULTIPLIER;
        TilemapTexture.SetPixel(tilePosition.x, tilePosition.y, tilePixel);
        TilemapTexture.Apply();

        // add to propertyblock
        TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();
        renderer.sharedMaterial.SetTexture("_GridInformationTexture", TilemapTexture);
    }
    public void ToggleGridOverlay()
    {
        // toggle using shader here
        float currentToggleValue = Tilemap.GetComponent<TilemapRenderer>().material.GetFloat("_GridOverlayToggle");
        // set up methods for updating all or only some tilemaps
        Tilemap.GetComponent<TilemapRenderer>().sharedMaterial.SetFloat("_GridOverlayToggle", currentToggleValue == 0 ? 1 : 0);
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
                Tilemap.GetTile(tileToCheck) == tile &&
                GetTileContentsAt(tileToCheck, tile) != null &&
                !liquidBodyTiles.Contains(tileToCheck))
            {
                liquidBodyTiles.Add(tileToCheck);
                GetNeighborLiquidLocations(tileToCheck, tile, liquidBodyTiles);
            }
            GameTile thisTile = (GameTile)Tilemap.GetTile(tileToCheck);
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
        if (holdsContent)
        {
            ChangeComposition(cellPosition, composition);

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

        var returnedTile = Tilemap.GetTile<GameTile>(cellLocation);
        if (returnedTile != null)
        {
            return returnedTile;
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
        return Tilemap.GetTile(cellLocation) == tile;
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
            if (holdsContent)
            {
                return GetLiquidBodyAt(cellPosition).contents;
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
        Tilemap.GetComponent<TilemapRenderer>().sharedMaterial.SetFloat("_GridOverlayToggle", 0);
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
