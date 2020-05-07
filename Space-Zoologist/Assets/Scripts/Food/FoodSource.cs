using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FoodSource: MonoBehaviour
{
    public Dictionary<string, float> needValues = new Dictionary<string, float>();
    public float FoodOutput { get; }

    [SerializeField] private FoodSourceSpecies species = default;
    private Dictionary<string, Need> speciesNeeds = null;

    private float neutralMultiplier = 0.5f;
    private float goodMultiplier = 1.0f;

    private void Start()
    {
        speciesNeeds = species.Needs;
        InitializeNeedValues();
    }

    private void InitializeNeedValues()
    {
        Debug.Log("here 2");
        foreach (string needType in species.Needs.Keys) {
            needValues.Add(needType, 0);
        }
    }

    private float CalculateOutput()
    {
        int severityTotal = 0;
        float output = 0;
        foreach (Need need in speciesNeeds.Values)
        {
            severityTotal += need.Severity;
        }
        foreach (KeyValuePair<string, float> needValuePair in needValues)
        {
            string needType = needValuePair.Key;
            float needValue = needValuePair.Value;
            NeedCondition condition = speciesNeeds[needType].GetCondition(needValue);
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
            float needSeverity = speciesNeeds[needType].Severity;
            output += multiplier + (needSeverity / severityTotal) * species.BaseOutput;
        }
        return output;
    }
}
