using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayInspectorText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inspectorWindowTitle = default;
    [SerializeField] private RectTransform layoutGroupRect = default;
    [SerializeField] private TextMeshProUGUI inspectorWindowText = default;
    [SerializeField] private TextMeshProUGUI populationInfoText = default;
    [SerializeField] private Image populationStatusIndicator = default;
    [SerializeField] private Sprite populationIncreasingIcon = default;
    [SerializeField] private Sprite populationStagnantIcon = default;
    [SerializeField] private Sprite populationDecreasingIcon = default;
    [SerializeField] private Color populationGrowingColor = default;
    [SerializeField] private Color populationDecayingColor = default;
    [SerializeField] private GameObject DetailButton = default;
    [SerializeField] private GameObject detailBackground = default;
    [SerializeField] private Text detailText = default;
    [SerializeField] private GameObject NeedSliderPrefab = null;
    public InspectorText CurrentDisplay => currentDisplay;
    private InspectorText currentDisplay = InspectorText.Population;
    public enum InspectorText { Population, Food, Area, Liquid, Nothing }

    private List<GameObject> needSliders = new List<GameObject>();
    private Color populationDefaultColor;

    private void Awake() {
        populationDefaultColor = populationInfoText.color;
    }

    public void DisplayPopulationStatus(Population population)
    {
        ClearInspectorWindow();

        currentDisplay = InspectorText.Population;
        inspectorWindowTitle.text = population.species.ID.Data.Name.Get(ItemName.Type.Colloquial);
        populationInfoText.text = "Population: " + population.Count;

        DetailButton.SetActive(true);
        detailBackground.SetActive(false);
        populationStatusIndicator.enabled = true;

        // Check to make sure that the population needs have been cached
        if (population.GrowthCalculator.HasNeedCache)
        {
            string displayName = population.Species.ID.Data.Name.Get(ItemName.Type.Colloquial);
            switch (population.GrowthCalculator.GrowthStatus)
            {
                case GrowthStatus.growing:
                    detailText.text = $"{displayName} " +
                        $"population will increase in {population.DaysTillGrowth()} days";
                    populationInfoText.color = populationGrowingColor;
                    populationStatusIndicator.sprite = populationIncreasingIcon;
                    break;
                case GrowthStatus.stagnant:
                    detailText.text = $"{displayName} " +
                        $"is stagnate";
                    populationInfoText.color = populationDefaultColor;
                    populationStatusIndicator.sprite = populationStagnantIcon;
                    break;
                case GrowthStatus.decaying:
                    detailText.text = $"{displayName} " +
                        $"population will decrease in {population.DaysTillDeath()} days";
                    populationInfoText.color = populationDecayingColor;
                    populationStatusIndicator.sprite = populationDecreasingIcon;
                    break;
            }

            this.inspectorWindowText.text = "";

            // Gotta handle predator prey differently
            if (population.GrowthCalculator.Rating.PredatorCount > 0)
            {
                this.inspectorWindowText.text = $"{population.gameObject.name} looks frightened...";
            }

            GenerateSliders(population);
        }
        // If there is no need cache then tell the player to wait a day
        else
        {
            detailText.text = "Please wait 1 day for the population to get accustomed to the enclosure";
            inspectorWindowText.text = "Please wait 1 day for the population to get accustomed to the enclosure";
            populationInfoText.color = populationDefaultColor;
            populationStatusIndicator.sprite = populationStagnantIcon;
        }
    }

    public void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        ClearInspectorWindow();

        currentDisplay = InspectorText.Food;
        inspectorWindowTitle.text = foodSource.Species.ID.Data.Name.Get(ItemName.Type.Colloquial);

        string displayText;
        bool hasNeeds = GameManager.Instance.Needs.HasCache(foodSource);

        if (foodSource.isUnderConstruction || !hasNeeds)
        {
            displayText = $"Under Construction \n";
        }
        else
        {
            displayText = $"Output: {foodSource.FoodOutput}\n";
            GenerateSliders(foodSource);
        }
        this.inspectorWindowText.text = displayText;
    }

    public void DisplayEnclosedArea(EnclosedArea enclosedArea)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Area;

        inspectorWindowTitle.text = $"Enclosure {enclosedArea.id + 1}";

        // THe composition is a list of float value in the order of the AtmoshpereComponent Enum
        float[] terrainComposition = enclosedArea.terrainComposition;

        string displayText = "";

        // Atmospheric info
        //displayText += "Atmospheric composition: \n";
        //foreach (var (value, index) in atmosphericComposition.WithIndex())
        //{
        //    displayText += $"{((AtmosphereComponent)index).ToString()} : {value}\n";
        //}

        foreach (var (value, index) in terrainComposition.WithIndex())
        {
            if (value == 0)
            {
                continue;
            }
            displayText += $"{((TileType)index).ToString()} : {value}\n";
        }

        //displayText += "\n";
        //displayText += $"Population count: {enclosedArea.populations.Count}\n";
        //displayText += $"Total animal count: {enclosedArea.animals.Count}\n";
        //displayText += $"Food Source count: {enclosedArea.foodSources.Count}\n";

        this.inspectorWindowText.text = displayText;
    }

    public void DisplayLiquidCompisition(float[] compositions)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Liquid;

        inspectorWindowTitle.text = "Body of Water";

        string displayText = "";
        if (compositions == null)
        {
            displayText = "Water : 0.000\n Salt : 0.000\n Bacteria : 0.000";
        }
        else
        {
            string[] liquidName = new string[] { "Water", "Salt", "Bacteria" };
            int decimalDigitsShown = 3;
            double truncFactor = Mathf.Pow(10, decimalDigitsShown);
            
            for (int i = 0; i < 3; i++)
            {
                double displayValue = System.Math.Truncate(compositions[i] * 100 * truncFactor) / truncFactor;
                displayText += $"{liquidName[i]} : {displayValue}%\n";
            }

        }
        this.inspectorWindowText.text = displayText;
    }

    public void ClearInspectorWindow() {
        DetailButton.SetActive(false);
        detailBackground.SetActive(false);
        populationStatusIndicator.enabled = false;
        detailText.text = "";

        inspectorWindowTitle.text = "---";
        inspectorWindowText.text = "Click on an item to inspect it";
        populationInfoText.text = "";
        foreach (GameObject obj in needSliders) {
            Destroy(obj);
        }
        needSliders.Clear();
        currentDisplay = InspectorText.Nothing;
    }

    private void GenerateSliders(MonoBehaviour life) 
    {
        if (life is FoodSource)
        {
            setupSlider("Liquid", ((FoodSource)life).Rating.WaterRating);
            setupSlider("Terrain", ((FoodSource)life).Rating.TerrainRating);
        }
        if (life is Population)
        {
            Population population = life as Population;
            NeedRating rating = population.GrowthCalculator.Rating;

            if (rating.HasFoodNeed) setupSlider("Food", rating.FoodRating);
            if (rating.HasFriendNeed) setupSlider("Friend", rating.FriendRating);
            if (rating.HasTerrainNeed) setupSlider("Terrain", rating.TerrainRating);
            if (rating.HasTreeNeed) setupSlider("Tree", rating.TreeRating);
            if (rating.HasWaterNeed) setupSlider("Water", rating.WaterRating);
        }
    }

    private void setupSlider(string name, float value, int min = -1, int max = 1)
    {
        // Original ratings range from 0:2, so we change it to range from -1:1
        value--;
        GameObject sliderObj = Instantiate(NeedSliderPrefab, layoutGroupRect);
        needSliders.Add(sliderObj);
        NeedSlider slider = sliderObj.GetComponent<NeedSlider>();
        slider.SetName(name);
        slider.SetMinMax(min, max);
        slider.SetValue(value);
    }
}
