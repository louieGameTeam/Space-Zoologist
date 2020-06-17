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

    [SerializeField] private GameObject foodSourcePrefab = default;

    private void Awake()
    {
        foreach (FoodSourceSpecies foodSourceSpecies in levelData.FoodSourceSpecies)
        {
            FoodSourceNeedSystem foodSourceNeedSystem = new FoodSourceNeedSystem(foodSourceSpecies.SpeciesName, rpm);
            foodSourceNeedSystems.Add(foodSourceSpecies, foodSourceNeedSystem);
        }
    }

    private void Start()
    {
        foodSources.AddRange(FindObjectsOfType<FoodSource>());
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            needSystemManager.AddSystem(foodSourceNeedSystem);
        }

        // Register with NeedSystemManager
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
        foodSourceNeedSystems[foodSource.Species].AddFoodSource(foodSource);

        // Register with NeedSystemManager
        needSystemManager.RegisterWithNeedSystems(foodSource);
    }

    public void UpdateFoodSources()
    {
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            foodSourceNeedSystem.UpdateSystem();
        }
    }
}
