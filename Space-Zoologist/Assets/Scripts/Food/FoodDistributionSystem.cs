using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FoodSourceManager uses AddFoodSource
public class FoodDistributionSystem
{
    private LevelData levelData = default;
    private Dictionary<FoodSourceSpecies, FoodSourceNeedSystem> foodSourceNeedSystems = new Dictionary<FoodSourceSpecies, FoodSourceNeedSystem>();
    public IEnumerable<FoodSourceNeedSystem> FoodSourceNeedSystems => foodSourceNeedSystems.Values;
    private readonly ReservePartitionManager rpm = null; 

    public FoodDistributionSystem(LevelData levelData, ReservePartitionManager rpm)
    {
        this.levelData = levelData;
        this.rpm = rpm;
        foreach (FoodSourceSpecies foodSourceSpecies in levelData.FoodSources)
        {
            FoodSourceNeedSystem foodSourceNeedSystem = new FoodSourceNeedSystem(foodSourceSpecies.SpeciesName, rpm);
            //Debug.Log($"Making FoodSourceNeedSystem for {foodSourceSpecies.SpeciesName}");
            foodSourceNeedSystems.Add(foodSourceSpecies, foodSourceNeedSystem);
        }
    }

    public void AddFoodSource(FoodSource foodSource)
    {
        Debug.Assert(levelData.FoodSources.Contains(foodSource.Species), "Added a food source to food distribution whose species is not in the LevelData");
        foodSourceNeedSystems[foodSource.Species].AddFoodSource(foodSource);
    }

    public void AddFoodSources(IEnumerable<FoodSource> newFoodSources)
    {
        foreach (FoodSource newFoodSource in newFoodSources)
        {
            Debug.Assert(levelData.FoodSources.Contains(newFoodSource.Species), "Added a food source to food distribution whose species is not in the LevelData");
            foodSourceNeedSystems[newFoodSource.Species].AddFoodSource(newFoodSource);
        }
    }

    public void UpdateSystems()
    {
        //Debug.Log("Update FoodDistribution");
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in FoodSourceNeedSystems)
        {
            foodSourceNeedSystem.UpdateSystem();
        }
    }
}
