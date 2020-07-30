﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum LiquidComposition { MineralX, MineralY, MineralZ };

/// <summary>
/// Handles liquid need value updates
/// </summary>
public class LiquidNeedSystem : NeedSystem
{
    // TODO: Find the right helper system
    private readonly TileSystem tileSystem = default;
    private readonly ReservePartitionManager rpm = default;

    public LiquidNeedSystem(ReservePartitionManager rpm, TileSystem tileSystem, NeedType needType = NeedType.Liquid) : base(needType)
    {
        this.rpm = rpm;
        this.tileSystem = tileSystem;
    }

    public override void UpdateSystem()
    {
        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        float[] liquidCompositionToUpdate = default; 

        foreach (Life life in Consumers)
        {
            if (life.GetType() == typeof(Population))
            {
                // TODO: Get all liquid composition with accessible area
                // TODO: Get composition from helper system

                List<float[]> liquidCompositions = rpm.GetLiquidComposition((Population)life);
                // Check is there is found composition
                if (liquidCompositions != null)
                {
                    foreach (float[] composition in liquidCompositions)
                    {

                        // TODO: Decide which liquid source to take from

                        liquidCompositionToUpdate = composition;
                    }
                }
                else
                {
                    this.isDirty = false;
                    return;
                }
            }
            else if (life.GetType() == typeof(FoodSource))
            {
                FoodSource foodSource = (FoodSource)life;
                List<float[]> liquidCompositions = tileSystem.GetLiquidCompositionWithinRange(Vector3Int.FloorToInt(life.GetPosition()), foodSource.Species.RootRadius);

                // Check is there is found composition
                if (liquidCompositions != null)
                {
                    float[] sumComposition = new float[liquidCompositions[0].Count()];

                    foreach (float[] composition in liquidCompositions)
                    {
                        foreach (var (value, index) in composition.WithIndex())
                        {
                            sumComposition[index] += value;
                        }
                    }

                    // Use to avergae composition to update
                    liquidCompositionToUpdate = sumComposition.Select(v => v / liquidCompositions.Count).ToArray();
                }
                else
                {
                    this.isDirty = false;
                    return;
                }
            }
            else
            {
                Debug.Assert(true, "Consumer type error!");
            }

            foreach (var (value, index) in liquidCompositionToUpdate.WithIndex())
            {
                string needName = ((LiquidComposition)index).ToString();

                if (life.GetNeedValues().ContainsKey(needName))
                {
                    life.UpdateNeed(needName, value);
                }
            }
        }

        this.isDirty = false;
    }
}