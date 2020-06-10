using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TerrianNeedSystem : NeedSystem
{
    private readonly ReservePartitionManager rpm = null;
    public TerrianNeedSystem(ReservePartitionManager rpm, string needName = "Terrian") : base(needName)
    {
        this.rpm = rpm;
    }

    /// <summary>
    /// Get the terrian each population as access to and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    { 
        foreach (Population population in populations)
        {
            var terrianCountsByType = rpm.GetTypesOfTiles(population);

            foreach (var (count, index) in terrianCountsByType.WithIndex())
            {
                string needName = ((TileType)index).ToString();

                if(population.NeedsValues.ContainsKey(needName))
                {
                    population.UpdateNeed(needName, count);
                }
            }
        }
    }
}

public static class ForeachExtension
{
    public static  IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((item, index) => (item, index));
    }
}