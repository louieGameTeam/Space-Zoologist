using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registers all of the species needs
/// </summary>
/// <remarks>
/// The species has a need for every ItemID in the ItemRegistry.
/// To check if a particular need applies to this species,
/// use NeedData.Needed
/// </remarks>
[Serializable]
public class NeedRegistry
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of all needs for this species")]
    [ParallelItemRegistry("needArrays", "needs")]
    private NeedDataJaggedArray needData = null;
    #endregion

    #region Public Methods
    public NeedData Get(ItemID itemID)
    {
        return GetNeedsWithCategory(itemID.Category)[itemID.Index];
    }
    public NeedData[] GetNeedsWithCategory(ItemRegistry.Category category)
    {
        return needData.NeedArrays[(int)category].Needs;
    }
    public bool WaterIsDrinkable(float[] composition)
    {
        bool AcceptableWaterRange(NeedData need)
        {
            float currentComposition = composition[need.ID.WaterIndex];

            // Check if this water composition is in range of the water need
            return currentComposition >= need.Minimum && currentComposition <= need.Maximum;
        }

        // Get all the water needs
        NeedData[] waterNeeds = FindWaterNeeds();

        // Water is drinkable if each component of water composition
        // is in range of the need
        return waterNeeds.All(AcceptableWaterRange);
    }
    #endregion

    #region Find Methods

    public bool TerrainIsNeeded(TileType tile)
    {
        NeedData[] terrainNeeds = FindTerrainNeeds();
        int index = Array.FindIndex(terrainNeeds, need => need.ID.Data.Tile == tile);
        return index >= 0 && index < terrainNeeds.Length;
    }
    public HashSet<TileType> FindNeededTerrain()
    {
        // Get all tile types on traversable terrain
        IEnumerable<TileType> tileTypes = GetNeedsWithCategory(ItemRegistry.Category.Tile)
            .Where(IsNeededTileOnly)
            .Select(need => need.ID.Data.Tile)
            .Distinct();

        return new HashSet<TileType>(tileTypes);
    }

    /// <summary>
    /// Accessible terrain is either needed OR traversable only
    /// </summary>
    /// <returns></returns>
    public HashSet<TileType> FindAccessibleTerrain()
    {
        IEnumerable<TileType> tileTypes = GetNeedsWithCategory(ItemRegistry.Category.Tile)
            .Where(need => IsTraversableTileOnly(need) || IsNeededTileOnly(need))
            .Select(need => need.ID.Data.Tile)
            .Distinct();
        
        return new HashSet<TileType>(tileTypes);
    }
    
    public HashSet<TileType> FindTraversableOnlyTerrain()
    {
        // Get all tile types on traversable terrain
        IEnumerable<TileType> tileTypes = GetNeedsWithCategory(ItemRegistry.Category.Tile)
            .Where(IsTraversableTileOnly)
            .Select(need => need.ID.Data.Tile)
            .Distinct();

        return new HashSet<TileType>(tileTypes);
    }

    private bool IsNeededTileOnly(NeedData need)
    {
        bool isNeededLiquid = need.ID.IsWater && need.UseAsTerrainNeed && !need.TraversableOnly;
        bool isNeededTerrain = need.Needed && !need.TraversableOnly;
        return isNeededLiquid || isNeededTerrain;
    }
    
    private bool IsTraversableTileOnly(NeedData need)
    {
        bool isTraversableOnlyLiquid = need.ID.IsWater && need.UseAsTerrainNeed && need.TraversableOnly;
        bool isTraversableOnlyTerrain = need.TraversableOnly;
        return isTraversableOnlyLiquid || isTraversableOnlyTerrain;
    }

    public NeedData[] FindPredatorNeeds()
    {
        return GetNeedsWithCategory(ItemRegistry.Category.Species)
            .Where(need => need.Needed)
            .Where(need => need.SpeciesNeedType == SpeciesNeedType.Predator)
            .ToArray();
    }
    public NeedData[] FindFriendNeeds()
    {
        return GetNeedsWithCategory(ItemRegistry.Category.Species)
            .Where(need => need.Needed)
            .Where(need => need.SpeciesNeedType == SpeciesNeedType.Friend)
            .OrderByDescending(need => need.Preferred)
            .ToArray();
    }
    public NeedData[] FindWaterNeeds()
    {
        return GetNeedsWithCategory(ItemRegistry.Category.Tile)
            .Where(need => need.Needed)
            .Where(need => need.ID.IsWater)
            .Where(need => need.ID.WaterIndex != 0 || need.UseAsWaterNeed)
            .ToArray();
    }
    public NeedData[] FindFoodNeeds()
    {
        return GetNeedsWithCategory(ItemRegistry.Category.Food)
            .Where(need => need.Needed)
            .Where(need => need.FoodNeedType == FoodNeedType.Food)
            .OrderByDescending(need => need.Preferred)
            .ToArray();
    }
    public NeedData[] FindTreeNeeds()
    {
        return GetNeedsWithCategory(ItemRegistry.Category.Food)
            .Where(need => need.Needed)
            .Where(need => need.FoodNeedType == FoodNeedType.Tree)
            .OrderByDescending(need => need.Preferred)
            .ToArray();
    }
    public NeedData[] FindTerrainNeeds()
    {
        return GetNeedsWithCategory(ItemRegistry.Category.Tile)
            .Where(need => need.Needed)
            .Where(need => !need.TraversableOnly)
            .Where(need => !need.ID.IsWater || need.UseAsTerrainNeed)
            .OrderByDescending(need => need.Preferred)
            .ToArray();
    }
    public NeedData[] FindAll(Predicate<NeedData> predicate)
    {
        List<NeedData> list = new List<NeedData>();

        for (int category = 0; category < needData.NeedArrays.Length; category++)
        {
            // Get the current list of need datas
            NeedData[] needDatas = needData.NeedArrays[category].Needs;

            // Go through each need data in the array
            for (int index = 0; index < needDatas.Length; index++)
            {
                // If the predicate is true then add it to the list
                if (predicate.Invoke(needDatas[index]))
                {
                    list.Add(needDatas[index]);
                }
            }
        }

        return list.ToArray();
    }
    #endregion
}
