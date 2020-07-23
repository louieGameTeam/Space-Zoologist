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

    public Dictionary<string, float> NeedsValues => needsValues;
    protected Dictionary<string, float> needsValues = new Dictionary<string, float>();

    [SerializeField] private FoodSourceSpecies species = default;

    private float neutralMultiplier = 0.5f;
    private float goodMultiplier = 1.0f;

    private int[] accessibleTerrian = new int[(int)TileType.TypesOfTiles];

    private void Awake()
    {
        if (species)
        {
            InitializeFoodSource(species,transform.position);
        }
    }

    public void InitializeFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        this.species = species;
        this.Position = position;
        this.GetComponent<SpriteRenderer>().sprite = species.FoodSourceItem.Icon;
        this.InitializeNeedValues();

        this.accessibleTerrian = TileSystem.ins.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);
    }

    private void InitializeNeedValues()
    {
        foreach (string needType in species.Needs.Keys) {
            needsValues.Add(needType, 0);
        }
    }

    private float CalculateOutput()
    {
        int severityTotal = 0;
        float output = 0;
        foreach (Need need in species.Needs.Values)
        {
            severityTotal += need.Severity;
        }
        foreach (KeyValuePair<string, float> needValuePair in needsValues)
        {
            string needType = needValuePair.Key;
            float needValue = needValuePair.Value;
            NeedCondition condition = species.Needs[needType].GetCondition(needValue);
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
            float needSeverity = species.Needs[needType].Severity;
            output += multiplier * (needSeverity / severityTotal) * species.BaseOutput;
        }
        return output;
    }

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    public void UpdateNeed(string need, float value)
    {
        Debug.Assert(needsValues.ContainsKey(need), $"{ species.SpeciesName } population has no need { need }");
        needsValues[need] = value;
        // Debug.Log($"The { species.SpeciesName } population { need } need has new value: {NeedsValues[need]}");
    }

    public Dictionary<string, float> GetNeedValues()
    {
        return this.NeedsValues;
    }

    public Vector3 GetPosition()
    {
        return this.gameObject.transform.position;
    }

    public bool GetAccessibilityStatus()
    {
        var preTerrain = this.accessibleTerrian;
        var curTerrain = TileSystem.ins.CountOfTilesInRange(Vector3Int.FloorToInt(this.Position), this.Species.RootRadius);

        if(!preTerrain.SequenceEqual(curTerrain))
        {
            this.accessibleTerrian = curTerrain;
        }

        return false;
    }
}
