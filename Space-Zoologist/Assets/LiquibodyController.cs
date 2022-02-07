using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidbodyController : MonoBehaviour
{
    #region Singleton
    private static LiquidbodyController _instance;
    public static LiquidbodyController Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<LiquidbodyController>();
            return _instance;
        }
    }
    #endregion

    private List<LiquidBody> liquidBodies;
    private Dictionary<Vector3Int, float[]> constructingTileContentDict;

    void Awake()
    {
        // declarations
        liquidBodies = new List<LiquidBody>();
        constructingTileContentDict = new Dictionary<Vector3Int, float[]>();
    }

    public bool AddLiquidContentsAt(Vector3Int pos, float[] contents, bool constructing = true)
    {
        // if tile already exists in the system
        // functionality may need to change in the near future
        // TODO: check with Ana for this functionality
        if (CheckTileAlreadyExists(pos))
        {
            Debug.LogError("Tile already exists!");
            return false;
        }

        if (constructing)
        {
            // add to construction list, do not affect the existing liquidbodies
            constructingTileContentDict.Add(pos, contents);
            Debug.Log("Liquid constructing at " + pos);
            return true;
        }
        else
        {
            MergeTile(pos, contents);
            return true;
        }
    }

    /// <summary>
    /// Gets the liquid contents at the location.
    /// </summary>
    /// <param name="pos">Tile location.</param>
    /// <param name="contents">3 parameter contents</param>
    /// <returns>True if successful.</returns>
    public bool GetLiquidContentsAt(Vector3Int pos, out float[] contents, out bool constructing)
    {
        // check if the tile already exists in the liquidbody.
        foreach (LiquidBody l in liquidBodies)
        {
            if (l.ContainsTile(pos))
            {
                contents = l.contents;
                constructing = false;
                return true;
            }
        }

        if (constructingTileContentDict.ContainsKey(pos))
        {
            contents = constructingTileContentDict[pos];
            constructing = true;
            return true;
        }

        Debug.LogError("No liquid contents at " + pos);
        contents = null;
        constructing = false;
        return false;
    }

    // TODO: hook this up to the next day
    public void MergeConstructingTiles()
    {
        // add every tile one by one
        foreach (KeyValuePair<Vector3Int, float[]> entry in constructingTileContentDict)
        {
            MergeTile(entry.Key, entry.Value);
        }
    }


    #region Helper Functions
    /// <summary>
    /// Merges the tile in the existing liquidbodies.
    /// If there is no existing neighbors, then make a new liquidbody and add it to the list.
    /// Note that this merge is not construction based but rather finalized.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>True if successful.</returns>
    private bool MergeTile(Vector3Int pos, float[] contents)
    {
        List<LiquidBody> neighborLiquidbodies = new List<LiquidBody>();

        foreach (LiquidBody l in liquidBodies)
        {
            // return false if contains the position already
            if (l.ContainsTile(pos))
            {
                Debug.LogError("Tile already exists in liquidbody!");
                return false;
            }
            else
            {
                if (l.NeighborsTile(pos))
                    neighborLiquidbodies.Add(l);
            }
        }

        switch (neighborLiquidbodies.Count)
        {
            case 0:
                // create a new liquidbody
                LiquidBody newLiquidbody = new LiquidBody(pos, contents, GenerateBodyID());

                // append it to the list
                liquidBodies.Add(newLiquidbody);
                break;
            case 1:
                // append to the existing liquidbody
                neighborLiquidbodies[0].AddTile(pos, contents);
                break;
            default:
                // merge the liquidbodies
                LiquidBody mergedLiquidbody = new LiquidBody(neighborLiquidbodies, GenerateBodyID());
                mergedLiquidbody.AddTile(pos, contents);

                // remove the original liquidbodies
                for (int i = 0; i < neighborLiquidbodies.Count; ++i)
                    liquidBodies.Remove(neighborLiquidbodies[i]);

                // add the newly created body
                liquidBodies.Add(mergedLiquidbody);
                break;
        }

        Debug.Log("Liquidbody merge successful.");
        return true;
    }

    private bool CheckTileAlreadyExists(Vector3Int pos)
    {
        // check within liquidbodies
        foreach (LiquidBody l in liquidBodies)
        {
            if (l.ContainsTile(pos))
                return true;
        }

        // check within currently constructing tiles
        if (constructingTileContentDict.ContainsKey(pos))
            return true;

        return false;
    }

    private int GenerateBodyID()
    {
        int newID = 0;
        bool idExists = false;

        while (!idExists)
        {
            // search all liquidbodies and see if there is a match in ID
            foreach (LiquidBody l in liquidBodies)
            {
                if (l.bodyID == newID)
                {
                    idExists = true;
                }
            }

            // if there is a match
            if (idExists)
            {
                // check the next one to see if it is available
                ++newID;
                idExists = false;
            }
        }

        return newID;
    }
    #endregion
}
