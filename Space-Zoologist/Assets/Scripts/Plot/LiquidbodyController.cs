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

    public List<LiquidBody> liquidBodies { get; private set; }
    private Dictionary<Vector3Int, float[]> constructingTileContentDict;

    public void Initialize()
    {
        // declarations
        liquidBodies = new List<LiquidBody>();
        constructingTileContentDict = new Dictionary<Vector3Int, float[]>();
    }

    public void AddLiquidContentsAt(Vector3Int pos, float[] contents, bool constructing = true)
    {
        if (constructing)
        {
            if (!constructingTileContentDict.ContainsKey(pos)) {
                // add to construction list, do not affect the existing liquidbodies
                constructingTileContentDict.Add(pos, contents);
                Debug.Log("Liquid constructing at " + pos);
            }
        }
        else
        {
            MergeTile(pos, contents);
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

        // Debug.LogError("No liquid contents at " + pos);
        contents = null;
        constructing = false;
        return false;
    }

    public void SetLiquidContentsAt(Vector3Int pos, float[] contents)
    {
        foreach (LiquidBody l in liquidBodies)
        {
            if (l.ContainsTile(pos))
                l.contents = contents;
        }
    }

    public void MergeConstructingTiles()
    {
        // add every tile one by one
        foreach (KeyValuePair<Vector3Int, float[]> entry in constructingTileContentDict)
        {
            MergeTile(entry.Key, entry.Value);
        }
    }

    public void RemoveConstructingTile(Vector3Int pos)
    {
        //Remove constructing tile from dictionary (generally after merging)
        constructingTileContentDict.Remove(pos);
    }

    public bool RemoveLiquidContentsFromLiquidbodyAt(Vector3Int pos)
    {
        // check if the liquid actually exists
        if (!CheckLiquidTileAlreadyExistsAt(pos))
        {
            Debug.LogError("Liquid does not exist at " + pos);
            return false;
        }

        // set up liquidbodies just in case
        // this works because there should be no tile in two different liquidbodies
        List<HashSet<Vector3Int>> dividedBodiesTiles = new List<HashSet<Vector3Int>>();
        bool hasDivisions = false;
        LiquidBody originalLiquidbody = new LiquidBody(new List<LiquidBody>());

        // find the liquidbody with the tile and remove the tile
        foreach (LiquidBody l in liquidBodies)
        {
            if (l.ContainsTile(pos))
            {
                originalLiquidbody = l;
                hasDivisions = l.RemoveTile(pos, out dividedBodiesTiles);
            }
        }

        // check if there are any tiles left, if so delete
        if (originalLiquidbody.TileCount == 0)
        {
            liquidBodies.Remove(originalLiquidbody);
            return true;
        }

        // if there are divisions when removing the tile
        if (hasDivisions)
        {
            // create and add each new liquidbody resulting from the divide
            for (int i = 0; i < dividedBodiesTiles.Count; ++i)
            {
                LiquidBody newLiquidBody = new LiquidBody(dividedBodiesTiles[i], originalLiquidbody.contents, GenerateBodyID());
                liquidBodies.Add(newLiquidBody);
            }

            liquidBodies.Remove(originalLiquidbody);
        }

        return true;
    }

    public void RemoveConstructingLiquidContent(Vector3Int pos)
    {
        constructingTileContentDict.Remove(pos);
    }

    /// <summary>
    /// Gets the liquidbody id, returns -1 if there is none found
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int GetBodyID(Vector3Int pos)
    {
        foreach (LiquidBody l in liquidBodies)
        {
            if (l.ContainsTile(pos))
                return l.bodyID;
        }

        return -1;
    }

    public void AddLiquidBody(LiquidBody body)
    {
        liquidBodies.Add(body);
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
            // adjust values without adding new tile if replacing existing tile
            if (l.ContainsTile(pos))
            {
                l.InsertTile(contents);
                Debug.Log("Liquidbody merge successful.");
                return true;
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

    private bool CheckLiquidTileAlreadyExistsAt(Vector3Int pos)
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
        int newID = 1;
        bool idExists = true;

        while (idExists)
        {
            // search all liquidbodies and see if there is a match in ID
            idExists = false;
            foreach (LiquidBody l in liquidBodies)
            {
                // if there is a match
                if (l.bodyID == newID)
                {
                    // check the next one to see if it is available
                    idExists = true;
                    ++newID;
                }
            }
        }

        return newID;
    }

    
    #endregion

    [ContextMenu("Print data")]
    private void PrintLiquidBodyData()
    {
        foreach(var body in liquidBodies)
            print(body.TileCount);
        print("Constructing " + constructingTileContentDict.Count);
    }
}
