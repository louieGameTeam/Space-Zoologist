using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrainNeedSystem : NeedSystem
{
    private readonly ReservePartitionManager rpm = null;
    private TileSystem tileSystem = null;
    public TerrainNeedSystem(ReservePartitionManager rpm, TileSystem tileSystem, string needName = "Terrian") : base(needName)
    {
        this.rpm = rpm;
        this.tileSystem = tileSystem;
    }

    /// <summary>
    /// Get the terrian each population as access to and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
        foreach (Life life in Consumers)
        {
            // Call different get tile function for Popultation and FoodSource
            if (life.GetType() == typeof(Population))
            {
                var terrianCountsByType = rpm.GetTypesOfTiles((Population)life);

                foreach (var (count, index) in terrianCountsByType.WithIndex())
                {
                    string needName = ((TileType)index).ToString();

                    if (life.GetNeedValues().ContainsKey(needName))
                    {
                        life.UpdateNeed(needName, count);
                    }
                }
            }
            else if(life.GetType() == typeof(FoodSource))
            {
                FoodSource foodSource = (FoodSource)life;
                var terrianCounts = tileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(life.GetPosition()), foodSource.Species.RootRadius);

                foreach (var (count, index) in terrianCounts.WithIndex())
                {
                    string needName = ((TileType)index).ToString();

                    if (life.GetNeedValues().ContainsKey(needName))
                    {
                        life.UpdateNeed(needName, count);
                    }
                }
            }
        }
    }
}

/// <summary>
/// To use .WithIdex
/// </summary>
public static class ForeachExtension
{
    public static  IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((item, index) => (item, index));
    }
}