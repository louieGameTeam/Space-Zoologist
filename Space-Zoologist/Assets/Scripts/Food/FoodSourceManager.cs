using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSourceManager : MonoBehaviour
{
    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private LevelData levelData = default;
    [SerializeField] private ReservePartitionManager rpm = default;
    private List<FoodSource> foodSources = new List<FoodSource>();
    private Dictionary<FoodSourceSpecies, FoodSourceNeedSystem> foodSourceNeedSystems = new Dictionary<FoodSourceSpecies, FoodSourceNeedSystem>();
    private Dictionary<string, FoodSourceSpecies> nameSpeciesMapping = new Dictionary<string, FoodSourceSpecies>();

    [SerializeField] private GameObject foodSourcePrefab = default;

    private void Awake()
    {
        foreach (FoodSourceSpecies species in levelData.FoodSourceSpecies)
        {
            FoodSourceNeedSystem needSystem = new FoodSourceNeedSystem(species.SpeciesName, rpm);
            foodSourceNeedSystems.Add(species, needSystem);
            nameSpeciesMapping.Add(species.SpeciesName, species);
        }
    }

    private void Start()
    {
        foreach (FoodSourceNeedSystem needSystem in foodSourceNeedSystems.Values)
        {
            needSystemManager.AddSystem(needSystem);
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

    public void CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        FoodSourceSpecies foodSourceSpecies = null;
        if (!nameSpeciesMapping.TryGetValue(foodsourceSpeciesID, out foodSourceSpecies))
        {
            throw new System.ArgumentException("foodsourceSpeciesID was not found in the FoodsourceManager's foodsources");
        }

        CreateFoodSource(foodSourceSpecies, position);
    }

    public void UpdateFoodSources()
    {
        foreach (FoodSourceNeedSystem needSystem in foodSourceNeedSystems.Values)
        {
            needSystem.UpdateSystem();
        }
    }
}
