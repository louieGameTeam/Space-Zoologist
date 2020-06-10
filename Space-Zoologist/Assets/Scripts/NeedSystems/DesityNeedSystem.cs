using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DensityNeedSystem : NeedSystem
{
    private readonly ReservePartitionManager rpm = null;
    private readonly PopulationDensitySystem populationDensitySystem = null;
    public DensityNeedSystem(ReservePartitionManager rpm, PopulationDensitySystem populationDensitySystem, string needName = "Density") : base(needName)
    {
        this.rpm = rpm;
        this.populationDensitySystem = populationDensitySystem;
    }

    /// <summary>
    /// Updates the density score of all the registered population and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
       foreach (Population population in populations)
       {
            float density = populationDensitySystem.GetDensityScore(population);
            population.UpdateNeed("Density", density);
       }
    }
}
