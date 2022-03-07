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
    public static Dictionary<Population, NeedAvailability> BuildPopulationNeedAvailability()
    {
        Dictionary<Population, NeedAvailability> result = new Dictionary<Population, NeedAvailability>();

        return result;
    }
    public static NeedAvailability BuildFoodSourceNeedAvailability(FoodSource foodSource)
    {
        List<NeedAvailabilityItem> needAvailabilityItems = new List<NeedAvailabilityItem>();

        // Count the terrain tiles under this food source
        int[] terrainCountsByTileType = GameManager
            .Instance
            .m_tileDataController
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
        List<float[]> liquidCompositionsInRange = GameManager
            .Instance
            .m_tileDataController
            .GetLiquidCompositionWithinRange(
                foodSource.GetCellPosition(),
                foodSource.Species.Size,
                foodSource.Species.RootRadius);

        // Map liquid composition to the number of times it appears
        Dictionary<float[], int> liquidCompositionCounts = new Dictionary<float[], int>(new LiquidCompositionComparer());

        // Go through each composition and add it to the dictionary
        foreach (float[] composition in liquidCompositionsInRange)
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
        IEnumerable<NeedAvailabilityItem> waterItems = liquidCompositionCounts
            .Select(kvp => new NeedAvailabilityItem(freshWaterID, kvp.Value, kvp.Key));
        needAvailabilityItems.AddRange(waterItems);
             
        return new NeedAvailability(needAvailabilityItems.ToArray());
    }
    #endregion
}
