using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Each NeedType holds a list of unique needs
public enum NeedType { Terrain, Atmosphere, Density, FoodSource, Liquid, Species, Temperature, Symbiosis };
public enum NeedCondition { Bad, Neutral, Good }

[System.Serializable]
public class TerrainNeedConstructData : NeedConstructData
{
    public float TerrainNeedThreshold => terrainNeedThreshold;

    [SerializeField] private float terrainNeedThreshold;

    public TerrainNeedConstructData(string name) 
        : base(name)
    {
        
    }

    public override float GetSurvivableThreshold()
    {
        return terrainNeedThreshold;
    }
}

[System.Serializable]
public class FoodNeedConstructData : NeedConstructData
{
    public float FoodNeedThreshold => foodNeedThreshold;

    [SerializeField] private float foodNeedThreshold;

    public FoodNeedConstructData(string name) 
        : base(name)
    {
        
    }

    public override float GetSurvivableThreshold()
    {
        return foodNeedThreshold;
    }
}

[System.Serializable]
public class LiquidNeedConstructData : NeedConstructData
{
    public float LiquidTileNeedThreshold => liquidTileNeedThreshold;
    public float FreshWaterThreshold => freshWaterThreshold;
    public float SaltThreshold => saltThreshold;
    public float BacteriaThreshold => bacteriaThreshold;

    [SerializeField] private float liquidTileNeedThreshold;
    [SerializeField] private float freshWaterThreshold;
    [SerializeField] private float saltThreshold;
    [SerializeField] private float bacteriaThreshold;

    public LiquidNeedConstructData(string name) 
        : base(name)
    {
        
    }

    public override float GetSurvivableThreshold()
    {
        return liquidTileNeedThreshold;
    }
}

/// <summary>
/// A data object that holds the information to create a Need object.
/// </summary>
[System.Serializable]
public abstract class NeedConstructData
{
    public string NeedName => needName;
    public bool IsPreferred => isPreferred;

    [SerializeField] private string needName = default;
    [SerializeField] private bool isPreferred = false;

    public NeedConstructData(string name)
    {
        this.needName = name;
    }

    public abstract float GetSurvivableThreshold();
}