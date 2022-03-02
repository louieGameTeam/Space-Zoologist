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
    public bool waterNeedMet = false;

    public float TerrainRating => terrainRating;
    public float WaterRating => waterRating;

    public Dictionary<ItemID, Need> Needs => needs;
    private Dictionary<ItemID, Need> needs = new Dictionary<ItemID, Need>();

    /// <summary>
    /// Special terrain water need
    /// </summary>
    /// <remarks>
    /// This is used by food sources that need
    /// water as a terrain need and not a liquid need
    /// </remarks>
    /// <example>
    /// Kelp needs to be placed in water to grow, 
    /// but it still has liquid needs 
    /// for specific water compositions
    /// </example>
    public Need TerrainWaterNeed => terrainWaterNeed;
    /// <summary>
    /// Special terrain water need
    /// </summary>
    /// <remarks>
    /// This is used by food sources that need
    /// water as a terrain need and not a liquid need
    /// </remarks>
    /// <example>
    /// Kelp needs to be placed in water to grow, 
    /// but it still has liquid needs 
    /// for specific water compositions
    /// </example>
    private Need terrainWaterNeed = null;

    // For runtime instances of a food source
    [Expandable][SerializeField] private FoodSourceSpecies species = default;


    public bool isUnderConstruction = false;
    private float terrainRating = 0f;
    private float waterRating = 0f;

    private int[] accessibleTerrian = new int[(int)TileType.TypesOfTiles];
    private bool hasAccessibilityChanged = default;
    private bool hasAccessibilityChecked = default;

    // To figure out if the output has changed, in order to invoke vent
    private float prevOutput = 0;

    private void Awake()
    {
        if (species)
        {
            InitializeFoodSource(species, transform.position);
        }
    }

    // Subscribe to events here
    private void Start()
    {
        
    }

    public void InitializeFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        this.species = species;
        this.Position = position;
        this.GetComponent<SpriteRenderer>().sprite = species.FoodSourceItem.Icon;
        this.InitializeNeedValues();
        this.accessibleTerrian = GameManager.Instance.m_tileDataController.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
    }

    private void InitializeNeedValues()
    {
        this.needs = this.species.SetupNeeds();
        terrainWaterNeed = species.GetTerrainWaterNeed();
    }

    private float CalculateOutput()
    {
        CalculateTerrainNeed();
        CalculateWaterNeed();

        float output;

        if(waterNeedMet && terrainNeedMet)
        {
            output = species.BaseOutput * (1 + (waterRating + terrainRating)/2);
        }
        else
        {
            output = species.BaseOutput * (1 + Mathf.Min(waterRating, 0)) * (1 + Mathf.Min(terrainRating, 0));
        }

        return Mathf.Ceil(output);
    }

    public void CalculateTerrainNeed()
    {
        float totalNeededTiles = species.Size.x * species.Size.y;
        float availablePreferredTiles = 0f;
        float availableSurvivableTiles = 0f;
        float totalTilesAvailable;

        foreach (KeyValuePair<ItemID, Need> need in this.needs)
        {
            if (need.Value.NeedType.Equals(NeedType.Terrain))
            {
                if (need.Value.IsPreferred)
                {
                    availablePreferredTiles += need.Value.NeedValue;
                }
                else
                {
                    availableSurvivableTiles += need.Value.NeedValue;
                }
            }
        }

        // Factor in the special terrain water need
        if (terrainWaterNeed != null)
        {
            if (terrainWaterNeed.IsPreferred)
            {
                availablePreferredTiles += terrainWaterNeed.NeedValue;
            }
            else availableSurvivableTiles += terrainWaterNeed.NeedValue;
        }

        totalTilesAvailable = availablePreferredTiles + availableSurvivableTiles;
        if (totalTilesAvailable >= totalNeededTiles)
        {
            terrainNeedMet = true;
            terrainRating = availablePreferredTiles / totalNeededTiles;
        }
        else
        {
            terrainNeedMet = false;
            terrainRating = (totalTilesAvailable - totalNeededTiles) / totalNeededTiles;
        }

        //Debug.Log(gameObject.name + " terrain Rating: " + terrainRating + ", preferred tiles: " + availablePreferredTiles + ", survivable tiles: " + availableSurvivableTiles);
    }

    /// <summary>
    /// Compute the number of tiles that the food source can drink from
    /// within range of the plant's root radius
    /// </summary>
    /// <returns></returns>
    public int DrinkableWaterInRange()
    {
        TileDataController tileDataController = GameManager.Instance.m_tileDataController;
        List<float[]> accessibleLiquidBodies = tileDataController
            .GetLiquidCompositionWithinRange(
                GetCellPosition(),
                Species.Size, 
                Species.RootRadius);

        // Compute how many of the compositions are drinkable
        return accessibleLiquidBodies
            .Where(body => LiquidNeedSystem.LiquidIsDrinkable(species.LiquidNeeds, body))
            .Count();
    }

    public void CalculateWaterNeed()
    {
        float waterTilesRequired = species.WaterTilesRequired;
        float waterTilesUsed = Mathf.Min(DrinkableWaterInRange(), waterTilesRequired);

        if (waterTilesUsed >= waterTilesRequired)
        {
            //Debug.Log("Water need met");
            waterNeedMet = true;
            waterRating = 0;

            // Update water rating for each liquid need
            foreach (LiquidNeedConstructData data in species.LiquidNeeds)
            {
                LiquidNeed waterNeed = needs[data.ID] as LiquidNeed;
                float percent = waterNeed.NeedValue;
                float minNeeded = data.MinThreshold;
                float maxWaterTilePercent = GrowthCalculator.GetMaxWaterTilePercent(data.ID.WaterIndex);
                waterRating += (percent - minNeeded) / (maxWaterTilePercent - minNeeded);
            }
        }
        else
        {
            waterNeedMet = false;
            waterRating = (waterTilesUsed - waterTilesRequired) / waterTilesRequired;
        }

        //if (gameObject)
            //Debug.Log(gameObject.name + " water Rating: " + waterRating + ", water source size: " + waterTilesUsed);
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>Z
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(ItemID need, float value)
    {
        Debug.Assert(this.needs.ContainsKey(need), $"{ species.ID } food source has no need { need }");
        this.needs[need].UpdateNeedValue(value);
        // Debug.Log($"The { species.SpeciesName } population { need } need has new value: {NeedsValues[need]}");
    }

    public Dictionary<ItemID, Need> GetNeedValues()
    {
        return this.Needs;
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    public Vector3Int GetCellPosition()
    {
        return GameManager.Instance.m_tileDataController.WorldToCell(GetPosition());
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
        if (!GameManager.Instance.m_tileDataController.HasTerrainChanged)
        {
            return false;
        }

        // Return result if have checked
        if (this.hasAccessibilityChecked)
        {
            return this.hasAccessibilityChanged;
        }

        var preTerrain = this.accessibleTerrian;
        var curTerrain = GameManager.Instance.m_tileDataController.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);

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
            this.accessibleTerrian = GameManager.Instance.m_tileDataController.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
        }

        // Reset flags
        this.hasAccessibilityChecked = false;
        this.hasAccessibilityChanged = false;
    }
}
