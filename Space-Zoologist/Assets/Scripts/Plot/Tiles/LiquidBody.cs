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
    public int TileCount { 
        get { return tiles == null ? 0 : tiles.Count; } 
    }

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
        tiles = new HashSet<Vector3Int>();

        // use union to join all of the liquidbodies together
        foreach (LiquidBody l in liquidbodies)
            this.tiles.UnionWith(l.tiles);

        contents = new float[] { 0, 0, 0 };

        // get the average per content
        for (int i = 0; i < this.contents.Length; ++i)
        {
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

    // Note that tile position doesn't matter when replacing an existing tile
    public void InsertTile(float[] contents)
    {
        // update the contents with the values
        for (int i = 0; i < this.contents.Length; ++i)
            this.contents[i] = (this.contents[i] * (tiles.Count-1) + contents[i]) / (tiles.Count);
    }

    public bool RemoveTile(Vector3Int pos, out List<HashSet<Vector3Int>> dividedBodiesTiles)
    {
        // check if the tile exists in the liquidbody
        if (!tiles.Contains(pos))
        {
            Debug.LogError("Tile does not exist!");
            dividedBodiesTiles = null;
            return false;
        }

        // remove the tile
        tiles.Remove(pos);

        // check for continuity
        // find the neighboring liquid tiles
        List<Vector3Int> neighborTiles = FourNeighborTileLocations(pos);

        switch (neighborTiles.Count)
        {
            // no tiles
            case 0:
                Debug.Log("No tiles left.");
                dividedBodiesTiles = null;
                return false;
            // end tile
            case 1:
                dividedBodiesTiles = null;
                return false;
            // more than one connection
            default:
                // ewwwwww
                List<HashSet<Vector3Int>> continuousBodies = new List<HashSet<Vector3Int>>();

                for (int i = 0; i < neighborTiles.Count; ++i)
                {
                    // if the neighbor actually is in the liquidbody
                    if (ContainsTile(neighborTiles[i]))
                    {
                        // total list of tiles that is going to be added to
                        HashSet<Vector3Int> neighborbodyTiles = new HashSet<Vector3Int>();
                        // queue to explore
                        Queue<Vector3Int> exploreQueue = new Queue<Vector3Int>();
                        // add the first tile
                        neighborbodyTiles.Add(neighborTiles[i]);
                        exploreQueue.Enqueue(neighborTiles[i]);

                        // should be O(4n), but could be optimized
                        while (exploreQueue.Count > 0)
                        {
                            // find the next neighbors of the queue
                            List<Vector3Int> fourNeighbors = FourNeighborTileLocations(exploreQueue.Dequeue());

                            // loop through neighbors
                            for (int neighborIndex = 0; neighborIndex < fourNeighbors.Count; ++neighborIndex)
                            {
                                // if the neighbor is in the liquidbody and not already in the tile list
                                if (ContainsTile(fourNeighbors[neighborIndex]) && !neighborbodyTiles.Contains(fourNeighbors[neighborIndex]))
                                {
                                    // add the neighbor
                                    neighborbodyTiles.Add(fourNeighbors[neighborIndex]);
                                    exploreQueue.Enqueue(fourNeighbors[neighborIndex]);
                                }
                            }
                        }

                        continuousBodies.Add(neighborbodyTiles);
                    }
                }

                // check if the continuous bodies are the same, if so, then they are connected (remove them)
                for (int i = 0; i < continuousBodies.Count; ++i)
                {
                    for (int j = i + 1; j < continuousBodies.Count; ++j)
                    {
                        if (continuousBodies[i].SetEquals(continuousBodies[j]))
                        {
                            continuousBodies.Remove(continuousBodies[j]);
                            j--;
                        }
                    }
                }

                // the whole thing is still continuous
                if (continuousBodies.Count <= 1)
                {
                    Debug.Log("Liquid removal from liquidbody successful.");
                    dividedBodiesTiles = null;
                    return false;
                }
                // split between two or more bodies
                else
                {
                    dividedBodiesTiles = continuousBodies;
                    return true;
                }
        }
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
        contents = new float[] { 0, 0, 0 };
    }

    public SerializedLiquidBody Serialize()
    {
        return new SerializedLiquidBody(this.bodyID, this.contents);
    }
}
