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

    public Dictionary<Vector3Int, long> GetPopDensityMap()
    {
        return popDensityMap;
    }

    /// <summary>
    /// Add a population to the Population Density System.
    /// </summary>
    /// <param name="pop">The population to be added.</param>
    public void AddPop(Population pop)
    {
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

    /// <summary>
    /// Remove a population from the Population Density System.
    /// </summary>
    /// <param name="pop">The population to be removed.</param>
    public void RemovePop(Population pop)
    {
        popsByID.Remove(rpm.PopToID[pop]);
    }

    /// <summary>
    /// Used by Reserve Partition Manager. Do not call this function yourself.
    /// Cleanups the bits occupied by the recycled IDs.
    /// </summary>
    /// <param name="recycledID"></param>
    public void CleanupDensityMap(int[] recycledID)
    {
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
    public float GetPopDensityAt(Vector3Int pos)
    {
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
                    density += cur.Species.Size * cur.Count / spaces[cur];
                }
            }
            return density;
        }
        else
        {
            return 0f;
        }
    }

    /// <summary>
    /// Generate the Density Map for a population. Would only be called when adding a population.
    /// </summary>
    /// <param name="pop"></param>
    private void GenerateDensityMap(Population pop)
    {
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
        spaces.Add(pop, space);
    }

    /// <summary>
    /// Get the density score of a population, significantly more expensive if the accessible area is big
    /// </summary>
    /// <param name="pop"></param>
    /// <returns></returns>
    public float GetDensityScore(Population pop)
    {
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

            if (rpm.CanAccess(pop, cur))
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
    /// Update all affected population after the given population changes Count.
    /// </summary>
    /// <param name="changedPopulation">The population that updated its Count.</param>
    public void UpdateAffectedPopulations(Population changedPopulation)
    {
        HashSet<int> AffectedPopulations = new HashSet<int>();

        //TODO changedPopulation.density = GetDensityScore(changedPopulation);

        Stack<Vector3Int> stack = new Stack<Vector3Int>();
        List<Vector3Int> accessed = new List<Vector3Int>();
        List<Vector3Int> unaccessible = new List<Vector3Int>();
        Vector3Int cur;

        //starting location
        Vector3Int location = rpm.WorldToCell(changedPopulation.transform.position);
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

            if (rpm.CanAccess(changedPopulation, cur))
            {
                //save the Vector3Int since it is already checked
                accessed.Add(cur);

                for (int i = 0; i < ReservePartitionManager.maxPop; i++) {
                    if (!AffectedPopulations.Contains(i) && ((popDensityMap[cur] >> i) & 1L) == 1L) {
                        //TODO popsByID[i].density = GetDensityScore(popsByID[i]);
                    }
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
    }
}