using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A runtime instance of a food source
/// </summary>
public class FoodSource: MonoBehaviour, Life
{
    public FoodSourceSpecies Species => species;
    public float FoodOutput => CalculateOutput();
    public Vector2 Position { get; private set; } = Vector2.zero;

    public Dictionary<string, Need> Needs => needs;
    private Dictionary<string, Need> needs = new Dictionary<string, Need>();

    [SerializeField] private FoodSourceSpecies species = default;

    private float neutralMultiplier = 0.5f;
    private float goodMultiplier = 1.0f;

    private int[] accessibleTerrian = new int[(int)TileType.TypesOfTiles];
    private bool hasAccessibleTerrainChanged = default;
    private bool hasAccessibleTerrainChecked = default;

    private TileSystem TileSystem = default;

    private void Awake()
    {
        if (species)
        {
            InitializeFoodSource(species, transform.position);
        }
    }

    public void InitializeFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        this.species = species;
        this.Position = position;
        this.GetComponent<SpriteRenderer>().sprite = species.FoodSourceItem.Icon;
        this.InitializeNeedValues();
        this.TileSystem = FindObjectOfType<TileSystem>();
        this.accessibleTerrian = this.TileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
    }

    private void InitializeNeedValues()
    {
        this.needs = this.species.SetupNeeds();
    }

    private float CalculateOutput()
    {
        int severityTotal = 0;
        float output = 0;
        foreach (Need need in this.needs.Values)
        {
            severityTotal += need.Severity;
        }
        foreach (KeyValuePair<string, Need> needValuePair in this.needs)
        {
            string needType = needValuePair.Key;
            float needValue = needValuePair.Value.NeedValue;
            NeedCondition condition = this.needs[needType].GetCondition(needValue);
            float multiplier = 0;
            switch (condition)
            {
                case NeedCondition.Bad:
                    multiplier = 0;
                    break;
                case NeedCondition.Neutral:
                    multiplier = neutralMultiplier;
                    break;
                case NeedCondition.Good:
                    multiplier = goodMultiplier;
                    break;
            }
            float needSeverity = this.needs[needType].Severity;
            output += multiplier * (needSeverity / severityTotal) * species.BaseOutput;
        }
        return output;
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
    /// Actual checking will only be done once per universial NS update loop,
    /// since terrain will not change during that time
    /// </remarks>
    /// <returns>True is accessible terrain had changed, false otherwise</returns>
    public bool GetAccessibilityStatus()
    {
        // Return result if have checked
        if (this.hasAccessibleTerrainChecked)
        {
            return this.hasAccessibleTerrainChanged;
        }

        var preTerrain = this.accessibleTerrian;
        var curTerrain = this.TileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);

        // Accessible terrain had changed
        if(!preTerrain.SequenceEqual(curTerrain))
        {
            this.hasAccessibleTerrainChanged = true;
            this.hasAccessibleTerrainChecked = true;
        }
        else
        {
            this.hasAccessibleTerrainChecked = true;
        }

        return this.hasAccessibleTerrainChanged;
    }

    /// <summary>
    /// Updates the accessible terrain info
    /// </summary>
    public void UpdateAccessibleTerrainInfo()
    {
        if (this.hasAccessibleTerrainChanged)
        {
            this.accessibleTerrian = this.TileSystem.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
        }

        // Reset flags
        this.hasAccessibleTerrainChecked = false;
        this.hasAccessibleTerrainChanged = false;
    }
}
