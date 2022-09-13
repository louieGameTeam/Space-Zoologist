using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO have this create the food source item and hold it, then have the store display that info
[CreateAssetMenu]
public class FoodSourceSpecies : ScriptableObject
{
    public ItemID ID => ItemRegistry.FindSpecies(this);
    public int RootRadius => rootRadius;
    public int BaseOutput => baseOutput;
    public Item FoodSourceItem => ID.Data.ShopItem;
    public HashSet<TileType> AccessibleTerrain => needs.FindTraversibleTerrain();
    public int WaterTilesRequired => waterTilesRequired;
    public Vector2Int Size => size;
    public NeedRegistry Needs => needs;
    public int TerrainTilesNeeded => size.x * size.y;

    [SerializeField] private Vector2Int size = new Vector2Int(1, 1); // default to 1 tile big
    [SerializeField] private int waterTilesRequired = default;
    [SerializeField] private int rootRadius = default;
    [SerializeField] private int baseOutput = default;
    [SerializeField]
    [Tooltip("Registry of everything that the food source needs")]
    private NeedRegistry needs = null;
}
