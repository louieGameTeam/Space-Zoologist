using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    public ItemID ID => ItemRegistry.FindSpecies(this);
    // Getters
    public string SpeciesName => ID.Data.Name.Get(ItemName.Type.English);
    public int TerrainTilesRequired => terrainTilesRequired;
    public int MinFoodRequired => minFoodRequired;
    public int MaxFoodRequired => maxFoodRequired;
    public int GrowthRate => growthRate;
    public int DecayRate => decayRate;
    public float Size => size;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public Sprite Icon => icon;
    public Sprite Sprite => icon;
    public int MoveCost => moveCost;
    public Sprite Representation => representation;
    // TODO setup tile weights for species
    public Dictionary<TileType, byte> TilePreference = default;
    public RuntimeAnimatorController AnimatorController => animatorController;
    public List<TerrainNeedConstructData> TerrainNeeds => terrainNeeds;
    public List<FoodNeedConstructData> FoodNeeds => foodNeeds;
    public List<LiquidNeedConstructData> LiquidNeeds => liquidNeeds;
    public List<PreyNeedConstructData> PreyNeeds => preyNeeds;

    // Values
    [SerializeField] private RuntimeAnimatorController animatorController = default;
    [SerializeField] private int terrainTilesRequired = default;
    [SerializeField] private int minFoodRequired = default;
    [SerializeField] private int maxFoodRequired = default;
    [SerializeField] private int growthRate = 3;
    [SerializeField] private int decayRate = 3;
    [SerializeField] private int moveCost = default;

    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private Sprite icon = default;

    [SerializeField] private List<TerrainNeedConstructData> terrainNeeds = default;
    [SerializeField] private List<FoodNeedConstructData> foodNeeds = default;
    [SerializeField] private List<LiquidNeedConstructData> liquidNeeds = default;
    [SerializeField] private List<PreyNeedConstructData> preyNeeds = default;

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

    public Dictionary<string, Need> SetupNeeds()
    {
        Dictionary<string, Need> needs = new Dictionary<string, Need>();
        
        //Terrain Needs
        foreach (TerrainNeedConstructData need in terrainNeeds)
        {
            needs.Add(need.NeedName, new TerrainNeed(need, this));
        }

        //Food Needs
        foreach (FoodNeedConstructData need in foodNeeds)
        {
            needs.Add(need.NeedName, new FoodNeed(need, minFoodRequired));
        }

        //Water Needs
        foreach (LiquidNeedConstructData need in liquidNeeds)
        {
            if(need.TileNeedThreshold <= 0)
                continue;

            needs.Add("Liquid", new LiquidNeed("Liquid", need));

            if(need.FreshWaterMinThreshold != 0)
                needs.Add("Water", new LiquidNeed("Water", need));

            if(need.FreshWaterMaxThreshold != 1)
                needs.Add("WaterPoison", new LiquidNeed("WaterPoison", need));

            if(need.SaltMinThreshold != 0)
                needs.Add("Salt", new LiquidNeed("Salt", need));

            if(need.SaltMaxThreshold != 1)
                needs.Add("SaltPoison", new LiquidNeed("SaltPoison", need));

            if(need.BacteriaMinThreshold != 0)
                needs.Add("Bacteria", new LiquidNeed("Bacteria", need));

            if(need.BacteriaMaxThreshold != 1)
                needs.Add("BacteriaPoison", new LiquidNeed("BacteriaPoison", need));
        }

        //Prey Needs
        foreach (PreyNeedConstructData need in preyNeeds)
        {
            needs.Add(need.NeedName, new PreyNeed(need));
        }

        return needs;
    }

}