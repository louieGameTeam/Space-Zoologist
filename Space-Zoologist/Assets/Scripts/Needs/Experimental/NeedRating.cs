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
    public bool FoodNeedIsMet => foodRating >= 1;
    public bool TerrainNeedIsMet => terrainRating >= 1;
    public bool WaterNeedIsMet => waterRating >= 1;
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
    #endregion

    #region Constructors
    public NeedRating(int predatorCount, float foodRating, float terrainRating, float waterRating)
    {
        this.predatorCount = predatorCount;
        this.foodRating = foodRating;
        this.terrainRating = terrainRating;
        this.waterRating = waterRating;
    }
    #endregion
}
