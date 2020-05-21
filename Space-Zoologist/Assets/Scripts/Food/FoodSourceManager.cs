using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSourceManager : MonoBehaviour
{
    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private LevelData levelData = default;
    [SerializeField] private ReservePartitionManager rpm = default;
    private List<FoodSource> foodSources = new List<FoodSource>();
    // Having food distribution system in FoodSourceManager is questionable
    private Dictionary<FoodSourceSpecies, FoodSourceNeedSystem> foodSourceNeedSystems = new Dictionary<FoodSourceSpecies, FoodSourceNeedSystem>();
    

    [SerializeField] private GameObject foodSourcePrefab = default;

    private void Awake()
    {
        foreach (FoodSourceSpecies foodSourceSpecies in levelData.FoodSources)
        {
            FoodSourceNeedSystem foodSourceNeedSystem = new FoodSourceNeedSystem(foodSourceSpecies.SpeciesName, rpm);
            foodSourceNeedSystems.Add(foodSourceSpecies, foodSourceNeedSystem);
        }
    }

    private void Start()
    {
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            needSystemManager.AddSystem(foodSourceNeedSystem);
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
    }

    public void UpdateFoodSources()
    {
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodSourceNeedSystems.Values)
        {
            foodSourceNeedSystem.UpdateSystem();
        }
    }
}
