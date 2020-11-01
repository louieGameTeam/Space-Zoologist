using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public enum SearchDirection { Up, Down, Left, Right }
public class LiquidBody
{
    public int bodyID;
    public HashSet<Vector3Int> tiles { get; private set; }
    public float[] contents;
    public HashSet<LiquidBody> referencedBodies { get; private set; }
    private BodyEmptyCallback callback;
    /// <summary>
    /// Creates of initializes a new body
    /// </summary>
    /// <param name="tilePositions"></param>
    /// <param name="contents"></param>
    public LiquidBody(HashSet<Vector3Int> tilePositions, float[] contents, BodyEmptyCallback bodyEmptyCallback)
    {
        this.bodyID = 0;
        this.tiles = tilePositions;
        this.contents = contents;
        this.referencedBodies = new HashSet<LiquidBody>();
        this.callback = bodyEmptyCallback;
    }
    /// <summary>
    /// Extend a existing body
    /// </summary>
    /// <param name="liquidBody"></param>
    /// <param name="newCellPosition"></param>
    public LiquidBody(LiquidBody liquidBody, Vector3Int newCellPosition)
    {
        this.bodyID = 0;
        this.tiles.UnionWith(liquidBody.tiles);
        this.tiles.Add(newCellPosition);
        liquidBody.contents.CopyTo(this.contents, liquidBody.contents.Length);
        this.referencedBodies.Add(liquidBody);
        this.callback = liquidBody.callback;
    }
    /// <summary>
    /// Merge multiple bodies
    /// </summary>
    /// <param name="liquidBodies"></param>
    public LiquidBody(HashSet<LiquidBody> liquidBodies, BodyEmptyCallback bodyEmptyCallback)
    {
        this.bodyID = 0;
        this.tiles = new HashSet<Vector3Int>();
        this.referencedBodies = liquidBodies;
        foreach (LiquidBody liquidBody in liquidBodies)
        {
            this.referencedBodies.UnionWith(referencedBodies);
            this.tiles.UnionWith(liquidBody.tiles);
        }
            this.contents = new float[liquidBodies.ToList().First().contents.Length];
        for (int i = 0; i < contents.Length; i++)
        {
            this.contents[i] = 0;
            foreach (LiquidBody liquidBody in liquidBodies)
            {
                this.contents[i] += liquidBody.contents[i] * liquidBody.tiles.Count;
            }
            this.contents[i] /= liquidBodies.Count * this.tiles.Count;
        }
        this.callback = bodyEmptyCallback;
    }
    /// <summary>
    /// Divide bodies
    /// </summary>
    /// <param name="dividedBody"></param>
    /// <param name="removedTiles"></param>
    /// <param name="dividePoint"></param>
    /// <param name="direction"></param>
    public LiquidBody(LiquidBody dividedBody, HashSet<Vector3Int> removedTiles, Vector3Int dividePoint, SearchDirection direction, BodyEmptyCallback bodyEmptyCallback)
    {
        this.bodyID = 0;
        this.tiles = new HashSet<Vector3Int>();
        tiles.Add(dividePoint);

        HashSet<Vector3Int> remainingTiles = new HashSet<Vector3Int>();
        remainingTiles.UnionWith(dividedBody.tiles);
        remainingTiles.ExceptWith(removedTiles);

        Queue<SearchDirection> sourceDirectionHistory = new Queue<SearchDirection>(2);
        sourceDirectionHistory.Enqueue(ReverseDirection(direction));

        CheckNeighbors(remainingTiles, dividePoint, sourceDirectionHistory);
        this.contents = new float[dividedBody.contents.Length];
        dividedBody.contents.CopyTo(this.contents, dividedBody.contents.Length);

        this.referencedBodies = new HashSet<LiquidBody>();
        this.referencedBodies.Add(dividedBody);

        this.callback = bodyEmptyCallback;
    }
    /// <summary>
    /// Recursively find all tiles connecting to the initial one
    /// </summary>
    /// <param name="existingTile"></param>
    /// <param name="cellPosition"></param>
    /// <param name="sourceDirectionHistory"></param>
    private void CheckNeighbors(HashSet<Vector3Int> existingTile, Vector3Int cellPosition, Queue<SearchDirection> sourceDirectionHistory)
    {
        foreach(SearchDirection direction in Enum.GetValues(typeof(SearchDirection)))
        {
            if (sourceDirectionHistory.Contains(direction)) //Optimization
            {
                continue;
            }
            Vector3Int newCell = new Vector3Int();
            switch (direction)
            {
                case SearchDirection.Up:
                    newCell = new Vector3Int(cellPosition.x, cellPosition.y + 1, cellPosition.z);
                    break;
                case SearchDirection.Down:
                    newCell = new Vector3Int(cellPosition.x, cellPosition.y - 1, cellPosition.z);
                    break;
                case SearchDirection.Right:
                    newCell = new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z);
                    break;
                case SearchDirection.Left:
                    newCell = new Vector3Int(cellPosition.x - 1, cellPosition.y, cellPosition.z);
                    break;
                default:
                    Debug.LogError("Undefined Search Direction");
                    break;
            }
            if (existingTile.Contains(newCell))
            {
                if (this.tiles.Add(newCell))
                {
                    if (sourceDirectionHistory.Count > 1)
                    {
                        sourceDirectionHistory.Dequeue();
                    }
                    sourceDirectionHistory.Enqueue(ReverseDirection(direction));
                    CheckNeighbors(existingTile, newCell, sourceDirectionHistory);
                    return;
                }
            }
            sourceDirectionHistory.Clear();
        }
    }
    /// <summary>
    /// Reverses the direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private SearchDirection ReverseDirection(SearchDirection direction)
    {
        switch (direction)
        {
            case SearchDirection.Up:
                return SearchDirection.Down;
            case SearchDirection.Down:
                return SearchDirection.Up;
            case SearchDirection.Right:
                return SearchDirection.Left;
            case SearchDirection.Left:
                return SearchDirection.Right;
            default:
                Debug.LogError("Undefined Search Direction");
                return direction;
        }
    }
    public void AddTile(Vector3Int tileToAdd)
    {
        if (!this.tiles.Add(tileToAdd))
        {
            Debug.LogError("Duplicated Tile added to liquid body");
        }
    }
    public void RemoveTile(Vector3Int tileToRemove)
    {
        this.tiles.Remove(tileToRemove);
        if (this.tiles.Count == 0)
        {
            this.callback.Invoke(this);
        }
    }
}
