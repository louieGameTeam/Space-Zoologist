using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A manager for managing how the reserve is "separated" for each population.
/// </summary>
public class ReservePartitionManager : MonoBehaviour
{
    // Singleton
    public static ReservePartitionManager ins;

    // Maximum number of populations allowed
    public const int maxPopulation = 64;

    // The list of populations, not guaranteed to be ordered
    public List<Population> Pops { get; private set; }

    // A dictionary that stores populations' id
    public Dictionary<Population, int> PopToID { get; private set; }

    // A list of opened ids for use
    private Queue<int> openID;
    private int lastRecycledID;

    // A map that represents the reserve and who can access each tile
    // The long is a bit mask with the bit (IDth bit) representing a population
    public Dictionary<Vector3Int, long> AccessMap { get; private set; }

    // Amount of accessible area for each population
    public Dictionary<Population, int> Spaces { get; private set; }

    // Amount of shared population with each population <id, <id, shared tiles> >
    public Dictionary<int, long[]> SharedSpaces { get; private set; }

    /// <summary> A list of populations to be loaded on startup. </summary>
    [SerializeField] List<Population> populationsOnStartUp = default;


    public void Awake()
    {
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }

        // long mask is limited to 64 bits
        openID = new Queue<int>();
        lastRecycledID = maxPopulation - 1; // 63
        for (int i = maxPopulation - 1; i >= 0; i--)
        {
            openID.Enqueue(i);
        }
        Pops = new List<Population>();
        PopToID = new Dictionary<Population, int>();
        AccessMap = new Dictionary<Vector3Int, long>();
        Spaces = new Dictionary<Population, int>();
        SharedSpaces = new Dictionary<int, long[]>();
    }

    public void Start()
    {
        foreach (Population population in populationsOnStartUp)
        {
            AddPopulation(population);
        }
    }

    ///<summary>
    ///Add a population to the RPM.
    ///</summary>
    public void AddPopulation(Population population)
    {
        if (!Pops.Contains(population))
        {
            // ignore their old id and assign it a new one
            int id = openID.Dequeue();
            // since IDs after maxPopulation-1 are recycled ids, we need to do clean up old values
            if (id == lastRecycledID) CleanupAccessMapForRecycledID();
            PopToID.Add(population, id);
            Pops.Add(population);

            // generate the map with the new id  
            GenerateMap(population);
            if (PopulationDensitySystem.ins != null) PopulationDensitySystem.ins.AddPop(population);
            
        }
    }

    ///<summary>
    ///Remove a population from the RPM.
    ///</summary>
    public void RemovePopulation(Population population)
    {
        if (PopulationDensitySystem.ins != null) PopulationDensitySystem.ins.RemovePop(population);
        Pops.Remove(population);
        openID.Enqueue(PopToID[population]);
        PopToID.Remove(population); // free ID
    }

    ///<summary>
    ///Called internally when ID is recycled.
    ///</summary>
    void CleanupAccessMapForRecycledID()
    {
        foreach (int id in openID)
        {
            foreach (Vector3Int loc in AccessMap.Keys)
            {
                // set the values to 0 through bit masking
                AccessMap[loc] &= ~(1L << id);
            }
            lastRecycledID = id;
        }
    }

    ///<summary>
    ///Populate the access map for a population with depth first search.
    ///</summary>
    private void GenerateMap(Population population)
    {
        if (!Pops.Contains(population))
        {
            AddPopulation(population);
        }
        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        HashSet<Vector3Int> accessible = new HashSet<Vector3Int>();
        HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();
        Vector3Int cur;

        // starting location
        Vector3Int location = FindObjectOfType<TileSystem>().WorldToCell(population.transform.position);
        stack.Push(location);

        TileSystem _tileSystem = FindObjectOfType<TileSystem>();

        // iterate until no tile left in list, ends in iteration 1 if population.location is not accessible
        while (stack.Count > 0)
        {
            // next point
            cur = stack.Pop();

            if (accessible.Contains(cur) || unaccessible.Contains(cur))
            {
                // checked before, move on
                continue;
            }

            // check if tilemap has tile and if population can access the tile (e.g. some cannot move through water)
            TerrainTile tile = _tileSystem.GetTerrainTileAtLocation(cur);
            if (tile != null && population.Species.AccessibleTerrain.Contains(tile.type))
            {
                // save the accessible location
                accessible.Add(cur);

                // check all 4 tiles around, may be too expensive/awaiting optimization
                stack.Push(cur + Vector3Int.left);
                stack.Push(cur + Vector3Int.up);
                stack.Push(cur + Vector3Int.right);
                stack.Push(cur + Vector3Int.down);
            }
            else
            {
                // save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }

        long[] SharedTiles = new long[maxPopulation];

        foreach (Vector3Int pos in accessible)
        {
            if (!AccessMap.ContainsKey(pos))
            {
                AccessMap.Add(pos, 0L);
            }
            // set the population.getID()th bit in AccessMap[pos] to 1
            AccessMap[pos] |= 1L << PopToID[population];

            // Collect info on how the population's space overlaps with others
            for (int i = 0; i < maxPopulation; i++) {
                SharedTiles[i] += (AccessMap[pos] >> i) & 1L;
            }
        }

        // Store the info on overlapping space
        Spaces[population] = accessible.Count;
        int id = PopToID[population];
        SharedSpaces[id] = SharedTiles;

        // Update the new info for pre-existing populations
        for (int i = 0; i < SharedSpaces[id].Length; i++) {
            if (SharedSpaces[id][i] != 0) {
                SharedSpaces[i][id] = SharedSpaces[id][i];
            }
        }
    }

    /// <summary>
    /// Get a list of all locations that can be accessed by this population.
    /// </summary>
    /// <param name="population"></param>
    /// <returns></returns>
    public List<Vector3Int> GetLocationsWithAccess(Population population)
    {
        List<Vector3Int> list = new List<Vector3Int>();
        foreach (KeyValuePair<Vector3Int, long> position in AccessMap)
        {
            if (CanAccess(population, position.Key))
            {
                list.Add(position.Key);
            }
        }
        return list;
    }

    ///<summary>
    ///Update the access map for every population in Pops.
    ///</summary>
    public void UpdateAccessMap()
    {
        AccessMap = new Dictionary<Vector3Int, long>();
        foreach (Population population in Pops)
        {
            GenerateMap(population);
        }
    }

    ///<summary>
    ///Check if a population can access toWorldPos.
    ///</summary>
    public bool CanAccess(Population population, Vector3 toWorldPos)
    {
        // convert to map position
        Vector3Int mapPos = FindObjectOfType<TileSystem>().WorldToCell(toWorldPos);

        return CanAccess(population, mapPos);
    }

    ///<summary>
    ///Check if a population can access CellPos.
    ///</summary>
    public bool CanAccess(Population population, Vector3Int CellPos)
    {
        // convert to map position
        Vector3Int mapPos = CellPos;

        // if accessible
        // check if the nth bit is set (i.e. accessible for the population)
        if (AccessMap.ContainsKey(mapPos))
        {
            if (((AccessMap[mapPos] >> PopToID[population]) & 1L) == 1L)
            {
                return true;
            }
        }

        // population can't access the position
        return false;
    }

    ///<summary>
    ///Go through Pops and return a list of populations that has access to the tile corresponding to toWorldPos.
    ///</summary>
    public List<Population> GetPopulationsWithAccessTo(Vector3 toWorldPos)
    {
        List<Population> accessible = new List<Population>();
        foreach (Population population in Pops)
        {
            // utilize CanAccess()
            if (CanAccess(population, toWorldPos))
            {
                accessible.Add(population);
            }
        }
        return accessible;
    }
}