using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    public ItemID ID => ItemRegistry.FindSpecies(this);
    public Item AnimalShopItem => ID.Data.ShopItem;
    public float FoodDominance => AnimalDominance.GetFoodDominance(ID).Dominance;
    public int TerrainTilesRequired => terrainTilesRequired;
    public int WaterTilesRequired => waterTilesRequired;
    public int MinFoodRequired => minFoodRequired;
    public int MaxFoodRequired => maxFoodRequired;
    public int TreesRequired => treesRequired;
    public int GrowthRate => growthRate;
    public int DecayRate => decayRate;
    public float Size => size;
    public HashSet<TileType> NeededTerrain => needs.FindNeededTerrain();
    public HashSet<TileType> TraversableOnlyTerrain => needs.FindTraversableOnlyTerrain();
    public HashSet<TileType> AccessibleTerrain => needs.FindAccessibleTerrain();
    public NeedData[] RequiredTreeNeeds => needs.FindTreeNeeds();
    public Sprite Icon => icon;
    public Sprite Sprite => icon;
    public int MoveCost => moveCost;
    public Sprite Representation => representation;
    // TODO setup tile weights for species
    public Dictionary<TileType, byte> TilePreference = default;
    public RuntimeAnimatorController AnimatorController => animatorController;
    public NeedRegistry Needs => needs;

    // Values
    [SerializeField] private RuntimeAnimatorController animatorController = default;
    [SerializeField] private int terrainTilesRequired = default;
    [SerializeField] private int minFoodRequired = default;
    [SerializeField] private int maxFoodRequired = default;
    [SerializeField] private int treesRequired = default;
    [SerializeField] private int waterTilesRequired = default;
    [SerializeField] private int growthRate = 3;
    [SerializeField] private int decayRate = 3;
    [SerializeField] private int moveCost = default;

    [SerializeField] private float size = default;
    [SerializeField] private Sprite icon = default;

    [SerializeField]
    [Tooltip("Registry of all the animal's needs")]
    [FormerlySerializedAs("needRegistry")]
    private NeedRegistry needs = null;

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

    public float GetTerrainDominance(TileType tile)
    {
        return AnimalDominance.TerrainDominance.GetAnimalDominance(tile, ID).Dominance;
    }
}