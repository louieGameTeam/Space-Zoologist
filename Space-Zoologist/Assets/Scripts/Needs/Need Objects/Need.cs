using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TerrainNeed : Need
{
    public TerrainNeed(NeedConstructData needConstructData) : base(needConstructData) {}

    protected override NeedType GetNeedType()
    {
        return NeedType.Terrain;
    }
}

[System.Serializable]
public class FoodNeed : Need
{
    public FoodNeed(NeedConstructData needConstructData) : base(needConstructData) {}

    protected override NeedType GetNeedType()
    {
        return NeedType.FoodSource;
    }
}

[System.Serializable]
public class LiquidNeed : Need
{
    private new LiquidNeedConstructData needConstructData;

    public LiquidNeed(NeedConstructData needConstructData) : base(needConstructData) { needConstructData = (LiquidNeedConstructData)needConstructData; }

    protected override NeedType GetNeedType()
    {
        return NeedType.Liquid;
    }

    public float GetFreshThreshold() { return needConstructData.FreshWaterThreshold; }
    public float GetSaltThreshold() { return needConstructData.SaltThreshold; }
    public float GetBacteriaThreshold() { return needConstructData.BacteriaThreshold; }
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
    public bool IsThresholdMet(float value)
    {
        return value >= needConstructData.GetSurvivableThreshold();
    }

    public float GetThreshold()
    {
        return needConstructData.GetSurvivableThreshold();
    }

    public void UpdateNeedValue(float value)
    {
        this.needValue = value;
    }
}
