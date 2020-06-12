using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FoodSource: MonoBehaviour
{
    public FoodSourceSpecies Species => species;
    public Dictionary<string, float> needValues = new Dictionary<string, float>();
    public float FoodOutput => species.BaseOutput;// CalculateOutput(); // ---------------------------Testing, change back later
    public Vector2 Position { get; private set; } = Vector2.zero;

    // Food source's environmental needs
    private Dictionary<string, float> needsValues = new Dictionary<string, float>();

    [SerializeField] private FoodSourceSpecies species = default;

    private float neutralMultiplier = 0.5f;
    private float goodMultiplier = 1.0f;

    private void Start()
    {
        if (species)
        {
            InitializeNeedValues();
        }
    }

    public void InitializeFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        this.species = species;
        this.Position = position;
        this.InitializeNeedValues();
    }

    private void InitializeNeedValues()
    {
        foreach (string needType in species.Needs.Keys) {
            needValues.Add(needType, 0);
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
        foreach (KeyValuePair<string, float> needValuePair in needValues)
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
}
