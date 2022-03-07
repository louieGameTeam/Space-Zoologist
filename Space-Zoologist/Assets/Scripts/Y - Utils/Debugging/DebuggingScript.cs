using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            FoodSourceManager foodSourceManager = GameManager.Instance.m_foodSourceManager;
            string message = "Food source ratings (experimental):";

            // Go through each food source in the manager
            foreach (FoodSource foodSource in foodSourceManager.FoodSources)
            {
                // Get food source need availability
                NeedAvailability needAvailability = NeedAvailabilityFactory.BuildFoodSourceNeedAvailability(foodSource);
                float terrainRating = NeedSystem.TerrainRating(foodSource.Species.Needs, needAvailability, foodSource.Species.TerrainTilesNeeded);
                float waterRating = NeedSystem.WaterRating(foodSource.Species.Needs, needAvailability, foodSource.Species.WaterTilesRequired);

                message += $"\n\t{foodSource.Species} at {foodSource.GetCellPosition()}: " +
                    $"\n\t\tTerrain rating: {terrainRating}" +
                    $"\n\t\tWater rating: {waterRating}";
            }

            Debug.Log(message);
        }
    }
}
