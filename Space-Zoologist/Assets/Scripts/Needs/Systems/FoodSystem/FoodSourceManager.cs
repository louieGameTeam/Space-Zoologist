using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager of all the FoodSource instance
/// </summary>
// TODO refactor to use reference util instead of foodSourceSpecies and refactor current food source subscription logic flow
public class FoodSourceManager : MonoBehaviour
{
    public List<FoodSource> FoodSources => foodSources;
    private List<FoodSource> foodSources = new List<FoodSource>();

    // A reference to the food source need system
    private FoodSourceNeedSystem foodSourceNeedSystems = default;
    // FoodSourceSpecies to string name
    private Dictionary<string, FoodSourceSpecies> foodSourceSpecies = new Dictionary<string, FoodSourceSpecies>();
    [SerializeField] private GameObject foodSourcePrefab = default;
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    [SerializeField] LevelDataReference LevelDataReference = default;

    private void Start()
    {
        // Fill string to FoodSourceSpecies Dictionary
        //foreach (FoodSourceSpecies species in LevelDataReference.LevelData.FoodSourceSpecies)
        //{
        //    foodSourceSpecies.Add(species.SpeciesName, species);
        //}

        foreach (FoodSourceSpecies species in this.LevelDataReference.LevelData.FoodSourceSpecies)
        {
            foodSourceSpecies.Add(species.SpeciesName, species);
        }
    }

    public void Initialize()
    {
        // Get the FoodSourceNeedSystems from NeedSystemManager
        this.foodSourceNeedSystems = (FoodSourceNeedSystem)NeedSystemManager.Systems[NeedType.FoodSource];

        // Get all FoodSource at start of level
        GameObject[] foods = GameObject.FindGameObjectsWithTag("FoodSource");

        foreach(GameObject food in foods)
        {
            foodSources.Add(food.GetComponent<FoodSource>());
        }

        // Register Foodsource with NeedSystem via NeedSystemManager
        foreach (FoodSource foodSource in foodSources)
        {
            NeedSystemManager.RegisterWithNeedSystems(foodSource);
        }
    }

    // TODO: combine two version into one
    private GameObject CreateFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position);
        foodSources.Add(foodSource);

        //Debug.Log("Food source being added: " + foodSource.Species.SpeciesName);
        this.foodSourceNeedSystems.AddFoodSource(foodSource);

        // Register with NeedSystemManager
        NeedSystemManager.RegisterWithNeedSystems(foodSource);
        return newFoodSourceGameObject;
    }

    public GameObject CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        return CreateFoodSource(foodSourceSpecies[foodsourceSpeciesID], position);
    }

    // TODO: not sure what this does
    public void UpdateFoodSourceSpecies(FoodSourceSpecies species)
    {
        this.foodSourceSpecies.Add(species.SpeciesName, species);
    }

    /// <summary>
    /// Update accessible terrain info for all food sources,
    /// called when all NS updates are done
    /// </summary>
    public void UpdateAccessibleTerrainInfoForAll()
    {
        foreach (FoodSource foodSource in this.foodSources)
        {
            foodSource.UpdateAccessibleTerrainInfo();
        }
    }
}