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
    public int FoodDominance => foodDominance;
    public int TerrainTilesRequired => terrainTilesRequired;
    public float GrowthScaleFactor => growthScaleFactor;
    public int GrowthRate => growthRate;
    public int DecayRate => decayRate;
    public float Size => size;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public Sprite Icon => icon;
    public Sprite Sprite => icon;
    public float Range => range;
    public int MoveCost => moveCost;
    public Sprite Representation => representation;
    public RuntimeAnimatorController AnimatorController => animatorController;

    // Values
    [SerializeField] private RuntimeAnimatorController animatorController = default;
    [SerializeField] private string speciesName = default;
    [SerializeField] private SpeciesType species = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private int foodDominance = default;
    [SerializeField] private int terrainTilesRequired = default;
    [SerializeField] private float growthScaleFactor = default;
    [Range(1, 30)]
    [SerializeField] private int growthRate = 3;
    [Range(1, 30)]
    [SerializeField] private int decayRate = 3;
    [SerializeField] private float range = default;
    [SerializeField] private int moveCost = default;

    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private Sprite icon = default;

    [SerializeField]
    private List<NeedTypeConstructData> needsList = new List<NeedTypeConstructData>();

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

    public Dictionary<string, Need> SetupNeeds()
    {
        Dictionary<string, Need> needs = new Dictionary<string, Need>();
        foreach (NeedTypeConstructData needData in needsList)
        {
            foreach (NeedConstructData need in needData.Needs)
            {
                // Use the NeedData to create Need
                needs.Add(need.NeedName, new Need(needData.NeedType, need));
                //Debug.Log($"Add {need.NeedName} Need for {this.SpeciesName}");
            }
        }
        return needs;
    }

    public List<PopulationBehavior> GetBehaviors()
    {
        List<PopulationBehavior> behaviors = new List<PopulationBehavior>();
        foreach (NeedTypeConstructData needData in needsList)
        {
            foreach (NeedConstructData need in needData.Needs)
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
}