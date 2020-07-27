using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSourceManager : MonoBehaviour
{
    // Singleton
    public static FoodSourceManager ins;

    public List<FoodSource> FoodSources => foodSources;
    private List<FoodSource> foodSources = new List<FoodSource>();

    [SerializeField] private LevelData levelData = default;
    // A reference to the food source need system
    private FoodSourceNeedSystem foodSourceNeedSystems = default;
    // FoodSourceSpecies to string name
    private Dictionary<string, FoodSourceSpecies> foodSourceSpecies = new Dictionary<string, FoodSourceSpecies>();

    [SerializeField] private GameObject foodSourcePrefab = default;

    private void Awake()
    {
        if(ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }

        // Fill string to FoodSourceSpecies Dictionary
        foreach (FoodSourceSpecies species in levelData.FoodSourceSpecies)
        {
            foodSourceSpecies.Add(species.SpeciesName, species);
        }
    }

    public void Initialize()
    {
        this.foodSourceNeedSystems = (FoodSourceNeedSystem)NeedSystemManager.ins.Systems["FoodSource"];

        // Get all FoodSource at start of level
        foodSources.AddRange(FindObjectsOfType<FoodSource>());

        // Register Foodsource with NeedSystem via NeedSystemManager
        foreach (FoodSource foodSource in foodSources)
        {
            NeedSystemManager.ins.RegisterWithNeedSystems(foodSource);
            this.foodSourceNeedSystems.AddFoodSource(foodSource);
        }
    }

    private void CreateFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position);
        foodSources.Add(foodSource);
        
        // Register with NeedSystemManager
        NeedSystemManager.ins.RegisterWithNeedSystems(foodSource);

        // Add food source as comsuned source to FS NS
        this.foodSourceNeedSystems.AddFoodSource(foodSource);
    }

    public void CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        CreateFoodSource(foodSourceSpecies[foodsourceSpeciesID], position);
    }
}