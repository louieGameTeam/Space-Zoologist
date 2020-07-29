using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO refactor to use reference util instead of foodSourceSpecies and refactor current food source subscription logic flow
public class FoodSourceManager : MonoBehaviour
{
    public List<FoodSource> FoodSources => foodSources;

    private List<FoodSource> foodSources = new List<FoodSource>();
    // Having food distribution system in FoodSourceManager is questionable
    // TODO: remove this, this is in the NeedSystemManager
    private Dictionary<string, FoodSourceNeedSystem> foodSourceNeedSystems = new Dictionary<string, FoodSourceNeedSystem>();
    // FoodSourceSpecies to string name
    private Dictionary<string, FoodSourceSpecies> foodSourceSpecies = new Dictionary<string, FoodSourceSpecies>();

    [SerializeField] private GameObject foodSourcePrefab = default;
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    [SerializeField] LevelDataReference LevelDataReference = default;

    private void Start()
    {
        // Fill string to FoodSourceSpecies Dictionary
        foreach (FoodSourceSpecies species in LevelDataReference.LevelData.FoodSourceSpecies)
        {
            foodSourceSpecies.Add(species.SpeciesName, species);
        }
    }

    public void Initialize()
    {
        // Get the FoodSourceNeedSystems from NeedSystemManager
        foreach (NeedSystem system in NeedSystemManager.Systems.Values)
        {
            if (foodSourceSpecies.ContainsKey(system.NeedName))
            {
                foodSourceNeedSystems.Add(system.NeedName, (FoodSourceNeedSystem)system);
            }
        }

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
    // UPdateSystem runs through all food sources and has them calculate and send their output to the populations


    private void CreateFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position);
        foodSources.Add(foodSource);
        Debug.Log("Current food need systems: ");
        foreach(KeyValuePair<string, FoodSourceNeedSystem> foodNeedSystem in this.foodSourceNeedSystems)
        {
            Debug.Log(foodNeedSystem.Key);
        }
        Debug.Log("Food source being added: " + foodSource.Species.SpeciesName);
        //foodSourceNeedSystems[foodSource.Species.SpeciesName].AddFoodSource(foodSource);

        // Register with NeedSystemManager
        NeedSystemManager.RegisterWithNeedSystems(foodSource);
    }

    public void CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        CreateFoodSource(foodSourceSpecies[foodsourceSpeciesID], position);
    }

    public void UpdateFoodSourceSpecies(FoodSourceSpecies species)
    {
        this.foodSourceSpecies.Add(species.SpeciesName, species);
    }

    // Deprecated
    public void UpdateFoodSources()
    {
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            foodSourceNeedSystem.UpdateSystem();
        }
    }
}