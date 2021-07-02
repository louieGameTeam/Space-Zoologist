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
    [SerializeField] private GameObject DetailButton = default;
    [SerializeField] private GameObject NeedSliderPrefab = null;
    public InspectorText CurrentDisplay => currentDisplay;
    private InspectorText currentDisplay = InspectorText.Population;
    public enum InspectorText { Population, Food, Area, Liquid }

    private List<GameObject> needSliders = new List<GameObject>();

    [Header("Temporary sprites")]
    [SerializeField] Sprite enclosedAreaSprite = default;
    [SerializeField] Sprite liquidSprite = default;
    [SerializeField] Sprite defaultSprite = default;


    GameObject detailBackground;
    Text detailText;
    float defaultHeight;
    public void Initialize()
    {
        defaultHeight = inspectorWindowImage.rectTransform.sizeDelta.y;

        detailBackground = DetailButton.transform.GetChild(0).gameObject;
        detailText = detailBackground.GetComponentInChildren<Text>(true);
    }

    public void DisplayPopulationStatus(Population population)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Population;
        inspectorWindowImage.sprite = population.species.Icon;

        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0,inspectorWindowImage.sprite.rect.size.x,defaultHeight/inspectorWindowImage.sprite.rect.size.y), defaultHeight);
        inspectorWindowTitle.text = population.species.SpeciesName;

        DetailButton.SetActive(true);
        detailBackground.SetActive(false);

        string displayText = $"{population.species.SpeciesName} Info: \n";

        displayText += $"Count: {population.Count}, {population.GrowthStatus}\n";
        if (population.GrowthStatus.Equals(GrowthStatus.stagnate))
        {
            //displayText += $"Please wait 1 day for population to get accustomed to enclosure\n";
            detailText.text = $"Please wait 1 day for population to get accustomed to enclosure";
        }
        else if (population.GrowthStatus.Equals(GrowthStatus.growing))
        {
            //displayText += $"{population.gameObject.name} population will increase in {population.DaysTillGrowth()} days\n";
            detailText.text = $"{population.gameObject.name} population will increase in {population.DaysTillGrowth()} days";
        }
        else
        {
            if (population.IsStagnate())
            {
                //displayText += $"{population.gameObject.name} is stagnate\n";
                detailText.text = $"{population.gameObject.name} is stagnate";
            }
            else
            {
                //displayText += $"{population.gameObject.name} population will decrease in {population.DaysTillDeath()} days\n";
                detailText.text = $"{population.gameObject.name} population will decrease in {population.DaysTillDeath()} days";
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
        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0, inspectorWindowImage.sprite.rect.size.x, defaultHeight / inspectorWindowImage.sprite.rect.size.y), defaultHeight);
        inspectorWindowTitle.text = foodSource.Species.SpeciesName;

        string displayText = $"{foodSource.name} Info: \n";

        displayText += $"Output: {foodSource.FoodOutput}\n";

        GenerateSliders(foodSource.Needs, ref displayText);

        this.inspectorWindowText.text = displayText;
    }

    public void DislplayEnclosedArea(EnclosedArea enclosedArea)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Area;

        inspectorWindowTitle.text = $"Area {enclosedArea.id}";
        inspectorWindowImage.sprite = enclosedAreaSprite;
        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0, inspectorWindowImage.sprite.rect.size.x, defaultHeight / inspectorWindowImage.sprite.rect.size.y), defaultHeight);


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

        inspectorWindowTitle.text = "Body of Water";
        inspectorWindowImage.sprite = liquidSprite;
        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0, inspectorWindowImage.sprite.rect.size.x, defaultHeight / inspectorWindowImage.sprite.rect.size.y), defaultHeight);


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
        DetailButton.SetActive(false);
        detailBackground.SetActive(false);
        detailText.text = "";
        inspectorWindowImage.sprite = defaultSprite;
        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0, inspectorWindowImage.sprite.rect.size.x, defaultHeight / inspectorWindowImage.sprite.rect.size.y), defaultHeight);

        inspectorWindowTitle.text = "Title";
        foreach (GameObject obj in needSliders) {
            Destroy(obj);
        }
        needSliders.Clear();
    }

    private void GenerateSliders(Dictionary<string, Need> needs, ref string displayText) {
        Dictionary<NeedType, List<Need>> needByType = new Dictionary<NeedType, List<Need>>();
        Dictionary<NeedType, float> needSatisfaction = new Dictionary<NeedType, float>();
        foreach (var pair in needs)
        {
            displayText += $"\n{pair.Key}: {pair.Value.NeedValue}";

            if (needByType.ContainsKey(pair.Value.NeedType))
            {
                needByType[pair.Value.NeedType].Add(pair.Value);
            }
            else {
                needByType[pair.Value.NeedType] = new List<Need>();
                needByType[pair.Value.NeedType].Add(pair.Value);
            }
        }

        foreach (var pair in needByType)
        {
            int numNeeds = pair.Value.Count;
            int totalSatisfaction = 0;

            foreach (Need need in pair.Value) {
                NeedCondition condition = need.GetCondition(need.NeedValue);

                //TODO Find a better function that converts the raw need value to need satisfaction (from -1 to 1).
                //currently finding the average satisfaction in the type
                if (condition == NeedCondition.Bad)
                    totalSatisfaction -= 1;
                else if (condition == NeedCondition.Good) {
                    totalSatisfaction += 1;
                }
                //neutral is 0

            }

            needSatisfaction[pair.Key] = ((float)totalSatisfaction) / numNeeds;
        }

        foreach (var pair in needSatisfaction) {
            GameObject sliderObj = Instantiate(NeedSliderPrefab, layoutGroupRect);
            needSliders.Add(sliderObj);

            NeedSlider slider = sliderObj.GetComponent<NeedSlider>();
            slider.SetName(pair.Key.ToString());
            slider.SetValue(pair.Value);
        }
    }
}
