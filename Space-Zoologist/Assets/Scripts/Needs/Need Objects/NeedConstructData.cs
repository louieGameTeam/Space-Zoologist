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
    [SerializeField] private bool isPreferred;

    public TerrainNeedConstructData(string name) 
        : base(name)
    {
    }

    protected override bool getIsPreferred()
    {
        return isPreferred;
    }

    public override float GetSurvivableThreshold() { return -1f; }
}

[System.Serializable]
public class FoodNeedConstructData : NeedConstructData
{
    public float FoodNeedThreshold => foodNeedThreshold;

    [SerializeField] private float foodNeedThreshold;
    [SerializeField] private bool isPreferred;

    public FoodNeedConstructData(string name) 
        : base(name)
    {
    }

    public override float GetSurvivableThreshold()
    {
        return foodNeedThreshold;
    }

    protected override bool getIsPreferred()
    {
        return isPreferred;
    }
}

[System.Serializable]
public class LiquidNeedConstructData : NeedConstructData
{
    public float TileNeedThreshold => tileNeedThreshold;
    public float FreshWaterThreshold => freshWaterThreshold;
    public float SaltThreshold => saltThreshold;
    public float BacteriaThreshold => bacteriaThreshold;

    [SerializeField] private float tileNeedThreshold;
    [SerializeField] private float freshWaterThreshold;
    [SerializeField] private float saltThreshold;
    [SerializeField] private float bacteriaThreshold;

    public LiquidNeedConstructData(string name) 
        : base(name)
    {
    }

    public override float GetSurvivableThreshold()
    {
        return tileNeedThreshold;
    }

    protected override bool getIsPreferred()
    {
        return false;
    }
}

/// <summary>
/// A data object that holds the information to create a Need object.
/// </summary>
[System.Serializable]
public abstract class NeedConstructData
{
    public string NeedName => needName;
    public bool IsPreferred => getIsPreferred();

    [SerializeField] private string needName = default;

    public NeedConstructData(string name)
    {
        this.needName = name;
    }

    public abstract float GetSurvivableThreshold();

    protected abstract bool getIsPreferred();
}