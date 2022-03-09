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
    public float FoodRating => foodRating;
    public float TerrainRating => terrainRating;
    public float WaterRating => waterRating;
    #endregion

    #region Private Fields
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
    public NeedRating(float foodRating, float terrainRating, float waterRating)
    {
        this.foodRating = foodRating;
        this.terrainRating = terrainRating;
        this.waterRating = waterRating;
    }
    #endregion
}
