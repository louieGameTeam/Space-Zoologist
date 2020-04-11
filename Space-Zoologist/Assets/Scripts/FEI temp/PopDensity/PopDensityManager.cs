using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A manager for calculating the population density of each population.
/// </summary>
public class PopDensityManager : MonoBehaviour
{
    //singleton
    public static PopDensityManager ins;


    //for easy access, equivalent to ReservePartitionManager.ins
    ReservePartitionManager rpm;

    //Dictionary<ID, population> initialized from rpm
    Dictionary<int, Population> popsByID;

    //Density map based on what species spreads throughout the area, similar to AccessMap in rpm
    Dictionary<Vector3Int, long> popDensityMap;

    Dictionary<Population, int> spaces;

    //if in demo
    public bool PDMDemo; 

    public void Awake()
    {
        //singleton
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }
        rpm = ReservePartitionManager.ins;
        popsByID = new Dictionary<int, Population>();
        spaces = new Dictionary<Population, int>();
        popDensityMap = new Dictionary<Vector3Int, long>();
    }

    public void Start()
    {
        Invoke("Init", 0.3f);
    }

    /// <summary>
    /// Initialize variables from rpm and generate new density map
    /// Has to be separate from start to allow populations to be added to the rpm
    /// </summary>
    public void Init() {
        //graph the density map if in demo
        if(PDMDemo)
            Graph();
    }

    public void AddPop(Population pop) {
        if (!rpm.Pops.Contains(pop))
        {
            Debug.LogError("Population " + pop.name + " is not a part of RPM!");
            return;
        }
        else
        {
            popsByID.Add(rpm.PopToID[pop], pop);
            GenerateDensityMap(pop);
        }
    }
    public void RemovePop(Population pop) {
        popsByID.Remove(rpm.PopToID[pop]);
    }

    /// <summary>
    /// Exclusive to RPM. Cleanup the bits occupied by the recycled IDs.
    /// </summary>
    /// <param name="recycledID"></param>
    public void CleanupDensityMap(int[] recycledID) {
        foreach (int id in recycledID)
        {
            foreach (Vector3Int loc in popDensityMap.Keys)
            {
                //set the values to 0 through bit masking
                popDensityMap[loc] &= ~(1L << id);
            }
        }
    }

    /// <summary>
    /// Determine the population density at a certain cell position.
    /// </summary>
    /// <param name="pos"> Cell Position </param>
    // O(n) algorithm
    public float GetPopDensityAt(Vector3Int pos) {
        //if not a key, no population lives there and therefore density is 0
        if (popDensityMap.ContainsKey(pos))
        {
            float density = 0;
            //accumulate the weight/tile (≈ density) of populations there
            for (int i = 0; i < 64; i++)
            {
                //the pop lives there, add its weight/tile to density
                if (popsByID.ContainsKey(i) && ((popDensityMap[pos] >> i) & 1L) == 1L)
                {
                    Population cur = popsByID[i];
                    print(cur.Species.Size * cur.Count / spaces[cur]);
                    //weight per tile
                    density += cur.Species.Size*cur.Count / spaces[cur];
                }
            }
            return density;
        }
        else {
            return 0f;
        }
    }

    /// <summary>
    /// Generate the Density Map, only called by running Init()
    /// </summary>
    //(2r^2 + 2r + 1) * O(n) algorithm, significantly more expensive if radius is big
    private void GenerateDensityMap()
    {
        popDensityMap = new Dictionary<Vector3Int, long>();

        List<Population> pops = rpm.Pops;

        foreach (Population pop in pops)
        {
            GenerateDensityMap(pop);
        }
    }

    private void GenerateDensityMap(Population pop) {
        //find the number of accessible tiles
        int space = 0;

        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        List<Vector3Int> accessed = new List<Vector3Int>();
        List<Vector3Int> unaccessible = new List<Vector3Int>();
        Vector3Int cur;

        //starting location
        Vector3Int location = rpm.WorldToCell(pop.transform.position);
        stack.Push(location);

        //iterate until no tile left in list, ends in iteration 1 if pop.location is not accessible
        while (stack.Count > 0)
        {
            //next point
            cur = stack.Pop();

            if (accessed.Contains(cur) || unaccessible.Contains(cur))
            {
                //checked before, move on
                continue;
            }

            if (rpm.CanAccess(pop, cur))
            {
                //save the Vector3Int since it is already checked
                accessed.Add(cur);

                space++;
                if (popDensityMap.ContainsKey(cur))
                {
                    popDensityMap[cur] |= 1L << rpm.PopToID[pop];
                }
                else
                {
                    popDensityMap.Add(cur, 1L << rpm.PopToID[pop]);
                }
                
                //check all 4 tiles around, may be too expensive/awaiting optimization
                stack.Push(cur + Vector3Int.left);
                stack.Push(cur + Vector3Int.up);
                stack.Push(cur + Vector3Int.right);
                stack.Push(cur + Vector3Int.down);
            }
            else
            {
                //save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }

        //save the amount of space the pop has
        spaces.Add(pop,space);
    }

    //(2r^2 + 2r + 1) * O(n) algorithm, significantly more expensive if radius is big
    public float GetDensityScore(Population pop) {
        //not initialized
        if (!rpm.Pops.Contains(pop))
            return -1;


        //calculate the number of accessible tiles
        float density = 0;

        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        List<Vector3Int> accessed = new List<Vector3Int>();
        List<Vector3Int> unaccessible = new List<Vector3Int>();
        Vector3Int cur;

        //starting location
        Vector3Int location = rpm.WorldToCell(pop.transform.position);
        stack.Push(location);

        //iterate until no tile left in list, ends in iteration 1 if pop.location is not accessible
        while (stack.Count > 0)
        {
            //next point
            cur = stack.Pop();

            if (accessed.Contains(cur) || unaccessible.Contains(cur))
            {
                //checked before, move on
                continue;
            }

            if (rpm.CanAccess(pop,cur))
            {
                //save the Vector3Int since it is already checked
                accessed.Add(cur);

                //add population density at the tile to density, note that density/tile * 1 tile = weight
                //so this is summing the weight at this point
                density += GetPopDensityAt(cur);

                //check all 4 tiles around, may be too expensive/awaiting optimization
                stack.Push(cur + Vector3Int.left);
                stack.Push(cur + Vector3Int.up);
                stack.Push(cur + Vector3Int.right);
                stack.Push(cur + Vector3Int.down);
            }
            else
            {
                //save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }

        //total weight / tiles = density
        density /= spaces[pop];
        return density;
    }

    /// <summary>
    /// Mask for showing the demo.
    /// </summary>
    public Tilemap mask;

    /// <summary>
    /// Graphing for demo purposes, may be worked into the game as a sort of inspection mode?
    /// </summary>
    public void Graph() {
        //colors
        Dictionary<Vector3Int, float> col = new Dictionary<Vector3Int, float>();

        //max density for color comparison
        float maxDensity = -1;

        //find max density and calculate density for each tile
        foreach (KeyValuePair<Vector3Int, long> pair in popDensityMap)
        {
            //calculate density
            float density = GetPopDensityAt(pair.Key);

            col.Add(pair.Key, density);

            if (density > maxDensity) {
                maxDensity = density;
            }
        }

        //set color based on the fraction density/maxdensity
        foreach (KeyValuePair<Vector3Int, float> pair in col)
        {
            //By default the flag is TileFlags.LockColor
            mask.SetTileFlags(pair.Key, TileFlags.None);

            //set color of tile, close to maxDensity = red, close to 0 = green, in the middle = orange
            mask.SetColor(pair.Key, new Color(pair.Value/maxDensity, 1-pair.Value/maxDensity, 0, 255.0f/255));
        }

        //debug
        print(maxDensity);
    }
}
