using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInspectorText : MonoBehaviour
{
    [SerializeField] private Text inspectorWindowText = default;
    public InspectorText CurrentDisplay => currentDisplay;
    private InspectorText currentDisplay = InspectorText.Population;
    public enum InspectorText { Population, Food, Area, Liquid }

    public void DisplayPopulationStatus(Population population)
    {
        currentDisplay = InspectorText.Population;
        string displayText = $"{population.species.SpeciesName} Info: \n";

        displayText += $"Count: {population.Count} [{population.GrowthStatus}]\n";


        if (population.GrowthStatus.Equals(GrowthStatus.growing))
        {
            displayText += $"{population.gameObject.name} population will increase in {population.DaysTillGrowth()} days\n";
        }
        else
        {
            HashSet<Need> UniqueNeed = new HashSet<Need>();
            foreach (Need need in population.Needs.Values)
            {
                if (UniqueNeed.Contains(need)) {
                    continue;
                }
                UniqueNeed.Add(need);
                
                if (need.GetCondition(need.NeedValue).Equals(NeedCondition.Bad))
                {
                    if (need.NeedName.Length == 1)
                    {
                        displayText += $"Death countdown for {need.NeedName[0]} need: {population.DaysTillDeath(need.NeedName[0])} days\n";
                    }
                    else
                    {
                        displayText += $"Death countdown for {need.NeedType} (";
                        foreach (string str in need.NeedName) displayText += str + ", ";
                        displayText = displayText.TrimEnd(new char[] { ',', ' ' });
                        displayText += $") need: {population.DaysTillDeath(need.NeedName[0])} days\n";
                    }
    
                }
            }
        }

        this.inspectorWindowText.text = displayText;
    }

    public void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        currentDisplay = InspectorText.Food;
        string displayText = $"{foodSource.name} Info: \n";

        displayText += $"Output: {foodSource.FoodOutput}/{foodSource.Species.BaseOutput}\n";

        HashSet<Need> UniqueNeed = new HashSet<Need>();
        foreach (Need need in foodSource.Needs.Values)
        {
            if (UniqueNeed.Contains(need))
            {
                continue;
            }
            UniqueNeed.Add(need);

            if (need.NeedName.Length == 1)
            {
                displayText += $"{need.NeedName[0]} : ";
            }
            else {
                foreach (string str in need.NeedName) displayText += str + ", ";
                displayText = displayText.TrimEnd(new char[] { ',', ' ' });
                displayText += " : ";
            }

            //TODO find a more elegant way. Temporary to make harmful need more intuitive
            if (need.Severity < 0) {
                displayText += $"{need.NeedValue} [{(NeedCondition)(NeedCondition.Good - need.GetCondition(need.NeedValue))}]\n";
                continue;
            }
            displayText += $"{need.NeedValue} [{need.GetCondition(need.NeedValue)}]\n";
        }


        this.inspectorWindowText.text = displayText;
    }

    public void DislplayEnclosedArea(EnclosedArea enclosedArea)
    {
        currentDisplay = InspectorText.Area;
        // THe composition is a list of float value in the order of the AtmoshpereComponent Enum
        float[] atmosphericComposition = enclosedArea.atmosphericComposition.GetComposition();
        float[] terrainComposition = enclosedArea.terrainComposition;

        string displayText = $"Enclosed Area {enclosedArea.id} Info: \n";

        // Atmospheric info
        displayText += "Atmospheric composition: \n";
        foreach (var (value, index) in atmosphericComposition.WithIndex())
        {
            displayText += $"{((AtmosphereComponent)index).ToString()} : {value}\n";
        }

        displayText += "\nTerrain: \n";
        foreach (var (value, index) in terrainComposition.WithIndex())
        {
            displayText += $"{((TileType)index).ToString()} : {value}\n";
        }

        displayText += "\n";
        displayText += $"Population count: {enclosedArea.populations.Count}\n";
        displayText += $"Total animal count: {enclosedArea.animals.Count}\n";
        displayText += $"Food Source count: {enclosedArea.foodSources.Count}\n";

        this.inspectorWindowText.text = displayText;
    }

    public void DisplayLiquidCompisition(float[] compositions)
    {
        currentDisplay = InspectorText.Liquid;
        string displayText = "Liquid composition: 0 0 0\n";

        // TODO causing errors, debug
        //foreach (var (composition, index) in compositions.WithIndex())
        //{
        //    displayText += $"{((LiquidComposition)index).ToString()} : {composition}\n";
        //}

        this.inspectorWindowText.text = displayText;
    }
}
