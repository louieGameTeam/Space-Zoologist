using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Hadnles need value update for all  `Terrain` type needs
/// </summary>
public class TerrainNeedSystem : NeedSystem
{
    // For `Population` consumers
    private readonly ReservePartitionManager rpm = null;
    // For `FoodSource` consumers
    private TileSystem tileSystem = null;

    public TerrainNeedSystem(ReservePartitionManager rpm, TileSystem tileSystem, NeedType needType = NeedType.Terrain) : base(needType)
    {
        this.rpm = rpm;
        this.tileSystem = tileSystem;
    }

    /// <summary>
    /// Get the terrian conposition of the consumers and update the need values
    /// </summary>
    public override void UpdateSystem()
    {
        // Unmark dirty when there is not consumer then exit
        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        foreach (Life life in Consumers)
        {
            int[] terrainCountsByType = new int[(int)TileType.TypesOfTiles];

            // Call different get terrain info function for Popultation and FoodSource
            if (life.GetType() == typeof(Population))
            {
                terrainCountsByType = rpm.GetTypesOfTiles((Population)life);
            }
            else if(life.GetType() == typeof(FoodSource))
            {
                FoodSource foodSource = (FoodSource)life;
                terrainCountsByType = tileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(life.GetPosition()), foodSource.Species.RootRadius);
            }
            else
            {
                Debug.Assert(true, "Consumer type error!");
            }

            // Update need values
            foreach (var (count, index) in terrainCountsByType.WithIndex())
            {
                string needName = ((TileType)index).ToString();

                if (life.GetNeedValues().ContainsKey(needName))
                {
                    life.UpdateNeed(needName, count);
                }
            }
        }

        this.isDirty = false;
    }
}

/// <summary>
/// To use .WithIndex
/// </summary>
public static class ForeachExtension
{
    public static  IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((item, index) => (item, index));
    }
}