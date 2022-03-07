using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    public ItemID ID => ItemRegistry.FindSpecies(this);
    public int TerrainTilesRequired => terrainTilesRequired;
    public int WaterTilesRequired => waterTilesRequired;
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
    public NeedRegistry Needs => needs;

    // Values
    [SerializeField] private RuntimeAnimatorController animatorController = default;
    [SerializeField] private int terrainTilesRequired = default;
    [SerializeField] private int minFoodRequired = default;
    [SerializeField] private int maxFoodRequired = default;
    [SerializeField] private int waterTilesRequired = default;
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

    [SerializeField]
    [Tooltip("Registry of all the animal's needs")]
    [FormerlySerializedAs("needRegistry")]
    private NeedRegistry needs;

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

    public Dictionary<ItemID, Need> SetupNeeds()
    {
        Dictionary<ItemID, Need> needs = new Dictionary<ItemID, Need>();
        
        //Terrain Needs
        foreach (TerrainNeedConstructData need in terrainNeeds)
        {
            if (!need.ID.IsWater)
            {
                needs.Add(need.ID, new TerrainNeed(need, this));
            }
        }

        //Food Needs
        foreach (FoodNeedConstructData need in foodNeeds)
        {
            needs.Add(need.ID, new FoodNeed(need, minFoodRequired));
        }

        // Water Needs
        foreach (LiquidNeedConstructData need in liquidNeeds)
        {
            needs.Add(need.ID, new LiquidNeed(need));
        }

        //Prey Needs
        foreach (PreyNeedConstructData need in preyNeeds)
        {
            needs.Add(need.ID, new PreyNeed(need));
        }

        return needs;
    }

    public Need GetTerrainWaterNeed()
    {
        TerrainNeedConstructData terrainWaterNeed = terrainNeeds.Find(need => need.ID.IsWater);

        if (terrainWaterNeed != null)
        {
            return new TerrainNeed(terrainWaterNeed, this);
        }
        else return null;
    }
}