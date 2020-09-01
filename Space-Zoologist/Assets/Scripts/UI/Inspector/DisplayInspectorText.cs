using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInspectorText : MonoBehaviour
{
    [SerializeField] private Text inspectorWindowText;

    public void DisplayAnimalStatus(Animal animal)
    {
        Population population = animal.PopulationInfo;

        string displayText = $"{population.species.SpeciesName} Info: \n";

        displayText += $"Count: {population.Count} [{population.GrowthStatus}]\n";

        foreach (Need need in population.Needs.Values)
        {
            displayText += $"{need.NeedName} : {need.NeedValue} [{need.GetCondition(need.NeedValue)}]\n";
        }

        this.inspectorWindowText.text = displayText;
    }

    public void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        string displayText = $"{foodSource.name} Info: \n";

        displayText += $"Output: {foodSource.FoodOutput}/{foodSource.Species.BaseOutput}\n";

        foreach (Need need in foodSource.Needs.Values)
        {
            displayText += $"{need.NeedName} : {need.NeedValue} [{need.GetCondition(need.NeedValue)}]\n";
        }


        this.inspectorWindowText.text = displayText;
    }

    public void DislplayEnclosedArea(EnclosedArea enclosedArea)
    {
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
        displayText += $"Total aniaml count: {enclosedArea.animals.Count}\n";
        displayText += $"Food Source count: {enclosedArea.foodSources.Count}\n";

        this.inspectorWindowText.text = displayText;
    }

    public void DisplayLiquidCompisition(float[] compositions)
    {
        string displayText = "Liquid composition: \n";

        foreach (var (composition, index) in compositions.WithIndex())
        {
            displayText += $"{((LiquidComposition)index).ToString()} : {composition}\n";
        }

        this.inspectorWindowText.text = displayText;
    }
}
