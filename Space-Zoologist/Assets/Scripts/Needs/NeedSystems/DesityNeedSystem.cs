using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

/// <summary>
/// A manager for calculating the population density of each population.
/// </summary>
public class PopulationDensitySystem : MonoBehaviour
{
    // Singleton
    public static PopulationDensitySystem ins;

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
                if (ReservePartitionManager.ins.PopulationByID.ContainsKey(i))
                    cur = ReservePartitionManager.ins.PopulationByID[i];
                else
                    continue;

                // The pop lives there, add its weight/tile to density
                if (ReservePartitionManager.ins.PopulationByID.ContainsKey(i) && ReservePartitionManager.ins.CanAccess(cur, pos))
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
    /// Get the density score of a population.
    /// </summary>
    /// <param name="pop"></param>
    /// <returns></returns>
    public float GetDensityScore(Population pop)
    {
        // For easier access
        ReservePartitionManager rpm = ReservePartitionManager.ins;
        int curID = rpm.PopulationToID[pop];

        // Not initialized or does not have space to live
        if (!rpm.Populations.Contains(pop) || ReservePartitionManager.ins.Spaces[pop] == 0)
            return -1;


        // Calculate the number of accessible tiles
        float totalSize = 0;

        // Sum up the total size based on the initialized values in rpm
        for (int i = 0; i < ReservePartitionManager.maxPopulation; i++)
        {
            Population other = rpm.PopulationByID[i];
            long sharedSpace = rpm.SharedSpaces[curID][i];
            if (sharedSpace != 0)
            {
                totalSize += other.Count * other.Species.Size * rpm.SharedSpaces[curID][i] * sharedSpace / rpm.Spaces[other];
            }
        }

        // Total Size / tiles = density
        float density = totalSize / ReservePartitionManager.ins.Spaces[pop];
        return density;
    }

    /// <summary>
    /// [Obsolete due to updates to need systems] Update all affected population after the given population changes Count.
    /// </summary>
    /// <param name="changedPopulation">The population that updated its Count.</param>
    public void UpdateAffectedPopulations(Population changedPopulation)
    {
        // For easier access
        ReservePartitionManager rpm = ReservePartitionManager.ins;
        int curID = rpm.PopulationToID[changedPopulation];

        // Not initialized
        if (!rpm.Populations.Contains(changedPopulation))
            return;

        // Sum up the total size based on the initialized values in rpm
        for (int i = 0; i < ReservePartitionManager.maxPopulation; i++)
        {
            // Current population. Note: Could be changedPopulation
            Population cur = rpm.PopulationByID[i];
            long sharedSpace = rpm.SharedSpaces[curID][i];
            if (sharedSpace != 0)
            {
                //cur.UpdateNeed(NeedType.Density, GetDensityScore(cur));
            }
        }
    }
}

public class DensityNeedSystem : NeedSystem
{
    private readonly ReservePartitionManager rpm = null;
    //private readonly TileSystem tileSystem = null;
    private PopulationDensitySystem populationDensitySystem = null;
    public DensityNeedSystem(ReservePartitionManager rpm, TileSystem tileSystem, string needName = "Density") : base(needName)
    {
        this.rpm = rpm;
        //this.tileSystem = tileSystem;
        this.populationDensitySystem = PopulationDensitySystem.ins;
    }

    public override void AddPopulation(Life population)
    {
        base.AddPopulation(population);
        ReservePartitionManager.ins.AddPopulation((Population)population);
    }

    /// <summary>
    /// Updates the density score of all the registered population and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
       foreach (Population population in lives)
       {
            float density = populationDensitySystem.GetDensityScore(population);
            population.UpdateNeed("Density", density);
       }
    }
}


