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
    #region Public Typedefs
    public class LiquidCompositionComparer : IEqualityComparer<float[]>
    {
        public bool Equals(float[] a, float[] b)
        {
            return a.SequenceEqual(b);
        }
        public int GetHashCode(float[] a)
        {
            return a[0].GetHashCode() + a[1].GetHashCode() + a[2].GetHashCode();
        }
    }
    #endregion

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

        // Maps the food item to the number that is available
        Dictionary<ItemID, float> foodToAmount = new Dictionary<ItemID, float>();

        foreach (FoodSource source in sourcesAvailable)
        {
            // Get the id of the food source
            ItemID sourceID = source.Species.ID;

            // Check if this already has the key
            if (foodToAmount.ContainsKey(sourceID))
            {
                foodToAmount[sourceID] += source.FoodOutput;
            }
            else foodToAmount[sourceID] = source.FoodOutput;
        }

        // Add food items to the list
        IEnumerable<NeedAvailabilityItem> foodItems = foodToAmount
            .Select(kvp => new NeedAvailabilityItem(kvp.Key, (int)kvp.Value));
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

        // Convert accessible liquids to need availability items
        IEnumerable<NeedAvailabilityItem> waterItems = ConvertLiquidCompositions(accessibleLiquids);
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
        ReservePartitionManager rpm = GameManager.Instance.m_reservePartitionManager;

        // Go through every population in the reserve partition manager
        foreach (Population population in rpm.Populations)
        {
            result[population] = Build(population);
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

        // Convert each kvp in the dictionary to a need availability item
        // and add it to the list of items
        IEnumerable<NeedAvailabilityItem> waterItems = ConvertLiquidCompositions(liquidCompositionsInRange);
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
