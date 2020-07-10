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
    private Dictionary<FoodSourceSpecies, FoodSourceNeedSystem> foodSourceNeedSystems = new Dictionary<FoodSourceSpecies, FoodSourceNeedSystem>();
    private Dictionary<string, FoodSourceSpecies> foodSourceMapping = new Dictionary<string, FoodSourceSpecies>();

    [SerializeField] private GameObject foodSourcePrefab = default;

    // Create FoodSourceNeedSystems for each food species
    private void Awake()
    {
        // Add new FoodSourceNeedSystem
        foreach (FoodSourceSpecies foodSourceSpecies in levelData.FoodSourceSpecies)
        {
            foodSourceMapping.Add(foodSourceSpecies.SpeciesName, foodSourceSpecies);
            FoodSourceNeedSystem foodSourceNeedSystem = new FoodSourceNeedSystem(foodSourceSpecies.SpeciesName, rpm);
            foodSourceNeedSystems.Add(foodSourceSpecies, foodSourceNeedSystem);
        }
    }

    private void Start()
    {
        // Get all FoodSource at start of level
        foodSources.AddRange(FindObjectsOfType<FoodSource>());

        // Add FoodSourceNeedSystems to NeedSystemManager
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            needSystemManager.AddSystem(foodSourceNeedSystem);
        }

        // Register Foodsource with NeedSystem via NeedSystemManager
        foreach (FoodSource foodSource in foodSources)
        {
            needSystemManager.RegisterWithNeedSystems(foodSource);
        }
    }

    public void CreateFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        Debug.Log("Food source created");
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position);
        foodSources.Add(foodSource);
        foodSourceNeedSystems[foodSource.Species].AddFoodSource(foodSource);

        // Register with NeedSystemManager
        needSystemManager.RegisterWithNeedSystems(foodSource);
    }

    public void CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        FoodSourceSpecies foodSourceSpecies = null;
        if (!foodSourceMapping.TryGetValue(foodsourceSpeciesID, out foodSourceSpecies))
        {
           throw new System.ArgumentException("foodsourceSpeciesID was not found in the FoodsourceManager's foodsources");
        }
        CreateFoodSource(foodSourceSpecies, position);
    }

    public void UpdateFoodSources()
    {
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            foodSourceNeedSystem.UpdateSystem();
        }
    }
}
