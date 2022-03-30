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
public static class NeedAvailabilityBuilder
{
    #region Public Methods
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

        // Function to help get the list. If it is not in the dictionary yet,
        // we add it first and then return it
        List<NeedAvailabilityItem> Get(Population population)
        {
            if (!populationToItems.ContainsKey(population))
            {
                populationToItems.Add(population, new List<NeedAvailabilityItem>());
            }
            return populationToItems[population];
        }

        // SPECIES

        // Loop from the first species to the second to last species
        for (int i = 0; i < rpm.Populations.Count - 1; i++)
        {
            // Another loop starts at the species after the current
            for (int j = i + 1; j < rpm.Populations.Count; j++)
            {
                // Get the two populations
                Population popA = rpm.Populations[i];
                Population popB = rpm.Populations[j];

                // Get the number of tiles shared
                int sharedTiles = rpm.NumOverlapTiles(popA, popB);

                // Count the number of popA invading popB's space,
                // then the number of popB invading popA's space
                int aInvadesB = Mathf.Min(sharedTiles, popA.Count);
                int bInvadesA = Mathf.Min(sharedTiles, popB.Count);

                // Add availability items for each other species
                Get(popA).Add(new NeedAvailabilityItem(popB.Species.ID, popB.Count, bInvadesA));
                Get(popB).Add(new NeedAvailabilityItem(popA.Species.ID, popA.Count, aInvadesB));
            }
        }

        // FOOD

        // Get the food that is contested over multiple species
        Dictionary<FoodSource, List<Population>> foodCompetition = rpm.FoodCompetition();

        foreach (KeyValuePair<FoodSource, List<Population>> kvp in foodCompetition)
        {
            // Sum the total dominance applied to this food source
            float appliedDominance = kvp
                .Value
                .Sum(pop => pop.FoodDominance);

            // Go through each population competing for this food source
            foreach (Population population in kvp.Value)
            {
                // Compute the proportion of the food that this species gets
                float foodProportionConsumed = population.FoodDominance / appliedDominance;
                NeedAvailabilityItem item = new NeedAvailabilityItem(
                    kvp.Key.Species.ID,
                    1,
                    kvp.Key.FoodOutput * foodProportionConsumed);
                Get(population).Add(item);
            }
        }

        // TERRAIN

        // Get the terrain tiles contested across multiple species
        Dictionary<Vector3Int, List<Population>> terrainCompetition = rpm.TerrainCompetition();

        foreach (KeyValuePair<Vector3Int, List<Population>> kvp in terrainCompetition)
        {
            // Get the type of tile at this position
            TileType tile = GameManager
                .Instance
                .m_tileDataController
                .GetTileData(kvp.Key)
                .currentTile
                .type;

            // Sum all the dominance applied to this tile
            float appliedDominance = kvp
                .Value
                .Sum(pop => pop.Species.GetTerrainDominance(tile));

            // If no dominance is applied to this tile then 
            // there is no need to compute the need availability
            // for this tile
            if (appliedDominance > 0f)
            {
                // Go through each population competing for this tile
                foreach (Population population in kvp.Value)
                {
                    // Compute the proportion of the tile that this species gets for themselves
                    float tileProportionOwned = population.GetTerrainDominance(tile) / appliedDominance;
                    NeedAvailabilityItem item = new NeedAvailabilityItem(
                        ItemRegistry.FindTile(tile),
                        1,
                        tileProportionOwned);
                    Get(population).Add(item);
                }
            }
        }

        // WATER
        // Water is evenly shared among all populations, so dominance is not a factor

        foreach (Population population in rpm.Populations)
        {
            List<float[]> accessibleLiquids = rpm.GetLiquidComposition(population);
            ItemID waterID = ItemRegistry.FindAnyNameContains("Water");

            // Convert accessible liquids to need availability items
            IEnumerable<NeedAvailabilityItem> waterItems = accessibleLiquids
                .Select(comp => new NeedAvailabilityItem(waterID, 1, 1, new LiquidBodyContent(comp)));

            // Add the water items to the list in the dictionary
            Get(population).AddRange(waterItems);
        }

        // Go through the lists of need availability and add them to the result
        foreach (KeyValuePair<Population, List<NeedAvailabilityItem>> kvp in populationToItems)
        {
            result.Add(kvp.Key, new NeedAvailability(kvp.Value.ToArray()));
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
            needAvailabilityItems.Add(new NeedAvailabilityItem(
                tileID, 
                Mathf.Min(1, terrainCountsByTileType[i]), 
                terrainCountsByTileType[i]));
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
            .Select(composition => new NeedAvailabilityItem(waterID, 1, 1, new LiquidBodyContent(composition)));
        needAvailabilityItems.AddRange(waterItems);
             
        return new NeedAvailability(needAvailabilityItems.ToArray());
    }
    #endregion
}
