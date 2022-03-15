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
    [SerializeField] private GameObject DetailButton = default;
    [SerializeField] private GameObject detailBackground = default;
    [SerializeField] private Text detailText = default;
    [SerializeField] private GameObject NeedSliderPrefab = null;
    public InspectorText CurrentDisplay => currentDisplay;
    private InspectorText currentDisplay = InspectorText.Population;
    public enum InspectorText { Population, Food, Area, Liquid }

    private List<GameObject> needSliders = new List<GameObject>();

    public void DisplayPopulationStatus(Population population)
    {
        ClearInspectorWindow();

        currentDisplay = InspectorText.Population;
        inspectorWindowTitle.text = population.species.ID.Data.Name.Get(ItemName.Type.Colloquial);
        populationInfoText.text = "Population: " + population.Count;

        DetailButton.SetActive(true);
        detailBackground.SetActive(false);

        if (GameManager.Instance.needRatings.HasRating(population))
        {
            switch (population.GrowthCalculator.GrowthStatus)
            {
                case GrowthStatus.growing:
                    detailText.text = $"{population.Species.ID.Data.Name.Get(ItemName.Type.Colloquial)} " +
                        $"population will increase in {population.DaysTillGrowth()} days";
                    break;
                case GrowthStatus.stagnant:
                    detailText.text = $"{population.Species.ID.Data.Name.Get(ItemName.Type.Colloquial)} " +
                        $"is stagnate";
                    break;
                case GrowthStatus.decaying:
                    detailText.text = $"{population.Species.ID.Data.Name.Get(ItemName.Type.Colloquial)} " +
                        $"population will decrease in {population.DaysTillDeath()} days";
                    break;
            }

            this.inspectorWindowText.text = "";

            // Gotta handle predator prey differently
            //if (population.GrowthCalculator.calculatePredatorPreyNeed() > 0)
            //{
            //    this.inspectorWindowText.text = $"{population.gameObject.name} looks frightened...";
            //}

            GenerateSliders(population);
        }
        else
        {
            detailText.text = "Please wait 1 day for the population to get accustomed to the enclosure";
        }
    }

    public void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        ClearInspectorWindow();

        currentDisplay = InspectorText.Food;
        inspectorWindowTitle.text = foodSource.Species.ID.Data.Name.Get(ItemName.Type.Colloquial);

        string displayText;
        bool hasNeeds = GameManager.Instance.needRatings.HasRating(foodSource);

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
            for (int i = 0; i < 3; i++)
            {
                displayText += $"{liquidName[i]} : {System.Math.Round(compositions[i] * 100, 3)}%\n";
            }

        }
        this.inspectorWindowText.text = displayText;
    }

    public void ClearInspectorWindow() {
        DetailButton.SetActive(false);
        detailBackground.SetActive(false);
        detailText.text = "";

        inspectorWindowTitle.text = "Title";
        inspectorWindowText.text = "Click on an item to inspect it";
        populationInfoText.text = "";
        foreach (GameObject obj in needSliders) {
            Destroy(obj);
        }
        needSliders.Clear();
    }

    private void GenerateSliders(Life life) 
    {
        if (life is FoodSource)
        {
            setupSlider("Liquid", ((FoodSource)life).Rating.WaterRating);
            setupSlider("Terrain", ((FoodSource)life).Rating.TerrainRating);
        }
        if (life is Population)
        {
            setupSlider("Liquid", ((Population)life).GrowthCalculator.Rating.WaterRating);
            setupSlider("Terrain", ((Population)life).GrowthCalculator.Rating.TerrainRating);
            setupSlider("Food", ((Population)life).GrowthCalculator.Rating.FoodRating);
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
