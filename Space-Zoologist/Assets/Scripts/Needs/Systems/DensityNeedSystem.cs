using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles density need value updates
/// </summary>
public class DensityNeedSystem : NeedSystem
{
    private readonly ReservePartitionManager rpm = null;
    private DensityCalculator densityCalculator = null;

    public DensityNeedSystem(ReservePartitionManager rpm, TileSystem tileSystem, NeedType needType = NeedType.Density) : base(needType)
    {
        this.rpm = rpm;
        this.densityCalculator = new DensityCalculator(rpm, tileSystem);
        this.densityCalculator.Init();
    }

    public override void AddConsumer(Life population)
    {
        base.AddConsumer(population);
        densityCalculator.AddPop((Population)population);
    }

    /// <summary>
    /// Updates the density score of all the registered population and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        foreach (Population population in Consumers)
        {
            float density = densityCalculator.GetDensityScore(population);
            population.UpdateNeed("Density", density);

            this.isDirty = false;
        }
    }
}
