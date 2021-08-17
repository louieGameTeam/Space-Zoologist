using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO have this create the food source item and hold it, then have the store display that info
[CreateAssetMenu]
public class FoodSourceSpecies : ScriptableObject
{
    public string SpeciesName => speciesName;
    public int RootRadius => rootRadius;
    public int RootArea => rootArea;
    public int BaseOutput => baseOutput;
    public Item FoodSourceItem => FoodSource;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public int Size => size;

    [SerializeField] private int size = 1; // default to 1 tile big
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private string speciesName = default;
    [SerializeField] private int rootRadius = default;
    [SerializeField] private int rootArea = default;
    [SerializeField] private int baseOutput = default;
    [SerializeField] private List<NeedConstructData> terrainNeeds = default;
    [SerializeField] private List<NeedConstructData> foodNeeds = default;
    [SerializeField] private List<NeedConstructData> waterNeeds = default;
    [SerializeField] private Item FoodSource = default;


    public Dictionary<string, Need> SetupNeeds()
    {
        Dictionary<string, Need> needs = new Dictionary<string, Need>();

        //Terrain Needs
        foreach (NeedConstructData need in terrainNeeds)
        {
            needs.Add(need.NeedName, new TerrainNeed(need));
        }

        //Food Needs
        foreach (NeedConstructData need in foodNeeds)
        {
            needs.Add(need.NeedName, new FoodNeed(need));
        }

        //Water Needs
        foreach (NeedConstructData need in waterNeeds)
        {
            needs.Add(need.NeedName, new LiquidNeed(need));
        }

        return needs;
    }

    public void SetupData(string name, int rootRadius, int output, List<List<NeedConstructData>> needs)
    {
        this.speciesName = name;
        this.rootRadius = rootRadius;
        this.baseOutput = output;

        for(int i = 0; i < needs.Count; ++i)
        {
            switch(i)
            {
                case 0:
                    terrainNeeds = needs[i];
                    break;
                case 1:
                    foodNeeds = needs[i];
                    break;
                case 2:
                    waterNeeds = needs[i];
                    break;
                default:
                    return;
            }
        }
    }
}
