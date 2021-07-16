using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A runtime instance of a food source
/// </summary>
public class FoodSource : MonoBehaviour, Life
{
    public FoodSourceSpecies Species => species;
    public float FoodOutput => CalculateOutput();
    public Vector2 Position { get; private set; } = Vector2.zero;
    public bool terrainNeedMet = false;
    public bool liquidNeedMet = false;

    public float TerrainRating => terrainRating;
    public float WaterRating => waterRating;

    public Dictionary<string, Need> Needs => needs;
    private Dictionary<string, Need> needs = new Dictionary<string, Need>();

    // For runtime instances of a food source
    [Expandable][SerializeField] private FoodSourceSpecies species = default;
    [SerializeField] private TileSystem tileSystem = default;


    public bool isUnderConstruction = false;
    private float terrainRating = 0f;
    private float waterRating = 0f;

    private int[] accessibleTerrian = new int[(int)TileType.TypesOfTiles];
    private bool hasAccessibilityChanged = default;
    private bool hasAccessibilityChecked = default;

    private TileSystem TileSystem = default;

    // To figure out if the output has changed, in order to invoke vent
    private float prevOutput = 0;

    private void Awake()
    {
        if (species)
        {
            InitializeFoodSource(species, transform.position, this.tileSystem);
        }
    }

    // Subscribe to events here
    private void Start()
    {
        // If the food has atmospheric need then subscribe to atmosphere changed event
        foreach (AtmosphereComponent atmosphereComponent in Enum.GetValues(typeof(AtmosphereComponent)))
        {
            if (this.needs.ContainsKey(atmosphereComponent.ToString()))
            {
                EventManager.Instance.SubscribeToEvent(EventType.AtmosphereChange, () =>
                {
                    this.hasAccessibilityChecked = true;
                    this.hasAccessibilityChanged = true;
                });
            }
        }
    }

    public void InitializeFoodSource(FoodSourceSpecies species, Vector2 position, TileSystem tileSystem)
    {
        this.species = species;
        this.Position = position;
        this.GetComponent<SpriteRenderer>().sprite = species.FoodSourceItem.Icon;
        this.InitializeNeedValues();
        this.TileSystem = tileSystem;
        this.accessibleTerrian = this.TileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
    }

    private void InitializeNeedValues()
    {
        this.needs = this.species.SetupNeeds();
    }

    private float CalculateOutput()
    {
        terrainNeedMet = false;
        liquidNeedMet = false;
        
        float numPreferredTiles = 0f;
        float survivableTiles = 0f;
        float totalNeededTiles = 0f;
        foreach (KeyValuePair<string, Need> needValuePair in this.needs)
        {
            string needType = needValuePair.Key;
            Need needValue = needValuePair.Value;
     
            if (needType.Equals("Liquid") && needValue.NeedType.Equals(NeedType.Liquid))
            {
                if (needIsSatisified(needType, needValue.NeedValue))
                {
                    waterRating = 1 + (needValue.NeedValue - needValue.GetMaxThreshold());
                    liquidNeedMet = true;
                }
            }
            if (needValue.NeedType.Equals(NeedType.Terrain))
            {
                totalNeededTiles = needValue.GetMaxThreshold();
                if (needValue.IsPreferred)
                {
                    numPreferredTiles = needValue.NeedValue;
                }
                else
                {
                    survivableTiles = needValue.NeedValue;
                }
            }
        }
        //Debug.Log(gameObject.name + " surv tiles: " + survivableTiles + " pref tiles: " + numPreferredTiles);
        if (survivableTiles + numPreferredTiles >= totalNeededTiles)
        {
            terrainRating = species.BaseOutput + numPreferredTiles;
            terrainNeedMet = true;
        }
        else
        {
            terrainRating = species.BaseOutput - (totalNeededTiles - survivableTiles - numPreferredTiles);
            if (terrainRating < 0) terrainRating = 0;
        }
        //Debug.Log(gameObject.name + " waterRating: " + waterRating + " terrainRating: " + terrainRating);
        float output = waterRating + terrainRating;
        return output;
    }

    private bool needIsSatisified(string needType, float needValue)
    {
        NeedCondition condition = this.needs[needType].GetCondition(needValue);
        // A need is not satisfied
        if (condition != NeedCondition.Good)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>Z
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(string need, float value)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.SpeciesName } food source has no need { need }");
        this.needs[need].UpdateNeedValue(value);
        // Debug.Log($"The { species.SpeciesName } population { need } need has new value: {NeedsValues[need]}");
    }

    public Dictionary<string, Need> GetNeedValues()
    {
        return this.Needs;
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    /// <summary>
    /// Checks accessible terrain info, ie terrain tile composition
    /// </summary>
    /// <remarks>
    /// Actual checking will only be done once per universal NS update loop,
    /// since terrain will not change during that time
    /// </remarks>
    /// <returns>True is accessible terrain had changed, false otherwise</returns>
    public bool GetAccessibilityStatus()
    {
        // No need to check if terrain was not modified
        if (!this.TileSystem.HasTerrainChanged)
        {
            return false;
        }

        // Return result if have checked
        if (this.hasAccessibilityChecked)
        {
            return this.hasAccessibilityChanged;
        }

        var preTerrain = this.accessibleTerrian;
        var curTerrain = this.TileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);

        // Accessible terrain had changed
        this.hasAccessibilityChecked = true;
        if(!preTerrain.SequenceEqual(curTerrain))
        {
            this.hasAccessibilityChanged = true;
        }

        return this.hasAccessibilityChanged;
    }

    /// <summary>
    /// Updates the accessible terrain info
    /// </summary>
    public void UpdateAccessibleTerrainInfo()
    {
        if (this.hasAccessibilityChanged)
        {
            this.accessibleTerrian = this.TileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
        }

        // Reset flags
        this.hasAccessibilityChecked = false;
        this.hasAccessibilityChanged = false;
    }
}
