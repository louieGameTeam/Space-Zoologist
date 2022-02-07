using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using static TilemapStatics;

[Serializable]
public class LiquidBody
{
    public int bodyID;
    public float[] contents;
    private HashSet<Vector3Int> tiles;

    /// <summary>
    /// Creates a new body with a single tile
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="contents"></param>
    /// <param name="bodyID"></param>
    public LiquidBody(Vector3Int tilePosition, float[] contents, int bodyID = 0)
    {
        this.bodyID = bodyID;
        this.tiles = new HashSet<Vector3Int>();
        tiles.Add(tilePosition);
        this.contents = new float[contents.Length];
        contents.CopyTo(this.contents, 0);
    }

    /// <summary>
    /// Creates a new body with hashset
    /// </summary>
    /// <param name="tilePositions"></param>
    /// <param name="contents"></param>
    public LiquidBody(HashSet<Vector3Int> tilePositions, float[] contents, int bodyID = 0)
    {
        this.bodyID = bodyID;
        this.tiles = tilePositions;
        this.contents = new float[contents.Length];
        contents.CopyTo(this.contents, 0);
    }

    /// <summary>
    /// Creates a new body with existing bodies
    /// </summary>
    /// <param name="tilePositions"></param>
    /// <param name="contents"></param>
    public LiquidBody(List<LiquidBody> liquidbodies, int bodyID = 0)
    {
        this.bodyID = bodyID;

        // use union to join all of the liquidbodies together
        foreach (LiquidBody l in liquidbodies)
            this.tiles.UnionWith(l.tiles);

        // get the average per content
        for (int i = 0; i < this.contents.Length; ++i)
        {
            contents[i] = 0;
            int totalTileCount = 0;

            // find the total contents and overall tile count
            foreach (LiquidBody l in liquidbodies)
            {
                contents[i] += l.tiles.Count * l.contents[i];
                totalTileCount += l.tiles.Count;
            }

            // take the average
            contents[i] /= totalTileCount;
        }
    }

    public void AddTile(Vector3Int pos, float[] contents)
    {
        if (!tiles.Add(pos))
        {
            Debug.LogError("Duplicated Tile added to liquid body " + this.bodyID);
            return;
        }

        // update the contents with the values
        for (int i = 0; i < this.contents.Length; ++i)
            this.contents[i] = (this.contents[i] * (tiles.Count-1) + contents[i]) / (tiles.Count);
    }

    public void RemoveTile(Vector3Int pos)
    {
        tiles.Remove(pos);
    }

    public bool ContainsTile(Vector3Int pos)
    {
        return tiles.Contains(pos);
    }

    public bool NeighborsTile(Vector3Int pos)
    {
        if (ContainsTile(pos))
            return false;

        foreach (Vector3Int tilePos in tiles)
        {
            if (IsNeighborTile(tilePos, pos))
                return true;
        }

        return false;
    }

    public void Clear()
    {
        tiles.Clear();
    }

    public SerializedLiquidBody Serialize()
    {
        return new SerializedLiquidBody(this.bodyID, this.contents);
    }
}
