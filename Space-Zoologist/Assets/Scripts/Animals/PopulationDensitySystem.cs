using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A manager for calculating the population density of each population.
/// </summary>
public class PopulationDensitySystem : MonoBehaviour
{
    // Singleton
    public static PopulationDensitySystem ins;

    // Dictionary<ID, population> initialized from rpm
    Dictionary<int, Population> popsByID;

    private void Awake()
    {
        // Singleton
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }
        popsByID = new Dictionary<int, Population>();
    }

    /// <summary>
    /// Do not do this yourself. Add a population to the Population Density System.
    /// Only called by the Reserve Partition Manager.
    /// </summary>
    /// <param name="pop">The population to be added.</param>
    public void AddPop(Population pop)
    {
        if (!ReservePartitionManager.ins.Pops.Contains(pop))
        {
            Debug.LogError("Population " + pop.name + " is not a part of RPM!");
            return;
        }
        else
        {
            popsByID.Add(ReservePartitionManager.ins.PopToID[pop], pop);
        }
    }

    /// <summary>
    /// Do not do this yourself. Remove a population from the Population Density System.
    /// Only called by the Reserve Partition Manager.
    /// </summary>
    /// <param name="pop">The population to be removed.</param>
    public void RemovePop(Population pop)
    {
        popsByID.Remove(ReservePartitionManager.ins.PopToID[pop]);
    }

    /// <summary>
    /// Determine the population density at a certain cell position.
    /// </summary>
    /// <param name="pos"> Cell Position </param>
    // O(n) algorithm
    public float GetPopDensityAt(Vector3Int pos)
    {
        // If not a key, no population lives there and therefore density is 0
        if (ReservePartitionManager.ins.AccessMap.ContainsKey(pos))
        {
            float density = 0;
            // Accumulate the weight/tile (≈ density) of populations there
            for (int i = 0; i < 64; i++)
            {
                // Initialize and error catch
                Population cur;
                if (popsByID.ContainsKey(i))
                    cur = popsByID[i];
                else
                    continue;

                // The pop lives there, add its weight/tile to density
                if (popsByID.ContainsKey(i) && ReservePartitionManager.ins.CanAccess(cur, pos))
                {
                    // Weight per tile
                    density += cur.Species.Size * cur.Count / ReservePartitionManager.ins.Spaces[cur];
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
    /// Get the density score of a population, significantly more expensive if the accessible area is big
    /// </summary>
    /// <param name="pop"></param>
    /// <returns></returns>
    public float GetDensityScore(Population pop)
    {
        // For easier access
        ReservePartitionManager rpm = ReservePartitionManager.ins;
        int curID = rpm.PopToID[pop];

        // Not initialized
        if (!rpm.Pops.Contains(pop))
            return -1;


        // Calculate the number of accessible tiles
        float totalSize = 0;

        // Sum up the total size based on the initialized values in rpm
        for (int i = 0; i < ReservePartitionManager.maxPopulation; i++) {
            Population other = popsByID[i];
            long sharedSpace = rpm.SharedSpaces[curID][i];
            if (sharedSpace != 0) {
                totalSize += other.Count * other.Species.Size * rpm.SharedSpaces[curID][i] * sharedSpace / rpm.Spaces[other];
            }
        }

        // Total Size / tiles = density
        float density = totalSize / ReservePartitionManager.ins.Spaces[pop];
        return density;
    }

    /// <summary>
    /// Update all affected population after the given population changes Count.
    /// </summary>
    /// <param name="changedPopulation">The population that updated its Count.</param>
    public void UpdateAffectedPopulations(Population changedPopulation)
    {
        // For easier access
        ReservePartitionManager rpm = ReservePartitionManager.ins;
        int curID = rpm.PopToID[changedPopulation];

        // Not initialized
        if (!rpm.Pops.Contains(changedPopulation))
            return;

        // Sum up the total size based on the initialized values in rpm
        for (int i = 0; i < ReservePartitionManager.maxPopulation; i++)
        {
            // Current population. Note: Could be changedPopulation
            Population cur = popsByID[i];
            long sharedSpace = rpm.SharedSpaces[curID][i];
            if (sharedSpace != 0)
            {
                cur.UpdateNeed(NeedType.Density, GetDensityScore(cur));
            }
        }
    }
}