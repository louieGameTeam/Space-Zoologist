using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to build objects of type 'NeedAvailability'
/// </summary>
/// <remarks>
/// Populations compete with each other for resources.
/// The complex task of computing which populations get
/// what resources is handled by this class
/// </remarks>
public static class NeedAvailabilityFactory
{
    #region Public Methods
    /// <summary>
    /// Build the need availability for a single population.
    /// </summary>
    /// <remarks>
    /// This function does NOT take species dominance into account
    /// </remarks>
    /// <param name="population"></param>
    /// <returns></returns>
    public static NeedAvailability Build(Population population)
    {
        List<NeedAvailabilityItem> items = new List<NeedAvailabilityItem>();
        ReservePartitionManager rpm = GameManager.Instance.m_reservePartitionManager;

        // FOOD SOURCE

        // Get a list of available food sources
        List<FoodSource> sourcesAvailable = rpm.GetAccessibleFoodSources(population);
        IEnumerable<NeedAvailabilityItem> foodItems = sourcesAvailable
            .Select(food => new NeedAvailabilityItem(food.Species.ID, (int)food.FoodOutput));
        items.AddRange(foodItems);

        // TERRAIN

        // Get the counts for each tile type for this population
        int[] tileCountByType = rpm.GetTypesOfTiles(population);

        // Go through each tile count
        for (int tileType = 0; tileType < tileCountByType.Length; tileType++)
        {
            int count = tileCountByType[tileType];

            // If the population can access some tiles of this type,
            // then add a need availability item to the list
            if (count > 0)
            {
                TileType tile = (TileType)tileType;
                ItemID tileID = ItemRegistry.FindTile(tile);
                items.Add(new NeedAvailabilityItem(tileID, count));
            }
        }

        // LIQUID

        List<float[]> accessibleLiquids = rpm.GetLiquidComposition(population);
        ItemID waterID = ItemRegistry.FindAnyNameContains("Water");

        // Convert accessible liquids to need availability items
        IEnumerable<NeedAvailabilityItem> waterItems = accessibleLiquids
            .Select(comp => new NeedAvailabilityItem(waterID, 1, comp));
        items.AddRange(waterItems);

        return new NeedAvailability(items.ToArray());
    }
    /// <summary>
    /// Build the distribution of need availability across all populations.
    /// </summary>
    /// <remarks>
    /// This function takes poulation dominance into account, distributing needs available
    /// based on the dominance of each species over the other species
    /// </remarks>
    /// <returns></returns>
    public static Dictionary<Population, NeedAvailability> BuildDistribution()
    {
        Dictionary<Population, NeedAvailability> result = new Dictionary<Population, NeedAvailability>();
        Dictionary<Population, List<NeedAvailabilityItem>> populationToItems = new Dictionary<Population, List<NeedAvailabilityItem>>();
        ReservePartitionManager rpm = GameManager.Instance.m_reservePartitionManager;

        //foreach (Population population in rpm.Populations)
        //{
        //    result[population] = Build(population);
        //}

        // Get the food that is contested over multiple species
        Dictionary<FoodSource, List<Population>> foodCompetition = rpm.FoodCompetition();

        foreach (KeyValuePair<FoodSource, List<Population>> kvp in foodCompetition)
        {
            // Sum the total dominance applied to this food source
            float appliedDominance = kvp.Value.Sum(pop => pop.FoodDominance);
        }

        return result;
    }
    /// <summary>
    /// Build the availability of needs for a single food source
    /// </summary>
    /// <param name="foodSource"></param>
    /// <returns></returns>
    public static NeedAvailability Build(FoodSource foodSource)
    {
        List<NeedAvailabilityItem> needAvailabilityItems = new List<NeedAvailabilityItem>();
        TileDataController tileDataController = GameManager.Instance.m_tileDataController;

        // Count the terrain tiles under this food source
        int[] terrainCountsByTileType = tileDataController
            .CountOfTilesUnderSpecies(
                foodSource.GetCellPosition(),
                foodSource.Species);

        // Go through each terrain count and add it to the list
        for (int i = 0; i < terrainCountsByTileType.Length; i++)
        {
            TileType tileType = (TileType)i;
            ItemID tileID = ItemRegistry.FindTile(tileType);
            needAvailabilityItems.Add(new NeedAvailabilityItem(tileID, terrainCountsByTileType[i]));
        }

        // Get a list of the liquid compositions in range
        List<float[]> liquidCompositionsInRange = tileDataController
            .GetLiquidCompositionWithinRange(
                foodSource.GetCellPosition(),
                foodSource.Species.Size,
                foodSource.Species.RootRadius);
        ItemID waterID = ItemRegistry.FindAnyNameContains("Water");

        // Add the water items
        IEnumerable<NeedAvailabilityItem> waterItems = liquidCompositionsInRange
            .Select(composition => new NeedAvailabilityItem(waterID, 1, composition));
        needAvailabilityItems.AddRange(waterItems);
             
        return new NeedAvailability(needAvailabilityItems.ToArray());
    }
    #endregion

    #region Private Methods
    private static IEnumerable<NeedAvailabilityItem> ConvertLiquidCompositions(List<float[]> liquidCompositions)
    {
        // Map liquid composition to the number of times it appears
        Dictionary<float[], int> liquidCompositionCounts = new Dictionary<float[], int>(new LiquidCompositionComparer());

        // Go through each composition and add it to the dictionary
        foreach (float[] composition in liquidCompositions)
        {
            if (liquidCompositionCounts.ContainsKey(composition))
            {
                liquidCompositionCounts[composition]++;
            }
            else liquidCompositionCounts.Add(composition, 1);
        }

        // Get the id of the water
        ItemID freshWaterID = ItemRegistry.FindAnyNameContains("Water");

        // Convert each kvp in the dictionary to a need availability item
        // and add it to the list of items
        return liquidCompositionCounts
            .Select(kvp => new NeedAvailabilityItem(freshWaterID, kvp.Value, kvp.Key));
    }
    #endregion
}
