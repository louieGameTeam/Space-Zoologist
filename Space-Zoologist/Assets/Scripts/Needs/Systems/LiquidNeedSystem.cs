using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum LiquidComposition { Water, Salt, Bacteria };

/// <summary>
/// Handles liquid need value updates
/// </summary>
public class LiquidNeedSystem : NeedSystem
{
    // TODO: Find the right helper system

    public LiquidNeedSystem(NeedType needType = NeedType.Liquid) : base(needType)
    {

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
                List<float[]> liquidCompositions = GameManager.Instance.m_reservePartitionManager.GetLiquidComposition((Population)life);
                int highScore = 0;

                // Check is there is found composition
                if (liquidCompositions != null)
                {
                    if (liquidCompositions.Count == 0)
                    {
                        this.isDirty = false;
                        return;
                    }

                    liquidCompositionToUpdate = liquidCompositions[0];

                    foreach (float[] composition in liquidCompositions)
                    {
                        // TODO: Decide which liquid source to take from
                        int curScore = 0;

                        Population population = (Population)life;

                        foreach (var (value, index) in composition.WithIndex())
                        {
                            string needName = ((LiquidComposition)index).ToString();
                            if (population.GetNeedValues().ContainsKey(needName))
                            {
                                curScore += ((int)(population.Needs[needName].GetCondition(value))) * population.Needs[needName].Severity;
                            }
                        }

                        if (curScore > highScore)
                        {
                            liquidCompositionToUpdate = composition;
                            highScore = curScore;
                        }
                    }
                }
                else
                {
                    this.isDirty = false;
                    continue;
                }
            }
            else if (life.GetType() == typeof(FoodSource))
            {
                FoodSource foodSource = (FoodSource)life;
                List<float[]> liquidCompositions = GameManager.Instance.m_gridSystem.GetLiquidCompositionWithinRange(GameManager.Instance.m_gridSystem.WorldToCell(life.GetPosition()), foodSource.Species.RootRadius);
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
                    continue;
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