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
    private readonly GridSystem gridSystem = default;
    private readonly ReservePartitionManager rpm = default;

    public LiquidNeedSystem(ReservePartitionManager rpm, GridSystem gridSystem, NeedType needType = NeedType.Liquid) : base(needType)
    {
        this.rpm = rpm;
        this.gridSystem = gridSystem;
    }

    public override void UpdateSystem()
    {
        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        Dictionary<Population, float> liquidTilesPerPopulation = new Dictionary<Population, float>();

        foreach(Population population in Consumers.OfType<Population>())
        {
            if(!liquidTilesPerPopulation.ContainsKey(population) && population.GetNeedValues().ContainsKey("LiquidTiles"))
                liquidTilesPerPopulation.Add(population, 0);
        }

        //Go through each body of water in the level and divide it between all populations that can access it
        foreach(LiquidBody liquidBody in gridSystem.liquidBodies)
        {
            HashSet<Population> accessiblePopulations = new HashSet<Population>();
            foreach(Population population in Consumers.OfType<Population>())
            {
                //If the population needs water, check if it can access the current liquidbody. If it can, add it to the set
                if(population.GetNeedValues().ContainsKey("LiquidTiles"))
                {
                    bool populationCanAccess = false;
                    foreach(Vector3Int location in rpm.GetLiquidLocations(population))
                    {
                        if(liquidBody.tiles.Contains(location))
                        {
                            populationCanAccess = true;
                            break;
                        }
                    }

                    if(populationCanAccess)
                        accessiblePopulations.Add(population);
                }
            }

            float waterSplit = liquidBody.tiles.Count / (float)accessiblePopulations.Count;
            foreach(Population population in accessiblePopulations)
            {
                liquidTilesPerPopulation[population] += waterSplit;
            }
        }

        float[] liquidCompositionToUpdate = default; 
        foreach (Life life in Consumers)
        {
            if (life is Population)
            {
                Population population = (Population)life;
                
                population.UpdateNeed("LiquidTiles", liquidTilesPerPopulation[population]);
                Debug.Log(population.name + " updated LiquidTiles with value: " + liquidTilesPerPopulation[population]);

                List<float[]> liquidCompositions = rpm.GetLiquidComposition(population);
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

                        foreach (var (value, index) in composition.WithIndex())
                        {
                            string needName = ((LiquidComposition)index).ToString();
                            if (population.GetNeedValues().ContainsKey(needName))
                            {
                                LiquidNeed need = (LiquidNeed)population.Needs[needName];

                                switch(needName)
                                {
                                    case "Water":
                                        curScore += need.IsFreshThresholdMet(value) ? 1 : 0;
                                        break;

                                    case "Salt":
                                        curScore += need.IsSaltThresholdMet(value) ? 1 : 0;
                                        break;

                                    case "Bacteria":
                                        curScore += need.IsBacteriaThresholdMet(value) ? 1 : 0;
                                        break;
                                }
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
            else if (life is FoodSource)
            {
                FoodSource foodSource = (FoodSource)life;

                int liquidCount = gridSystem.CountOfTilesInRange(gridSystem.WorldToCell(foodSource.GetPosition()), foodSource.Species.RootRadius)[(int)TileType.Liquid];
                foodSource.UpdateNeed("LiquidTiles", liquidCount);
                Debug.Log(foodSource.name + " updated LiquidTiles with value: " + liquidCount);

                List<float[]> liquidCompositions = gridSystem.GetLiquidCompositionWithinRange(gridSystem.WorldToCell(life.GetPosition()), foodSource.Species.RootRadius);
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
                    Debug.Log("Life: " + ((MonoBehaviour)life).gameObject.name + " updates need of type: " + needName + " with value " + value);
                }
            }
        }

        this.isDirty = false;
    }
}