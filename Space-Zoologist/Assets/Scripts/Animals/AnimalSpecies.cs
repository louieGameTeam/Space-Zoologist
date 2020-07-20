using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    // Getters
    public string SpeciesName => speciesName;
    public int Dominance => dominance;
    public float GrowthFactor => growthFactor;
    public Dictionary<string, Need> Needs => needs;
    public float Size => size;
    public List<BehaviorScriptTranslation> Behaviors => needBehaviorSet;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public Sprite Icon => icon;
    public Sprite Sprite => icon;
    public Sprite Representation => representation;

    public AnimatorController AnimatorController => animatorController;

    // Values
    [SerializeField] private AnimatorController animatorController = default;
    [SerializeField] private string speciesName = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private int dominance = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;

    [Header("Behavior displayed when need isn't being met")]
    [SerializeField] private List<BehaviorScriptTranslation> needBehaviorSet = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private Sprite icon = default;

    private Dictionary<string, Need> needs = new Dictionary<string, Need>();
    [SerializeField] private List<NeedTypeConstructData> needsList = new List<NeedTypeConstructData>()
    {
        new NeedTypeConstructData(NeedType.Atmosphere),
        new NeedTypeConstructData(NeedType.Terrain),
        new NeedTypeConstructData(NeedType.Density),
        new NeedTypeConstructData(NeedType.Food),
        new NeedTypeConstructData(NeedType.Species),
    };

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

    private void OnEnable()
    {
        foreach (NeedTypeConstructData needData in needsList)
        {
            foreach (NeedConstructData need in needData.Needs)
            {
                // Use the NeedData to create Need
                Needs.Add(need.NeedName, new Need(needData.NeedType, need));
                //Debug.Log($"Add {need.NeedName} Need for {this.SpeciesName}");
            }
        }
    }
    public void SetupData(string name, int dominance, float growthFactor, List<string> accessibleTerrain, List<NeedTypeConstructData> needsList)
    {
        // TODO setup behaviors and accessible terrain
        this.speciesName = name;
        this.dominance = dominance;
        this.growthFactor = growthFactor;
        this.accessibleTerrain = new List<TileType>();
        foreach(string tileType in accessibleTerrain)
        {
            if (tileType.Equals("Sand", StringComparison.OrdinalIgnoreCase))
            {
                this.accessibleTerrain.Add(TileType.Sand);
            }
            if (tileType.Equals("Grass", StringComparison.OrdinalIgnoreCase))
            {
                this.accessibleTerrain.Add(TileType.Grass);
            }
            if (tileType.Equals("Dirt", StringComparison.OrdinalIgnoreCase))
            {
                this.accessibleTerrain.Add(TileType.Dirt);
            }
            if (tileType.Equals("Liquid", StringComparison.OrdinalIgnoreCase))
            {
                this.accessibleTerrain.Add(TileType.Liquid);
            }
            if (tileType.Equals("Rock", StringComparison.OrdinalIgnoreCase))
            {
                this.accessibleTerrain.Add(TileType.Rock);
            }
            if (tileType.Equals("Wall", StringComparison.OrdinalIgnoreCase))
            {
                this.accessibleTerrain.Add(TileType.Wall);
            }
        }
        //this.accessibleTerrain = accessibleTerrain;
        this.needsList = needsList;
    }
}
