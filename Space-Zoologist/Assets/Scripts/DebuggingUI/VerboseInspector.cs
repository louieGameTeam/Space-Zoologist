using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The verbose inspector takes the same selection as the in-game inspector
/// and displays a lot more information about it.  Great for debugging the game
/// </summary>
public class VerboseInspector : MonoBehaviour
{
    #region Public Typedefs
    [System.Serializable]
    public class LiquidbodyListWrapper
    {
        public List<LiquidBody> liquidBodies;

        public LiquidbodyListWrapper(IEnumerable<LiquidBody> liquidBodies)
        {
            this.liquidBodies = new List<LiquidBody>(liquidBodies);
        }
    }
    [System.Serializable]
    public class NeedMetEntry
    {
        public string need;
        public bool met;

        public NeedMetEntry(KeyValuePair<NeedType, bool> entry)
        {
            need = entry.Key.ToString();
            met = entry.Value;
        }
    }
    [System.Serializable]
    public class SerializableGrowthCalculator
    {
        public float maxFreshWaterTilePercent = GrowthCalculator.maxFreshWaterTilePercent;
        public float maxSaltTilePercent = GrowthCalculator.maxSaltTilePercent;
        public float maxBacteriaTilePercent = GrowthCalculator.maxBacteriaTilePercent;

        public string growthStatus;
        public int growthCountdown;
        public int decayCountdown;
        public float foodRating;
        public float waterRating;
        public float terrainRating;
        public float populationIncreaseRate;
        public List<NeedMetEntry> isNeedMet;

        public SerializableGrowthCalculator(GrowthCalculator calculator)
        {
            growthStatus = calculator.GrowthStatus.ToString();
            growthCountdown = calculator.GrowthCountdown;
            decayCountdown = calculator.DecayCountdown;
            foodRating = calculator.FoodRating;
            waterRating = calculator.WaterRating;
            terrainRating = calculator.TerrainRating;
            populationIncreaseRate = calculator.populationIncreaseRate;

            // Create a list with all need met entries
            IEnumerable<NeedMetEntry> entries = calculator.IsNeedMet
                .Select(entry => new NeedMetEntry(entry));
            isNeedMet = new List<NeedMetEntry>(entries);
        }
    }
    [System.Serializable]
    public class SerializableNeed
    {
        public string needName;
        public string needType;
        public float needValue;
        public float severity;
        public bool isPreferred;

        public SerializableNeed(Need need)
        {
            needName = need.NeedName;
            needType = need.NeedType.ToString();
            needValue = need.NeedValue;
            severity = need.Severity;
            isPreferred = need.IsPreferred;
        }
    }
    [System.Serializable]
    public class SerializablePopulation
    {
        public int count;
        public int prePopulationCount;
        public float foodDominance;
        public Vector3 origin;
        public List<SerializableNeed> needs;
        public SerializableGrowthCalculator growthCalculator;
        public bool isPaused;
        public bool hasAccessibilityChanged;

        public SerializablePopulation(Population population)
        {
            // Set the first few fields
            count = population.Count;
            prePopulationCount = population.PrePopulationCount;
            foodDominance = population.FoodDominance;
            origin = population.Origin;

            // Create a list with all the entries in the needs dictionary
            IEnumerable<SerializableNeed> entries = population.Needs.Values
                .Select(need => new SerializableNeed(need));
            needs = new List<SerializableNeed>(entries);

            // Set the remaining fields
            growthCalculator = new SerializableGrowthCalculator(population.GrowthCalculator);
            isPaused = population.IsPaused;
            hasAccessibilityChanged = population.HasAccessibilityChanged;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("If true, this verbose inspector tries to find an inspector " +
        "in the scene to connect to as soon as the scene begins")]
    private bool connectOnAwake = false;
    [SerializeField]
    [Tooltip("Prefab used to display an object from the verbose inspector")]
    private VerboseInspectorItem itemPrefab;
    [SerializeField]
    [Tooltip("Layout group to display the items in")]
    private Transform itemParent;
    #endregion

    #region Private Fields
    private Inspector inspector;
    private List<VerboseInspectorItem> items = new List<VerboseInspectorItem>();
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        if (connectOnAwake)
        {
            ConnectInspector(FindObjectOfType<Inspector>());
        }
    }
    #endregion

    #region Public Methods
    public void ConnectInspector(Inspector inspector)
    {
        // Prevent re-connecting the same inspector
        if (this.inspector != inspector)
        {
            // If an inspector was previously connected, 
            // then remove this script's listener from it
            if (this.inspector)
            {
                this.inspector.SelectionChangedEvent.RemoveListener(UpdateItems);
            }

            // Listen for the selection changed event on the new inspector
            if (inspector)
            {
                inspector.SelectionChangedEvent.AddListener(UpdateItems);
            }

            // Set this inspector and update the text
            this.inspector = inspector;
            UpdateItems();
        }
    }
    #endregion

    #region Private Methods
    private void UpdateItems()
    {
        // Destroy any existing items on update
        foreach (VerboseInspectorItem item in items)
        {
            Destroy(item.gameObject);
        }
        // Clear out the list
        items.Clear();

        // Try to get the game manager
        GameManager gameManager = GameManager.Instance;

        // Check if there is a game manager and a connected inspector
        if(gameManager && inspector)
        {
            // Get the tile data at the inspector's position and display all the data as a JSON
            GridSystem.TileData tileData = gameManager.m_gridSystem.GetTileData(inspector.selectedPosition);
            CreateInspectorItem(tileData);

            if (tileData.Food)
            {
                // Inspect the food source component
                FoodSource food = tileData.Food.GetComponent<FoodSource>();
                CreateInspectorItem(food);
                CreateInspectorItem(food.Species);
            }
            if (tileData.Animal)
            {
                // Get some references to the objects we will inspect
                Animal animal = tileData.Animal.GetComponent<Animal>();
                SerializablePopulation population = new SerializablePopulation(animal.PopulationInfo);
                SerializableGrowthCalculator growthCalculator = new SerializableGrowthCalculator(animal.PopulationInfo.GrowthCalculator);

                CreateInspectorItem(animal.PopulationInfo.Species);
                CreateInspectorItem(population, "Population");
            }
            if (tileData.currentLiquidBody != null)
            {
                // Inspect the referenced liquid bodies
                LiquidbodyListWrapper referencedBodies = new LiquidbodyListWrapper(tileData.currentLiquidBody.referencedBodies);
                CreateInspectorItem(referencedBodies, "Referenced Liquid Bodies");
            }
        }
    }
    private void CreateInspectorItem(object item, string itemName = null)
    {
        VerboseInspectorItem itemUI = Instantiate(itemPrefab, itemParent);
        itemUI.DisplayItem(item, itemName);
        items.Add(itemUI);
    }
    #endregion
}
