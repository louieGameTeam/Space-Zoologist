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
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("If true, this verbose inspector tries to find an inspector " +
        "in the scene to connect to as soon as the scene begins")]
    private bool connectOnAwake = false;
    [SerializeField]
    [Tooltip("Prefab used to display an object from the verbose inspector")]
    private VerboseInspectorItem itemPrefab = null;
    [SerializeField]
    [Tooltip("Layout group to display the items in")]
    private Transform itemParent = null;
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
            TileData tileData = gameManager.m_tileDataController.GetTileData(inspector.selectedPosition);
            CreateInspectorItem(tileData);

            if (tileData.Food)
            {
                // Inspect the food source component
                FoodSource food = tileData.Food.GetComponent<FoodSource>();
                CreateInspectorItem(food, "FoodSource");
                CreateInspectorItem(food.Species);

                // Make sure the food source has a rating before trying to access it
                if (food.HasNeedCache)
                {
                    CreateInspectorItem(new SerializableNeedAvailability(food.Availability), "FoodSourceNeedAvailability");
                    CreateInspectorItem(food.Rating, "FoodSourceNeedRating");
                }
            }
            if (tileData.Animal)
            {
                // Get some references to the objects we will inspect
                Animal animal = tileData.Animal.GetComponent<Animal>();
                SerializablePopulation population = new SerializablePopulation(animal.PopulationInfo);
                SerializableGrowthCalculator growthCalculator = new SerializableGrowthCalculator(animal.PopulationInfo.GrowthCalculator);

                CreateInspectorItem(animal.PopulationInfo.Species);
                CreateInspectorItem(population, "Population");
                CreateInspectorItem(growthCalculator, "GrowthCalculator");

                // Make sure the population has a rating before trying to access it
                if (animal.PopulationInfo.GrowthCalculator.HasNeedCache)
                {
                    CreateInspectorItem(new SerializableNeedAvailability(animal.PopulationInfo.GrowthCalculator.Availabilty), "PopulationNeedAvailability");
                    CreateInspectorItem(animal.PopulationInfo.GrowthCalculator.Rating, "PopulationNeedRating");
                }
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
