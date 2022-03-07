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
            PopulationManager populationManager = GameManager.Instance.m_populationManager;

            if (populationManager.Populations.Count > 0)
            {
                Population population = populationManager.Populations[0];

                // On the first level, the goat has 28 dirt, 36 grass and 21 maple fruits
                NeedAvailability goatAvailability = new NeedAvailability(
                    new NeedAvailabilityItem(ItemRegistry.FindHasName("Dirt"), 28),
                    new NeedAvailabilityItem(ItemRegistry.FindHasName("Grass"), 36),
                    new NeedAvailabilityItem(new ItemID(ItemRegistry.Category.Food, 0), 21));

                float terrainRating = NeedSystem.TerrainRating(
                    population.species.Needs, 
                    goatAvailability, 
                    population.species.TerrainTilesRequired * population.Count);
                float foodRating = NeedSystem.FoodRating(
                    population.species.Needs, 
                    goatAvailability, 
                    population.species.MinFoodRequired * population.Count, 
                    population.species.MaxFoodRequired * population.Count);

                Debug.Log($"Population {population.species} ratings (experimental):" +
                    $"\n\tFood Rating: {foodRating}" + 
                    $"\n\tTerrain Rating: {terrainRating}");
            }
        }
    }
}
