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
    [SerializeField]
    [ItemIDFilter("Water")]
    [Tooltip("The type of water needed by the species")]
    private ItemID id;
    [Range(0, 1)] 
    [SerializeField]
    [Tooltip("The minimum amount of this water type needed " +
        "to be drinkable by this species")]
    private float minThreshold = 0;
    [Range(0, 1)] 
    [SerializeField]
    [Tooltip("The maximum amount of this water type tolerated " +
        "to be drinkable by this species")]
    private float maxThreshold = 1;

    public override float GetSurvivableThreshold()
    {
        return -1;
    }
    protected override bool getIsPreferred()
    {
        return false;
    }
    protected override ItemID getID()
    {
        return id;
    }
}

[System.Serializable]
public class PreyNeedConstructData : NeedConstructData
{
    [SerializeField]
    [Tooltip("ID of the animal that this animal devours/symbiots with")]
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