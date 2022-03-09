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
    private NeedDataJaggedArray needData;
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
    public HashSet<TileType> FindTraversibleTerrain()
    {
        // Get all tile types on traversible terrain
        IEnumerable<TileType> tileTypes = GetNeedsWithCategory(ItemRegistry.Category.Tile)
            .Where(need => need.Needed)
            .Where(need => !need.ID.IsWater || need.UseAsTerrainNeed)
            .Select(need => need.ID.Data.Tile)
            .Distinct();

        return new HashSet<TileType>(tileTypes);
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
            .OrderByDescending(need => need.Preferred)
            .ToArray();
    }
    public NeedData[] FindTerrainNeeds()
    {
        return GetNeedsWithCategory(ItemRegistry.Category.Tile)
            .Where(need => need.Needed)
            .Where(need => !need.TraversibleOnly)
            .Where(need => !need.ID.IsWater || need.UseAsTerrainNeed)
            .OrderByDescending(need => need.Preferred)
            .ToArray();
    }
    public NeedData[] FindAllNeeded()
    {
        return FindAll(need => need.Needed);
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
