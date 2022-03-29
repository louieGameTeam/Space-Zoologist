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
    public float FoodRating => foodRating;
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
    #endregion

    #region Private Fields
    [SerializeField]
    [Tooltip("Number of species hostile to this species in the area")]
    private int predatorCount;
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
    public NeedRating(int predatorCount, float foodRating, float terrainRating, float waterRating, float friendRating, float treeRating)
    {
        this.predatorCount = predatorCount;
        this.foodRating = foodRating;
        this.terrainRating = terrainRating;
        this.waterRating = waterRating;
        this.friendRating = friendRating;
        this.treeRating = treeRating;
    }
    #endregion
}
