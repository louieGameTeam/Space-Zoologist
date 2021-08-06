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
    [SerializeField] private GameObject TileCountPrefab = null;
    public InspectorText CurrentDisplay => currentDisplay;
    private InspectorText currentDisplay = InspectorText.Population;
    public enum InspectorText { Population, Food, Area, Liquid }

    private List<GameObject> tempInfo = new List<GameObject>();

    [Header("Temporary sprites")]
    [SerializeField] Sprite enclosedAreaSprite = default;
    [SerializeField] Sprite defaultSprite = default;

    // Matches type to sprite
    [SerializeField] List<TileAndSprite> sprites = default;
    Dictionary<TileType, Sprite> tileToSprite;
    [System.Serializable]
    public class TileAndSprite {
        public TileType type;
        public Sprite sprite;
    }

    GameObject detailBackground;
    Text detailText;
    float defaultHeight;
    public void Initialize()
    {
        defaultHeight = inspectorWindowImage.rectTransform.sizeDelta.y;

        detailBackground = DetailButton.transform.GetChild(0).gameObject;
        detailText = detailBackground.GetComponentInChildren<Text>(true);
        tileToSprite = new Dictionary<TileType, Sprite>();
        foreach (var pair in sprites) {
            tileToSprite.Add(pair.type, pair.sprite);
        }
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

        DetailButton.GetComponent<Button>().onClick.AddListener(() => UpdatePopulationDetails(population));

        UpdatePopulationDetails(population);

        this.inspectorWindowText.text = "";
        GenerateSliders(population);
    }

    public void UpdatePopulationDetails(Population population) {
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
    }

    public void DisplayFoodSourceStatus(FoodSource foodSource)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Food;
        inspectorWindowImage.sprite = foodSource.Species?.FoodSourceItem.Icon;
        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0, inspectorWindowImage.sprite.rect.size.x, defaultHeight / inspectorWindowImage.sprite.rect.size.y), defaultHeight);
        inspectorWindowTitle.text = foodSource.Species.SpeciesName;

        string displayText = $"\n";

        if (foodSource.isUnderConstruction)
        {
            displayText += $"Under Construction \n";

        }
        else
        {
            displayText += $"Output: {foodSource.FoodOutput}\n";

            GenerateSliders(foodSource);
        }
        this.inspectorWindowText.text = displayText;
    }

    public void DislplayEnclosedArea(EnclosedArea enclosedArea)
    {
        ClearInspectorWindow();
        currentDisplay = InspectorText.Area;

        inspectorWindowTitle.text = $"Enclosure {enclosedArea.id + 1}";
        inspectorWindowImage.sprite = enclosedAreaSprite;
        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0, inspectorWindowImage.sprite.rect.size.x, defaultHeight / inspectorWindowImage.sprite.rect.size.y), defaultHeight);


        // THe composition is a list of float value in the order of the AtmoshpereComponent Enum
        float[] atmosphericComposition = enclosedArea.atmosphericComposition.GetComposition();
        float[] terrainComposition = enclosedArea.terrainComposition;

        string displayText = $"\n";

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
            TileType type = (TileType)index;
            if (!tileToSprite.ContainsKey(type)) tileToSprite[type] = defaultSprite;
            SetupTileCount(type.ToString(), (int)value, tileToSprite[type]);
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
        inspectorWindowImage.sprite = tileToSprite[TileType.Liquid];
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

    public void ClearInspectorWindow()
    {
        DetailButton.GetComponent<Button>().onClick.RemoveAllListeners();
        DetailButton.SetActive(false);
        detailBackground.SetActive(false);
        detailText.text = "";
        inspectorWindowImage.sprite = defaultSprite;
        inspectorWindowImage.rectTransform.sizeDelta = new Vector2(Mathf.LerpUnclamped(0, inspectorWindowImage.sprite.rect.size.x, defaultHeight / inspectorWindowImage.sprite.rect.size.y), defaultHeight);

        inspectorWindowTitle.text = "Title";
        foreach (GameObject obj in tempInfo) {
            Destroy(obj);
        }
        tempInfo.Clear();
    }

    private void GenerateSliders(Life life) {
        if (life is FoodSource)
        {
            SetupSlider("Liquid", ((FoodSource)life).WaterRating);
            SetupSlider("Terrain", ((FoodSource)life).TerrainRating);
        }
        if (life is Population)
        {
            SetupSlider("Liquid", ((Population)life).GrowthCalculator.WaterRating, -5, 5);
            SetupSlider("Terrain", ((Population)life).GrowthCalculator.TerrainRating, -5, 5);
            SetupSlider("Food", ((Population)life).GrowthCalculator.FoodRating, -5, 5);
        }
    }

    private void SetupSlider(string name, float value, int min = 0, int max = 10)
    {
        GameObject sliderObj = Instantiate(NeedSliderPrefab, layoutGroupRect);
        tempInfo.Add(sliderObj);
        NeedSlider slider = sliderObj.GetComponent<NeedSlider>();
        slider.max = max;
        slider.min = min;
        slider.SetName(name);
        slider.SetValue(value);
    }

    private void SetupTileCount(string name, int count, Sprite image)
    {
        GameObject tileCountObj = Instantiate(TileCountPrefab, layoutGroupRect);
        tempInfo.Add(tileCountObj);
        TileCount script = tileCountObj.GetComponent<TileCount>();
        script.SetName(name);
        script.SetCount(count);
        script.SetImage(image);
    }
}
