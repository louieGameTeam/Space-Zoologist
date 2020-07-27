using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum LiquidComposition { MineralX, MineralY, MineralZ };

public class LiquidNeedSystem : NeedSystem
{
    // TODO: Find the right helper system
    private readonly TileSystem tileSystem = default;

    public LiquidNeedSystem(TileSystem tileSystem, string needName = "Liquid") : base(needName)
    {
        this.tileSystem = tileSystem;
    }

    public override void UpdateSystem()
    {
        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        foreach (Life life in Consumers)
        {
            if (life.GetType() == typeof(Population))
            {
                // TODO: Get all liquid composition with accessible area
                // TODO: Get composition from helper system
                Dictionary<string, float> liquidComposition = new Dictionary<string, float>()
                {
                    { "MineralX", 3.5f },
                    { "MineralY", 6.5f },
                    { "MineralZ", 10f },
                };

                foreach (string needName in liquidComposition.Keys)
                {
                    if (life.GetNeedValues().ContainsKey(needName))
                    {
                        life.UpdateNeed(needName, liquidComposition[needName]);
                    }
                }
            }
            else if (life.GetType() == typeof(FoodSource))
            {
                // TODO: Get all liquid composition with in range
                FoodSource foodSource = (FoodSource)life;
                List<float[]> liquidCompositions = tileSystem.GetLiquidCompositionWithinRange(Vector3Int.FloorToInt(life.GetPosition()), foodSource.Species.RootRadius);

                // Check is there is found composition
                if (liquidCompositions != null)
                {
                    float[] sumComposition = new float[liquidCompositions[0].Count()];

                    foreach (float[] composition in liquidCompositions)
                    {
                        foreach( var (value, index) in composition.WithIndex())
                        {
                            sumComposition[index] += value;
                        }
                    }

                    var averageComposition = sumComposition.Select(v => v / liquidCompositions.Count).ToArray();

                    foreach (var (value, index) in averageComposition.WithIndex())
                    {
                        string needName = ((LiquidComposition)index).ToString();

                        if (life.GetNeedValues().ContainsKey(needName))
                        {
                            life.UpdateNeed(needName, value);
                        }
                    }
                }
            }
            else
            {
                Debug.Assert(true, "Consumer type error!");
            }
        }

        this.isDirty = false;
    }
}