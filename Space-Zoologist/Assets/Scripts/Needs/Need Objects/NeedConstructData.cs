using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Each NeedType holds a list of unique needs
public enum NeedType { Terrain, Atmosphere, Density, FoodSource, Liquid, Species, Temperature, Symbiosis, Prey };
public enum NeedCondition { Bad, Neutral, Good }

[System.Serializable]
public class TerrainNeedConstructData : NeedConstructData
{
    [SerializeField] private bool isPreferred;
    [SerializeField]
    [Tooltip("ID of the terrain tile that this species needs")]
    [ItemIDFilter(ItemRegistry.Category.Tile)]
    private ItemID tileID;

    protected override bool getIsPreferred()
    {
        return isPreferred;
    }
    protected override ItemID getID()
    {
        return tileID;
    }

    public override float GetSurvivableThreshold() { return -1f; }
}

[System.Serializable]
public class FoodNeedConstructData : NeedConstructData
{
    [SerializeField] private bool isPreferred;
    [SerializeField]
    [Tooltip("ID of the food that this animal can consume")]
    [ItemIDFilter(ItemRegistry.Category.Food)]
    private ItemID foodID;

    public override float GetSurvivableThreshold()
    {
        return -1;
    }

    protected override bool getIsPreferred()
    {
        return isPreferred;
    }
    protected override ItemID getID()
    {
        return foodID;
    }
}

[System.Serializable]
public class LiquidNeedConstructData : NeedConstructData
{
    public float TileNeedThreshold => tileNeedThreshold;
    public float FreshWaterMinThreshold => freshWaterMinThreshold;
    public float FreshWaterMaxThreshold => freshWaterMaxThreshold;
    public float SaltMinThreshold => saltMinThreshold;
    public float SaltMaxThreshold => saltMaxThreshold;
    public float BacteriaMinThreshold => bacteriaMinThreshold;
    public float BacteriaMaxThreshold => bacteriaMaxThreshold;

    [SerializeField] private float tileNeedThreshold;
    [Range(0,1)] [SerializeField] private float freshWaterMinThreshold;
    [Range(0,1)] [SerializeField] private float freshWaterMaxThreshold = 1;
    [Range(0,1)] [SerializeField] private float saltMinThreshold;
    [Range(0,1)] [SerializeField] private float saltMaxThreshold = 1;
    [Range(0,1)] [SerializeField] private float bacteriaMinThreshold;
    [Range(0,1)] [SerializeField] private float bacteriaMaxThreshold = 1;


    [Range(0, 1)] [SerializeField] private float minThreshold = 0;
    [Range(0, 1)] [SerializeField] private float maxThreshold = 1;



    public override float GetSurvivableThreshold()
    {
        return tileNeedThreshold;
    }

    protected override bool getIsPreferred()
    {
        return false;
    }
    protected override ItemID getID()
    {
        return ItemRegistry.FindHasName("Fresh Water");
    }
}

[System.Serializable]
public class PreyNeedConstructData : NeedConstructData
{
    [SerializeField]
    [Tooltip("ID of the animal that this animal devours")]
    [ItemIDFilter(ItemRegistry.Category.Species)]
    private ItemID preyID;

    public override float GetSurvivableThreshold() { return 0; }
    protected override ItemID getID() => preyID;
    protected override bool getIsPreferred() { return false; }
}

/// <summary>
/// A data object that holds the information to create a Need object.
/// </summary>
[System.Serializable]
public abstract class NeedConstructData
{
    public ItemID ID => getID();
    public bool IsPreferred => getIsPreferred();

    public abstract float GetSurvivableThreshold();
    protected abstract ItemID getID();
    protected abstract bool getIsPreferred();
}