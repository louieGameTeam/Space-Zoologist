using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A system to calculate the population density
/// </summary>
public class DensityCalculator
{
    private ReservePartitionManager rpm = null;
    private TileSystem tileSystem = null;

    readonly int NumPopPerTile = 2;
    readonly float TilePerUnitSize = 0.5f;

    /// <summary>
    /// Mask for showing the demo.
    /// </summary>
    public Tilemap mask;

    public DensityCalculator(ReservePartitionManager rpm, TileSystem tileSystem)
    {
        this.rpm = rpm;
        this.tileSystem = tileSystem;
    }

    List<Population> populationsByOrderOfDominance = new List<Population>();

    //Dictionary<ID, population> initialized from rpm
    Dictionary<int, Population> popsByID = new Dictionary<int, Population>();

    //Density map based on what species spreads throughout the area, similar to AccessMap in rpm
    Dictionary<Vector3Int, long> popOccupationMap = new Dictionary<Vector3Int, long>(); //bit mask
    Dictionary<Vector3Int, int> numOccupatedMap = new Dictionary<Vector3Int, int>(); // how many pop uses this tile
    Dictionary<Population, List<Vector3Int>> OccupatedTerritory = new Dictionary<Population, List<Vector3Int>>(); //the territories that the pop takes

    Dictionary<Population, int> remainingRequiredSpace = new Dictionary<Population, int>();

    public class DominanceSorter : IComparer<Population> {
        public int Compare(Population p1, Population p2) {
            return p1.Dominance.CompareTo(p2.Dominance);
        }
    }

    DominanceSorter sorter = new DominanceSorter();

    /// <summary>
    /// Initialize variables from rpm and generate new density map
    /// Has to be separate from start to allow populations to be added to the rpm
    /// </summary>
    public void Init()
    {
        //graph the density map if in demo
        //if (PDMDemo)
        //    Graph();

        GenerateDensityMap();
    }

    public void AddPop(Population pop)
    {
        if (!rpm.Populations.Contains(pop))
        {
            Debug.LogError("Population " + pop.name + " is not a part of RPM!");
            return;
        }
        else
        {
            populationsByOrderOfDominance.Add(pop);
            populationsByOrderOfDominance.Sort(sorter);
            popsByID.Add(rpm.PopulationToID[pop], pop);
            remainingRequiredSpace[pop] = (int)(pop.Count * pop.Species.Size * TilePerUnitSize);
            GenerateDensityMap(pop);
        }
    }

    public void RemovePop(Population pop)
    {
        CleanupDensityMap(rpm.PopulationToID[pop]);
        remainingRequiredSpace.Remove(pop);
        OccupatedTerritory.Remove(pop);
        populationsByOrderOfDominance.Remove(pop);

        popsByID.Remove(rpm.PopulationToID[pop]);
    }

    public void UpdateSystem() {
        var popByOrder = populationsByOrderOfDominance;
        for(int i = popByOrder.Count-1; i>= 0; i--){
            UpdateDensityMap(popByOrder[i]);
        }
    }

    public void UpdateDensityMap(Population pop) {
        remainingRequiredSpace[pop] = (int)(pop.Count * pop.Species.Size * TilePerUnitSize);
        CleanupDensityMap(rpm.PopulationToID[pop]);
        GenerateDensityMap(pop);
    }

    public void CleanupDensityMap(int id)
    {
        Population pop = popsByID[id];
        foreach (Vector3Int loc in OccupatedTerritory[pop])
        {
            //set the values to 0 through bit masking
            popOccupationMap[loc] &= ~(1L << id);
            numOccupatedMap[loc]--;
        }
        OccupatedTerritory[pop] = new List<Vector3Int>();
        remainingRequiredSpace[pop] = (int)(pop.Count * pop.Species.Size * TilePerUnitSize);
    }

    /// <summary>
    /// Exclusive to RPM. Cleanup the bits occupied by the recycled IDs.
    /// </summary>
    /// <param name="recycledID"></param>
    public void CleanupDensityMap(int[] recycledID)
    {
        foreach (int id in recycledID)
        {
            foreach (Vector3Int loc in popOccupationMap.Keys)
            {
                //set the values to 0 through bit masking
                popOccupationMap[loc] &= ~(1L << id);
            }
        }
    }

    /// <summary>
    /// Determine the population density at a certain cell position.
    /// </summary>
    /// <param name="pos"> Cell Position </param>
    // O(n) algorithm
    public int GetPopDensityAt(Vector3Int pos)
    {
        //if not a key, no population lives there and therefore density is 0
        if (popOccupationMap.ContainsKey(pos))
        {
            return numOccupatedMap[pos];
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Generate the Density Map, only called by running Init()
    /// </summary>
    private void GenerateDensityMap()
    {
        popOccupationMap = new Dictionary<Vector3Int, long>();
        numOccupatedMap = new Dictionary<Vector3Int, int>();

        List<Population> pops = rpm.Populations;

        foreach (Population pop in pops)
        {
            GenerateDensityMap(pop);
        }
    }

    //Fill in territory with BFS
    private void GenerateDensityMap(Population pop)
    {
        //find the number of accessible tiles
        int neededSpace = remainingRequiredSpace[pop];

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> accessed = new HashSet<Vector3Int>();
        HashSet<Vector3Int> unaccessible = new HashSet<Vector3Int>();
        List<Vector3Int> territory = new List<Vector3Int>();
        Vector3Int cur;

        //starting location
        //Vector3Int location = FindObjectOfType<TileSystem>().WorldToCell(pop.transform.position);
        Vector3Int location = tileSystem.WorldToCell(pop.transform.position);
        queue.Enqueue(location);

        //iterate until no tile left in list, ends in iteration 1 if pop.location is not accessible
        while (queue.Count > 0 && neededSpace > 0)
        {
            //next point
            cur = queue.Dequeue();

            if (accessed.Contains(cur) || unaccessible.Contains(cur))
            {
                //checked before, move on
                continue;
            }

            if (rpm.CanAccess(pop, cur) && (!numOccupatedMap.ContainsKey(cur) || numOccupatedMap[cur] < NumPopPerTile))
            {
                //save the Vector3Int since it is already checked
                accessed.Add(cur);

                neededSpace--;
                if (popOccupationMap.ContainsKey(cur))
                {
                    popOccupationMap[cur] |= 1L << rpm.PopulationToID[pop];
                    numOccupatedMap[cur]++;
                }
                else
                {
                    popOccupationMap.Add(cur, 1L << rpm.PopulationToID[pop]);
                    numOccupatedMap[cur] = 1;
                }

                territory.Add(cur);

                //check all 4 tiles around, may be too expensive/awaiting optimization
                queue.Enqueue(cur + Vector3Int.left);
                queue.Enqueue(cur + Vector3Int.up);
                queue.Enqueue(cur + Vector3Int.right);
                queue.Enqueue(cur + Vector3Int.down);
            }
            else
            {
                //save the Vector3Int since it is already checked
                unaccessible.Add(cur);
            }
        }

        //save the amount of space the pop has
        remainingRequiredSpace[pop] = neededSpace;

        OccupatedTerritory[pop] = territory;
    }

    // Returns the # of owned tiles / # of needed tiles
    public float GetDensityScore(Population pop)
    {
        //not initialized
        if (!rpm.Populations.Contains(pop))
            return -1;

        int requiredSpace = (int)(pop.Count * pop.Species.Size * TilePerUnitSize);

        // need 0 tiles
        if (requiredSpace == 0)
            return 1;

        float obtainedSpace = OccupatedTerritory[pop].Count;
        float score = obtainedSpace/requiredSpace;
        return score;
    }

    /// <summary>
    /// Graphing for demo purposes, may be worked into the game as a sort of inspection mode?
    /// </summary>
    public void Graph(Tilemap mask)
    {
        int i = 0;
        //find max density and calculate density for each tile
        foreach (var pair in OccupatedTerritory)
        {
            foreach (var location in pair.Value) {

                //By default the flag is TileFlags.LockColor
                mask.SetTileFlags(location, TileFlags.None);

                //set color of tile, close to maxDensity = red, close to 0 = green, in the middle = orange
                Color color = new Color(i%3/2f, (i+1)%3/2f, (i+2)%3/2f, 200.0f / 255);
                Color existingColor = mask.GetColor(location);
                if (existingColor.a != 200.0f / 255)
                    mask.SetColor(location, color);
                else
                {
                    Color newColor = Color.Lerp(color, existingColor, 0.5f);
                    newColor.a = 1f;
                    mask.SetColor(location, newColor);

                }
            }
            i++;
        }

    }
    
}
