using System.Collections.Generic;
using UnityEngine;
using System;

public enum SpeciesType { Goat, Cow, Anteater, Spider, Slug, Momo }

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    // Getters
    public string SpeciesName => speciesName;
    public SpeciesType Species => species;
    public int TerrainTilesRequired => terrainTilesRequired;
    public float GrowthScaleFactor => growthScaleFactor;
    public int GrowthRate => growthRate;
    public int DecayRate => decayRate;
    public float Size => size;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public Sprite Icon => icon;
    public Sprite Sprite => icon;
    public float Range => range;
    public Sprite Representation => representation;
    // TODO setup tile weights for species
    public Dictionary<TileType, byte> TilePreference = default;
    public RuntimeAnimatorController AnimatorController => animatorController;

    // Values
    [SerializeField] private RuntimeAnimatorController animatorController = default;
    [SerializeField] private string speciesName = default;
    [SerializeField] private SpeciesType species = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int terrainTilesRequired = default;
    [SerializeField] private float growthScaleFactor = default;
    [Range(1, 30)]
    [SerializeField] private int growthRate = 3;
    [Range(1, 30)]
    [SerializeField] private int decayRate = 3;
    [SerializeField] private float range = default;

    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private Sprite icon = default;

    [SerializeField] private List<NeedConstructData> terrainNeeds = default;
    [SerializeField] private List<NeedConstructData> foodNeeds = default;
    [SerializeField] private List<NeedConstructData> waterNeeds = default;

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

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

    public List<PopulationBehavior> GetBehaviors()
    {
        List<PopulationBehavior> behaviors = new List<PopulationBehavior>();
        foreach (List<NeedConstructData> needsList in new List<NeedConstructData>[]{terrainNeeds, foodNeeds, waterNeeds})
        {
            foreach (NeedConstructData need in needsList)
            {
                foreach (NeedBehavior needBehavior in need.Conditions)
                {
                    if (needBehavior.Behavior != null)
                    {
                        behaviors.Add(needBehavior.Behavior);
                    }
                }
            }
        }
        return behaviors;
    }

    public Dictionary<Need, Dictionary<NeedCondition, PopulationBehavior>> SetupBehaviors(Dictionary<string, Need> needs)
    {
        Dictionary<Need, Dictionary<NeedCondition, PopulationBehavior>> needBehaviorDict = new Dictionary<Need, Dictionary<NeedCondition, PopulationBehavior>>();
        foreach (List<NeedConstructData> needsList in new List<NeedConstructData>[]{terrainNeeds, foodNeeds, waterNeeds})
        {
            foreach (NeedConstructData need in needsList)
            {
                Dictionary<NeedCondition, PopulationBehavior> needBehaviors = new Dictionary<NeedCondition, PopulationBehavior>();
                foreach (NeedBehavior needBehavior in need.Conditions)
                {
                    if (!needBehaviors.ContainsKey(needBehavior.Condition))
                    {
                        needBehaviors.Add(needBehavior.Condition, needBehavior.Behavior);
                    }
                }
                needBehaviorDict.Add(needs[need.NeedName], needBehaviors);
            }
        }
        return needBehaviorDict;
    }

    public void SetupData(string name, int growthRate, List<string> accessibleTerrain, List<List<NeedConstructData>> needsLists)
    {
        // TODO setup behaviors and accessible terrain
        this.speciesName = name;
        this.growthRate = growthRate;
        this.accessibleTerrain = new List<TileType>();
        foreach (string tileType in accessibleTerrain)
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
                this.accessibleTerrain.Add(TileType.Stone);
            }
            if (tileType.Equals("Wall", StringComparison.OrdinalIgnoreCase))
            {
                this.accessibleTerrain.Add(TileType.Wall);
            }
        }
        //this.accessibleTerrain = accessibleTerrain;

        for(int i = 0; i < needsLists.Count; ++i)
        {
            switch(i)
            {
                case 0:
                    terrainNeeds = needsLists[i];
                    break;
                case 1:
                    foodNeeds = needsLists[i];
                    break;
                case 2:
                    waterNeeds = needsLists[i];
                    break;
                default:
                    return;
            }
        }
    }
}