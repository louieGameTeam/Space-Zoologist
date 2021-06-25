using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInspectorText : MonoBehaviour
{
    [SerializeField] private Text inspectorWindowTitle = default;
    [SerializeField] private Image inspectorWindowImage = default;
    [SerializeField] private RectTransform layoutGroupRect = default;
    [SerializeField] private Text inspectorWindowText = default;
    [SerializeField] private GameObject NeedSliderPrefab = null;
    public InspectorText CurrentDisplay => currentDisplay;
    private InspectorText currentDisplay = InspectorText.Population;
    public enum InspectorText { Population, Food, Area, Liquid }

    private List<GameObject> needSliders = new List<GameObject>();

    public void DisplayPopulationStatus(Population population)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Population;
        inspectorWindowImage.sprite = population.species.Icon;
        inspectorWindowTitle.text = population.species.SpeciesName;

        string displayText = $"{population.species.SpeciesName} Info: \n";

        displayText += $"Count: {population.Count}, {population.GrowthStatus}\n";
        if (population.GrowthStatus.Equals(GrowthStatus.stagnate))
        {
            displayText += $"Please wait 1 day for population to get accustomed to enclosure\n";
        }
        else if (population.GrowthStatus.Equals(GrowthStatus.growing))
        {
            displayText += $"{population.gameObject.name} population will increase in {population.DaysTillGrowth()} days\n";
        }
        else
        {
            if (population.IsStagnate())
            {
                displayText += $"{population.gameObject.name} is stagnate\n";
            }
            else
            {
                displayText += $"{population.gameObject.name} population will decrease in {population.DaysTillDeath()} days\n";
            }
            List<NeedType> unmetNeeds = population.GetUnmentNeeds();
            foreach (NeedType needType in unmetNeeds)
            {
                displayText += $"\n{needType.ToString()} need not being met"; 
            }
        }

        GenerateSliders(population.Needs, ref displayText);

        this.inspectorWindowText.text = displayText;
    }

    public void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Food;
        inspectorWindowImage.sprite = foodSource.Species?.FoodSourceItem.Icon;
        inspectorWindowTitle.text = foodSource.Species.SpeciesName;

        string displayText = $"{foodSource.name} Info: \n";

        displayText += $"Output: {foodSource.FoodOutput}\n";
        if (!foodSource.terrainNeedMet)
        {
            displayText += $"\n Terrain need not being met";
        }
        if (!foodSource.liquidNeedMet)
        {
            displayText += $"\n Liquid need not being met";
        }

        GenerateSliders(foodSource.Needs, ref displayText);

        this.inspectorWindowText.text = displayText;
    }

    public void DislplayEnclosedArea(EnclosedArea enclosedArea)
    {
        ClearInspectorWindow();
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
        ClearInspectorWindow();
        currentDisplay = InspectorText.Liquid;
        string displayText = "";
        if (compositions == null)
        {
            displayText = "Water : 0.00\n Salt : 0.00 \n Bacteria : 0.00\n";
        }
        else
        {
            string[] liquidName = new string[] { "Water", "Salt", "Bacteria" };
            for (int i = 0; i < 3; i++)
            {
                displayText += $"{liquidName[i]} : {compositions[i] * 100}%\n";
            }

        }
        this.inspectorWindowText.text = displayText;
    }

    public void ClearInspectorWindow() {
        inspectorWindowImage.sprite = null;
        inspectorWindowTitle.text = "Title";
        foreach (GameObject obj in needSliders) {
            Destroy(obj);
        }
        needSliders.Clear();
    }

    private void GenerateSliders(Dictionary<string, Need> needs, ref string displayText) {

        foreach (var pair in needs)
        {
            displayText += $"\n{pair.Key}: {pair.Value.NeedValue}";

            GameObject sliderObj = Instantiate(NeedSliderPrefab, layoutGroupRect);
            needSliders.Add(sliderObj);

            NeedSlider slider = sliderObj.GetComponent<NeedSlider>();
            slider.SetName(pair.Key);

            NeedCondition condition = pair.Value.GetCondition(pair.Value.NeedValue);

            //FIXME TODO Find a better function that converts the raw need value to need satisfaction (from -1 to 1).
            if (condition == NeedCondition.Bad)
                slider.SetValue(-1f + pair.Value.NeedValue / pair.Value.GetThresholdForFirstGoodCondition());
            else if (condition == NeedCondition.Neutral)
                slider.SetValue(pair.Value.NeedValue / pair.Value.GetThresholdForFirstGoodCondition());
            else
                slider.SetValue(1f);
        }
    }
}
