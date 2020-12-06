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
    public BodyEmptyCallback callback;
    public void Clear()
    {
        this.tiles.Clear();
        this.referencedBodies.Clear();
        this.callback = null;
    }
    /// <summary>
    /// Creates of initializes a new body
    /// </summary>
    /// <param name="tilePositions"></param>
    /// <param name="contents"></param>
    public LiquidBody(HashSet<Vector3Int> tilePositions, float[] contents, BodyEmptyCallback bodyEmptyCallback, int bodyID = 0)
    {
        this.bodyID = bodyID;
        this.tiles = tilePositions;
        this.contents = new float[contents.Length];
        contents.CopyTo(this.contents, 0);
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
        this.tiles = new HashSet<Vector3Int>();
        this.tiles.UnionWith(liquidBody.tiles);
        this.tiles.Add(newCellPosition);
        this.contents = new float[liquidBody.contents.Length];
        liquidBody.contents.CopyTo(this.contents, 0);
        this.referencedBodies = new HashSet<LiquidBody>();
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
        this.referencedBodies = new HashSet<LiquidBody>();
        this.referencedBodies.UnionWith(liquidBodies);
        foreach (LiquidBody liquidBody in liquidBodies)
        {
            if (liquidBody.referencedBodies.Count > 0)
            {
                this.referencedBodies.UnionWith(liquidBody.referencedBodies);
                this.referencedBodies.Remove(liquidBody); //Remove preview bodies as preview bodies have referenced bodies
            }
            this.tiles.UnionWith(liquidBody.tiles);
        }
        this.contents = new float[liquidBodies.ToList().First().contents.Length];
        for (int i = 0; i < contents.Length; i++)
        {
            this.contents[i] = 0;
            int tileCount = 0;
            foreach (LiquidBody liquidBody in this.referencedBodies)
            {
                if (liquidBody.bodyID != 0) // Not preview body
                {
                    tileCount += liquidBody.tiles.Count;
                    this.contents[i] += liquidBody.contents[i] * liquidBody.tiles.Count;
                }
            }
            Debug.Log("Merged tile count" + tileCount.ToString());
            this.contents[i] /= tileCount;
        }
        this.callback = bodyEmptyCallback;
    }
    /// <summary>
    /// Divide bodies
    /// </summary>
    /// <param name="dividedBody"></param>
    /// <param name="remainingTile"></param>
    /// <param name="dividePoint"></param>
    /// <param name="direction"></param>
    public LiquidBody(LiquidBody dividedBody, HashSet<Vector3Int> remainingTiles, Vector3Int dividePoint, Vector3Int startPoint, BodyEmptyCallback bodyEmptyCallback)
    {
        Debug.Log("Divided body ID: " + dividedBody.bodyID.ToString() + " Divided body tile count: " + dividedBody.tiles.Count);
        this.bodyID = 0;
        this.tiles = new HashSet<Vector3Int>();
        this.tiles.Add(startPoint);
        remainingTiles.Remove(startPoint);

        CheckNeighbors(remainingTiles, startPoint);
        this.contents = new float[dividedBody.contents.Length];
        dividedBody.contents.CopyTo(this.contents, 0);

        this.referencedBodies = new HashSet<LiquidBody>();
        this.referencedBodies.Add(dividedBody);
        if (dividedBody.bodyID == 0)
        {
            this.referencedBodies.UnionWith(dividedBody.referencedBodies);
            this.referencedBodies.Remove(dividedBody);
            dividedBody.callback.Invoke(dividedBody);
        }
        //this.RemoveReferencedPreviewBodies();

        this.callback = bodyEmptyCallback;
    }
    /// <summary>
    /// Recursively find all tiles connecting to the initial one
    /// </summary>
    /// <param name="existingTile"></param>
    /// <param name="cellPosition"></param>
    /// <param name="sourceDirectionHistory"></param>
    private void CheckNeighbors(HashSet<Vector3Int> existingTile, Vector3Int cellPosition)
    {
        foreach (Vector3Int position in GridUtils.FourNeighborTiles(cellPosition))
        {
            if (existingTile.Remove(position))
            {
                CheckNeighbors(existingTile, position);
/*                if (this.tiles.Add(position))
                {
                    return;
                }*/
            }
        }
    }
    public void AddTile(Vector3Int tileToAdd)
    {
        if (!this.tiles.Add(tileToAdd))
        {
            Debug.LogError("Duplicated Tile added to liquid body " + this.bodyID);
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
    public SerializedLiquidBody Serialize()
    {
        return new SerializedLiquidBody(this.bodyID, this.tiles, this.contents);
    }
}
