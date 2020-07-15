using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSourceManager : MonoBehaviour
{
    public List<FoodSource> FoodSources => foodSources;

    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private LevelData levelData = default;
    [SerializeField] private ReservePartitionManager rpm = default;
    private List<FoodSource> foodSources = new List<FoodSource>();
    // Having food distribution system in FoodSourceManager is questionable
    private Dictionary<string, FoodSourceNeedSystem> foodSourceNeedSystems = new Dictionary<string, FoodSourceNeedSystem>();
    // FoodSourceSpecies to string name
    private Dictionary<string, FoodSourceSpecies> foodSourceSpecies = new Dictionary<string, FoodSourceSpecies>();

    [SerializeField] private GameObject foodSourcePrefab = default;

    private void Awake()
    {
        // Fill string to FoodSourceSpecies Dictionary
        foreach (FoodSourceSpecies species in levelData.FoodSourceSpecies)
        {
            foodSourceSpecies.Add(species.SpeciesName, species);
        }
    }

    private void Start()
    {
        // Get the FoodSourceNeedSystems from NeedSystemManager
        foreach (NeedSystem system in needSystemManager.Systems.Values)
        {
            if (foodSourceSpecies.ContainsKey(system.NeedName))
            {
                foodSourceNeedSystems.Add(system.NeedName, (FoodSourceNeedSystem)system);
            }
        }

        // Get all FoodSource at start of level
        foodSources.AddRange(FindObjectsOfType<FoodSource>());

        // Register Foodsource with NeedSystem via NeedSystemManager
        foreach (FoodSource foodSource in foodSources)
        {
            needSystemManager.RegisterWithNeedSystems(foodSource);
        }
    }

    public void CreateFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position);
        foodSources.Add(foodSource);
        foodSourceNeedSystems[foodSource.Species.SpeciesName].AddFoodSource(foodSource);

        // Register with NeedSystemManager
        needSystemManager.RegisterWithNeedSystems(foodSource);
    }

    public void CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        //FoodSourceSpecies foodSourceSpecies = null;
        ////if (!nameSpeciesMapping.TryGetValue(foodsourceSpeciesID, out foodSourceSpecies))
        ////{
        ////    throw new System.ArgumentException("foodsourceSpeciesID was not found in the FoodsourceManager's foodsources");
        ////}

        //CreateFoodSource(foodSourceSpecies, position);

        //// Register with NeedSystemManager
        //needSystemManager.RegisterWithNeedSystems(foodSource);
    }

    public void UpdateFoodSources()
    {
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            foodSourceNeedSystem.UpdateSystem();
        }
    }
}