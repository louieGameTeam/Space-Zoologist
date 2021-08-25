using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TerrainNeed : Need
{
    private AnimalSpecies animalSpecies;
    private FoodSourceSpecies foodSourceSpecies;

    public TerrainNeed(TerrainNeedConstructData needConstructData, AnimalSpecies species) : base(needConstructData) 
    { 
        animalSpecies = species;
    }

    public TerrainNeed(TerrainNeedConstructData needConstructData, FoodSourceSpecies species) : base(needConstructData) 
    {
        foodSourceSpecies = species;
    }

    protected override NeedType GetNeedType()
    {
        return NeedType.Terrain;
    }

    public override float GetThreshold()
    {
        if(animalSpecies)
            return animalSpecies.TerrainTilesRequired;
        
        if(foodSourceSpecies)
            return Mathf.Pow(foodSourceSpecies.Size, 2);
        
        return needConstructData.GetSurvivableThreshold();
    }
}

[System.Serializable]
public class FoodNeed : Need
{
    private int foodThreshold;

    public FoodNeed(FoodNeedConstructData needConstructData, int minFoodThreshold) : base(needConstructData) 
    {
        foodThreshold = minFoodThreshold;
    }

    protected override NeedType GetNeedType()
    {
        return NeedType.FoodSource;
    }

    public override float GetThreshold()
    {
        return foodThreshold;
    }
}

[System.Serializable]
public class LiquidNeed : Need
{
    private new LiquidNeedConstructData needConstructData;
    private string needType;

    public LiquidNeed(string needType, LiquidNeedConstructData needConstructData) : base(needConstructData) 
    { 
        this.needConstructData = needConstructData; 
        this.needType = needType;
    }

    protected override NeedType GetNeedType()
    {
        return NeedType.Liquid;
    }

    public override float GetThreshold()
    {
        switch(needType)
        {
            case "LiquidTiles":
                return base.GetThreshold();
            case "Water":
                return GetFreshThreshold();
            case "Salt":
                return GetSaltThreshold();
            case "Bacteria":
                return GetBacteriaThreshold();
            default:
                return base.GetThreshold();
        }
    }

    private float GetFreshThreshold() { return needConstructData.FreshWaterThreshold; }
    private float GetSaltThreshold() { return needConstructData.SaltThreshold; }
    private float GetBacteriaThreshold() { return needConstructData.BacteriaThreshold; }

    public override bool IsThresholdMet(float value)
    {
        switch(needType)
        {
            case "LiquidTiles":
                return base.IsThresholdMet(value);
            case "Water":
                return IsFreshThresholdMet(value);
            case "Salt":
                return IsSaltThresholdMet(value);
            case "Bacteria":
                return IsBacteriaThresholdMet(value);
            default:
                return base.IsThresholdMet(value);
        }
    }
    
    private bool IsFreshThresholdMet(float value) { return value >= GetFreshThreshold(); }
    private bool IsSaltThresholdMet(float value) { return value >= GetSaltThreshold(); }
    private bool IsBacteriaThresholdMet(float value) { return value >= GetBacteriaThreshold(); }
}

[System.Serializable]
public abstract class Need
{
    public string NeedName => needName;
    public NeedType NeedType => GetNeedType();
    public Sprite Sprite => sprite;
    public float NeedValue => this.needValue;
    public bool IsPreferred => needConstructData.IsPreferred;

    [SerializeField] private string needName => needConstructData.NeedName;
    [SerializeField] private float needValue = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int severity = 1;
    [SerializeField] private Sprite sprite = default;

    protected NeedConstructData needConstructData;

    protected abstract NeedType GetNeedType();

    protected Need(NeedConstructData needConstructData)
    {
        this.needConstructData = needConstructData;
    }

    /// <summary>
    /// Returns what condition the need is in based on the given need value.
    /// </summary>
    /// <param name="value">The value to compare to the need thresholds</param>
    /// <returns></returns>
    public virtual bool IsThresholdMet(float value)
    {
        return value >= needConstructData.GetSurvivableThreshold();
    }

    public virtual float GetThreshold()
    {
        return needConstructData.GetSurvivableThreshold();
    }

    public virtual void UpdateNeedValue(float value)
    {
        this.needValue = value;
    }
}
