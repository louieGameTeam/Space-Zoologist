using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public delegate void BodyEmptyCallback(LiquidBody liquidBody);
public enum AbsoluteDirection { Towards, Against, Parallel}
public class TileLayerManager : MonoBehaviour
{
    private enum Direction2D { X, Y }
    private enum CellStatus { Other, Self, Walked }
    private enum MoveType { Towards, Parallel1, Parallel2, Away }
    public bool holdsContent;
    private Tilemap tilemap;
    public Dictionary<Vector3Int, TileData> positionsToTileData { get; private set; } = new Dictionary<Vector3Int, TileData>();
    private HashSet<Vector3Int> ChangedTiles = new HashSet<Vector3Int>();
    public HashSet<LiquidBody> liquidBodies { get; private set; } = new HashSet<LiquidBody>();
    public List<LiquidBody> previewBodies { get; private set; } = new List<LiquidBody>();
    private HashSet<Vector3Int> RemovedTiles = new HashSet<Vector3Int>();
    private BodyEmptyCallback bodyEmptyCallback;
    [SerializeField] private int quickCheckIterations = 6; //Number of tiles to quick check, if can't reach another tile within this many walks, try to generate new body by performing full check
                                                           // Increment by 2 makes a difference. I.E. even numbers, at least 6 to account for any missing tile in 8 surrounding tiles

    /*    private void Awake()
        {
            this.bodyEmptyCallback = OnLiquidBodyEmpty;
            this.tilemap = this.gameObject.GetComponent<Tilemap>();
        }*/
    private void Awake()
    {
        this.Initialize();
    }
    private void Initialize()
    {
        this.bodyEmptyCallback = OnLiquidBodyEmpty;
        this.tilemap = this.gameObject.GetComponent<Tilemap>();
        this.positionsToTileData = new Dictionary<Vector3Int, TileData>();
        this.liquidBodies = new HashSet<LiquidBody>();
        this.previewBodies = new List<LiquidBody>();
        this.RemovedTiles = new HashSet<Vector3Int>();
        this.tilemap.ClearAllTiles();
    }
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
        Initialize();
        Dictionary<int, LiquidBody> bodyIDsToLiquidBodies = new Dictionary<int, LiquidBody>();
        foreach (SerializedLiquidBody serializedLiquidBody in serializedTilemap.SerializedLiquidBodies)
        {
            LiquidBody liquidBody = ParseSerializedLiquidBody(serializedLiquidBody);
            bodyIDsToLiquidBodies.Add(liquidBody.bodyID, liquidBody);
            this.liquidBodies.Add(liquidBody);
        }
        Dictionary<string, GameTile> namesToGameTiles = new Dictionary<string, GameTile>();
        foreach(SerializedTileData serializedTileData in serializedTilemap.SerializedTileDatas)
        {
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
            this.positionsToTileData.Add(tileData.tilePosition,tileData);
            ApplyChangesToTilemap(tileData.tilePosition);
        }
    }
    private LiquidBody ParseSerializedLiquidBody(SerializedLiquidBody serializedLiquidBody)
    {
        HashSet<Vector3Int> tiles = new HashSet<Vector3Int>();
        if (serializedLiquidBody.BodyID == 0)
        {
            Debug.LogError("Liquid Body has body ID 0. Is temporary bodies being stored or no proper ID given to the bodies");
        }
        for (int i = 0; i < serializedLiquidBody.TilePositions.Length/3; i++)
        {
            int index = i * 3;
            tiles.Add(new Vector3Int(serializedLiquidBody.TilePositions[index], serializedLiquidBody.TilePositions[index + 1], serializedLiquidBody.TilePositions[index + 2]));
        }
        return new LiquidBody(tiles, serializedLiquidBody.Contents, this.bodyEmptyCallback, serializedLiquidBody.BodyID);
    }
    private TileData ParseSerializedTileData(SerializedTileData serializedTileData, GameTile gameTile, Dictionary<int, LiquidBody> bodyIDsToLiquidBodies)
    {
        Vector3Int position = new Vector3Int(serializedTileData.TilePosition[0], serializedTileData.TilePosition[1], serializedTileData.TilePosition[2]);
        Color color = new Color(serializedTileData.Color[0],serializedTileData.Color[1], serializedTileData.Color[2], serializedTileData.Color[3]);
        LiquidBody liquidBody = null;
        if (serializedTileData.LiquidBodyID != 0)
        {
            liquidBody = bodyIDsToLiquidBodies[serializedTileData.LiquidBodyID];
        }
        return new TileData(gameTile, position, color, liquidBody);
    }
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
        if (Input.GetKeyDown(KeyCode.M))
        {
            return;
        }
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
    public SerializedTilemap Serialize()
    {
        Debug.Log("Serialize " + this.tilemap.name);
        return new SerializedTilemap(this.gameObject.name, this.positionsToTileData, this.liquidBodies);
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

    private void ResetHistoryArray (CellStatus[,] historyArray)
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
}
