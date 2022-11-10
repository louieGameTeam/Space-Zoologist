using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rating of how well the species is doing in different categories
/// </summary>
[System.Serializable]
public class NeedRating
{
    #region Public Properties
    public int PredatorCount => predatorCount;
    public int PreyCount => preyCount;
    public float FoodRating => preyCount > 0 ? 2 : foodRating;
    public float TerrainRating => terrainRating;
    public float WaterRating => waterRating;
    public float FriendRating => friendRating;
    public float TreeRating => treeRating;
    
    public bool HasFoodNeed => !float.IsNaN(foodRating);
    public bool HasTerrainNeed => !float.IsNaN(terrainRating);
    public bool HasWaterNeed => !float.IsNaN(waterRating);
    public bool HasFriendNeed => !float.IsNaN(friendRating);
    public bool HasTreeNeed => !float.IsNaN(treeRating);

    public bool FoodNeedIsMet => foodRating >= 1;
    public bool TerrainNeedIsMet => terrainRating >= 1;
    public bool WaterNeedIsMet => waterRating >= 1;
    public bool FriendNeedIsMet => friendRating >= 1;
    public bool TreeNeedIsMet => treeRating >= 1;
    public bool AllNeedsAreMet => (!HasFoodNeed || FoodNeedIsMet) &&
        (!HasTerrainNeed || TerrainNeedIsMet) &&
        (!HasWaterNeed || WaterNeedIsMet) &&
        (!HasFriendNeed || FriendNeedIsMet) &&
        (!HasTreeNeed || TreeNeedIsMet);
    #endregion

    #region Private Fields
    [SerializeField]
    [Tooltip("Number of species hostile to this species in the area")]
    private int predatorCount;
    [SerializeField]
    [Tooltip ("Number of species consumable by this species in the area")]
    private int preyCount;
    [SerializeField]
    [Tooltip("Rating of how well the food need is met, from 0 - 2")]
    private float foodRating;
    [SerializeField]
    [Tooltip("Rating of how well the terrain need is met, from 0 - 2")]
    private float terrainRating;
    [SerializeField]
    [Tooltip("Rating of how well the water need is met, from 0 - 2")]
    private float waterRating;
    [SerializeField]
    [Tooltip("Rating of how well the friend need is met, from 0 - 2")]
    private float friendRating;
    [SerializeField]
    [Tooltip("Rating of how well the tree need is met, from 0 - 2")]
    private float treeRating;
    #endregion

    #region Constructors
    public NeedRating(int predatorCount, int preyCount, float foodRating, float terrainRating, float waterRating, float friendRating, float treeRating)
    {
        this.predatorCount = predatorCount;
        this.preyCount = preyCount;
        this.foodRating = foodRating;
        this.terrainRating = terrainRating;
        this.waterRating = waterRating;
        this.friendRating = friendRating;
        this.treeRating = treeRating;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Compute the change rate for a hypothetical population, 
    /// assuming that it has this need rating
    /// </summary>
    /// <param name="populationSize"></param>
    /// <returns></returns>
    public float ComputeChangeRate(int populationSize)
    {
        float totalRating = 0;
        int appliedRatings = 0;

        float predatorPreyRatio = 0;
        if (predatorCount > 0)
        {
            predatorPreyRatio = -predatorCount / (float) populationSize;
            appliedRatings++;
        }    

        if (AllNeedsAreMet)
        {
            // Apply the ratings to the local variables
            ApplyRatingIf (HasFoodNeed, FoodRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasTerrainNeed, TerrainRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasWaterNeed, WaterRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasTreeNeed, TreeRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasFriendNeed, FriendRating - 1, ref totalRating, ref appliedRatings);
        }
        else
        {
            // Apply the ratings to the local variables
            ApplyRatingIf (HasFoodNeed && !FoodNeedIsMet, FoodRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasTerrainNeed && !TerrainNeedIsMet, TerrainRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasWaterNeed && !WaterNeedIsMet, WaterRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasTreeNeed && !TreeNeedIsMet, TreeRating - 1, ref totalRating, ref appliedRatings);
            ApplyRatingIf (HasFriendNeed && !FriendNeedIsMet, FriendRating - 1, ref totalRating, ref appliedRatings);
        }

        // If some ratings were applied then compute the average
        if (appliedRatings > 0)
        {
            float avgRating = totalRating / appliedRatings;
            return Mathf.Max (predatorPreyRatio + avgRating, -1f);
        }
        // If no ratings were applied then population size will not change at all
        else
        {
            return 0;
        }
    }
    #endregion

    #region Private Methods
    private void ApplyRatingIf(bool condition, float rating, ref float totalRating, ref int appliedRatings)
    {
        if (condition)
        {
            totalRating += rating;
            appliedRatings++;
        }
    }
    #endregion
}
