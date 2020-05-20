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
    private FoodDistributionSystem foodDistributionSystem = null;
    

    [SerializeField] private GameObject foodSourcePrefab = default;

    private void Awake()
    {
        foodDistributionSystem = new FoodDistributionSystem(levelData, rpm);
        //AddFoodSources(FindObjectsOfType<FoodSource>());
    }

    private void Start()
    {
        foreach (FoodSourceNeedSystem foodSourceNeedSystem in foodDistributionSystem.FoodSourceNeedSystems)
        {
            //Debug.Log($"Adding {foodSourceNeedSystem.NeedName} FoodSourceNeedSystem to NeedSystemManager");
            needSystemManager.AddSystem(foodSourceNeedSystem);
        }
    }

    public void CreateFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position);
        this.AddFoodSource(foodSource);
    }

    public void AddFoodSource(FoodSource foodSource)
    {
        //Debug.Log($"Added {foodSource.Species.name} FoodSource to FoodSourceManager");
        foodSources.Add(foodSource);
        foodDistributionSystem.AddFoodSource(foodSource);
    }

    public void AddFoodSources(IEnumerable<FoodSource> newFoodSources)
    {
        foodSources.AddRange(newFoodSources);
        foodDistributionSystem.AddFoodSources(newFoodSources);
    }

    // This is also very questionable
    public void UpdateFoodDistributionSystem()
    {
        foodDistributionSystem.UpdateSystems();
    }
}
