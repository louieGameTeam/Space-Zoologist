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
    private TileDataController m_gridsystemReference => GameManager.Instance.m_tileDataController;

    public LiquidNeedSystem(NeedType needType = NeedType.Liquid) : base(needType) {}

    public override void UpdateSystem()
    {
        //throw new System.NotImplementedException(
        //    "Liquid need system no longer able to update");

        if (this.Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        Dictionary<Population, float> liquidTilesPerPopulation = new Dictionary<Population, float>();
        Dictionary<Population, HashSet<LiquidBody>> liquidBodiesPerPopulation = new Dictionary<Population, HashSet<LiquidBody>>();

        // Setup the dictionary for each population if that population has water needs
        foreach(Population population in Consumers.OfType<Population>())
        {
            // NOTE: uses the old need system
            //if(!liquidTilesPerPopulation.ContainsKey(population) && population.HasWaterNeeds())
            //    liquidTilesPerPopulation.Add(population, 0);
        }

        //Go through each body of water in the level and divide it between all populations that can access it
        foreach(LiquidBody liquidBody in LiquidbodyController.Instance.liquidBodies)
        {
            HashSet<Population> accessiblePopulations = new HashSet<Population>();

            // For each liquid body, check if this population can drink from it
            foreach(Population population in Consumers.OfType<Population>())
            {
                // Check if the species can drink from this liquid body
                // NOTE: this logic does not work anymore because it is based on the deprecated needs system
                //if (LiquidIsDrinkable(population.species.LiquidNeeds, liquidBody))
                //{
                //    bool populationCanAccess = false;

                //    // check if any of the liquidbody's tiles are accessible to this population
                //    foreach (Vector3Int location in GameManager.Instance.m_reservePartitionManager.GetLiquidLocations(population)) 
                //    {
                //        if (liquidBody.ContainsTile(location))
                //        {
                //            populationCanAccess = true;
                //            break;
                //        }
                //    }

                //    //if the population can access this water source, add it to the set
                //    if (populationCanAccess) 
                //        accessiblePopulations.Add(population);
                //}
            }

            // split this water source equally between all populations that have access to it, regardless of that population's size
            // Does this account for water dominance?  Or is there no such thing as water dominance?
            float waterSplit = liquidBody.TileCount / (float)accessiblePopulations.Count;
            foreach(Population population in accessiblePopulations)
            {
                liquidTilesPerPopulation[population] += waterSplit;

                if(!liquidBodiesPerPopulation.ContainsKey(population))
                    liquidBodiesPerPopulation.Add(population, new HashSet<LiquidBody>());
                
                liquidBodiesPerPopulation[population].Add(liquidBody);
            }
        }

        float[] liquidCompositionToUpdate = default; 
        foreach (Life life in Consumers)
        {
            if (life is Population)
            {
                Population population = (Population)life;
                
                // Set the number of liquid tiles that this population drinks from
                population.drinkableLiquidTiles = liquidTilesPerPopulation[population];
                //Debug.Log(population.name + " updates LiquidTiles with value: " + liquidTilesPerPopulation[population]);

                // Check is there is found composition
                if (liquidBodiesPerPopulation.ContainsKey(population))
                {
                    int totalTiles = 0;

                    //Average out all the liquid compositions
                    liquidCompositionToUpdate = new float[]{0, 0, 0};

                    foreach (LiquidBody liquidBody in liquidBodiesPerPopulation[population])
                    {
                        //Weight each composition based on the number of tiles in the liquidbody
                        liquidCompositionToUpdate[0] += liquidBody.contents[0] * liquidBody.TileCount;
                        liquidCompositionToUpdate[1] += liquidBody.contents[1] * liquidBody.TileCount;
                        liquidCompositionToUpdate[2] += liquidBody.contents[2] * liquidBody.TileCount;

                        totalTiles += liquidBody.TileCount;
                    }

                    for(int i = 0; i <= 2; ++i)
                    {
                        liquidCompositionToUpdate[i] /= totalTiles;
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

                float liquidCount = 0;
                List<float[]> liquidCompositions = new List<float[]>();
                List<float[]> potentialCompositions = m_gridsystemReference.GetLiquidCompositionWithinRange(m_gridsystemReference.WorldToCell(foodSource.GetPosition()), foodSource.Species.Size, foodSource.Species.RootRadius);

                foreach(float[] composition in potentialCompositions)
                {
                    // NOTE: this logic does not work anymore because it is based on the deprecated needs system
                    //if (LiquidIsDrinkable(foodSource.Species.LiquidNeeds, composition))
                    //{
                    //    ++liquidCount;
                    //    liquidCompositions.Add(composition);
                    //}
                }

                // foodSource.UpdateNeed("LiquidTiles", liquidCount);
                //Debug.Log(foodSource.name + " updated LiquidTiles with value: " + liquidCount);

                // Check is there is found composition
                if (liquidCompositions.Count > 0)
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

            // Update all water needs for 
            foreach (var (value, index) in liquidCompositionToUpdate.WithIndex())
            {
                // Get the item id based on the water composition index
                ItemID waterID = ItemID.FromWaterIndex(index);

                // Update the corresponding water need
                if (life.GetNeedValues().ContainsKey(waterID))
                {
                    life.UpdateNeed(waterID, value);
                    //Debug.Log("Life: " + ((MonoBehaviour)life).gameObject.name + " updates need of type: " + needName + " with value " + value);
                }
            }
        }

        this.isDirty = false;
    }

    /// <summary>
    /// Determine if a liquid body is drinkable by a species that has these liquid needs
    /// </summary>
    /// <param name="liquidNeeds"></param>
    /// <param name="liquidBody"></param>
    /// <returns></returns>
    public static bool LiquidIsDrinkable(List<LiquidNeedConstructData> liquidNeeds, LiquidBody liquidBody)
    {
        return LiquidIsDrinkable(liquidNeeds, liquidBody.contents);
    }

    /// <summary>
    /// Determine if a liquid body with these contents 
    /// is drinkable by a species that has these liquid needs
    /// </summary>
    /// <param name="liquidNeeds"></param>
    /// <param name="liquidBodyContents"></param>
    /// <returns></returns>
    public static bool LiquidIsDrinkable(List<LiquidNeedConstructData> liquidNeeds, float[] liquidBodyContents)
    {
        int index = 0;
        bool drinkable = true;

        // Iterate over each liquid need
        while (drinkable && index < liquidNeeds.Count)
        {
            // Get the composition addressed by this need
            LiquidNeedConstructData need = liquidNeeds[index];
            float currentComposition = liquidBodyContents[need.ID.WaterIndex];

            // If the water composition is below the min
            // or above the max, it is not drinkable
            if (currentComposition < need.MinThreshold || currentComposition > need.MaxThreshold)
            {
                drinkable = false;
            }

            index++;
        }

        return drinkable;
    }
}