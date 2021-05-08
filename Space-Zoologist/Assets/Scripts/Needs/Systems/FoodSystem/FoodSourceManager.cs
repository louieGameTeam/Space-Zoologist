using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager of all the FoodSource instance
/// </summary>
public class FoodSourceManager : GridObjectManager
{
    public static FoodSourceManager ins;
    public List<FoodSource> FoodSources => foodSources;
    private List<FoodSource> foodSources = new List<FoodSource>();
    private Dictionary<FoodSourceSpecies, List<FoodSource>> foodSourcesBySpecies = new Dictionary<FoodSourceSpecies, List<FoodSource>>();

    // A reference to the food source need system
    private FoodSourceNeedSystem foodSourceNeedSystems = default;
    // FoodSourceSpecies to string name
    private Dictionary<string, FoodSourceSpecies> foodSourceSpecies = new Dictionary<string, FoodSourceSpecies>();
    [SerializeField] private GameObject foodSourcePrefab = default;
    [SerializeField] NeedSystemManager NeedSystemManager = default;
    [SerializeField] LevelDataReference LevelDataReference = default;
    [SerializeField] TileSystem TileSystem = default;
    [SerializeField] GridSystem GridSystem = default;

    private void Awake()
    {
        if (ins != null)
        {
            Destroy(gameObject);
        }
        else {
            ins = this;
        }
    }

    public override void Start()
    {
        foreach (FoodSourceSpecies species in this.LevelDataReference.LevelData.FoodSourceSpecies)
        {
            foodSourceSpecies.Add(species.SpeciesName, species);
        }
        base.Start();
    }

    public void Initialize()
    {
        // Get the FoodSourceNeedSystems from NeedSystemManager
        this.foodSourceNeedSystems = (FoodSourceNeedSystem)NeedSystemManager.Systems[NeedType.FoodSource];

        // Get all FoodSource at start of level
        // TODO make use of saved tile
        GameObject[] foods = GameObject.FindGameObjectsWithTag("FoodSource");

        foreach (GameObject food in foods)
        {
            foodSources.Add(food.GetComponent<FoodSource>());
            Vector3Int GridPosition = TileSystem.WorldToCell(food.transform.position);
            GridSystem.CellGrid[GridPosition.x, GridPosition.y].ContainsFood = true;
            GridSystem.CellGrid[GridPosition.x, GridPosition.y].Food = food;
        }

        // Register Foodsource with NeedSystem via NeedSystemManager
        foreach (FoodSource foodSource in foodSources)
        {
            if (!foodSourcesBySpecies.ContainsKey(foodSource.Species))
            {
                foodSourcesBySpecies.Add(foodSource.Species, new List<FoodSource>());
                foodSourcesBySpecies[foodSource.Species].Add(foodSource);
            }
            else {
                foodSourcesBySpecies[foodSource.Species].Add(foodSource);
            }
            
            this.foodSourceNeedSystems.AddFoodSource(foodSource);
            NeedSystemManager.RegisterWithNeedSystems(foodSource);
            EventManager.Instance.InvokeEvent(EventType.NewFoodSource, foodSource);
        }
    }

    // TODO: combine two version into one
    private GameObject CreateFoodSource(FoodSourceSpecies species, Vector2 position)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position, this.TileSystem);
        foodSources.Add(foodSource);

        if (!foodSourcesBySpecies.ContainsKey(foodSource.Species))
        {
            foodSourcesBySpecies.Add(foodSource.Species, new List<FoodSource>());
            foodSourcesBySpecies[foodSource.Species].Add(foodSource);
        }
        else
        {
            foodSourcesBySpecies[foodSource.Species].Add(foodSource);
        }

        //Debug.Log("Food source being added: " + foodSource.Species.SpeciesName);
        this.foodSourceNeedSystems.AddFoodSource(foodSource);

        // Register with NeedSystemManager
        NeedSystemManager.RegisterWithNeedSystems(foodSource);

        EventManager.Instance.InvokeEvent(EventType.NewFoodSource, newFoodSourceGameObject.GetComponent<FoodSource>());

        return newFoodSourceGameObject;
    }

    public GameObject CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        return CreateFoodSource(foodSourceSpecies[foodsourceSpeciesID], position);
    }

    public void DestroyFoodSource(FoodSource foodSource) {
        foodSources.Remove(foodSource);
        foodSourceNeedSystems.RemoveFoodSource(foodSource);
        foodSourcesBySpecies[foodSource.Species].Remove(foodSource);
        NeedSystemManager.UnregisterWithNeedSystems(foodSource);
        Destroy(foodSource.gameObject);
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

    public string GetSpeciesID(FoodSourceSpecies species) {
        if (foodSourceSpecies.ContainsValue(species)) {
            for (var pair = foodSourceSpecies.GetEnumerator(); pair.MoveNext() != false;) {
                if (pair.Current.Value.Equals(species)) {
                    return pair.Current.Key;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Get a list of Food Source with the given species name.
    /// </summary>
    /// <param name="speciesName">Same as FoodSourceSpecies.SpeciesName</param>
    /// <returns>An list of Food Source with the given species name</returns>
    public List<FoodSource> GetFoodSourcesWithSpecies(string speciesName) {
        // Given species doesn't exist in the level
        if (!foodSourceSpecies.ContainsKey(speciesName)) return null;
        FoodSourceSpecies species = foodSourceSpecies[speciesName];

        // No food source of given species exist
        if (!foodSourcesBySpecies.ContainsKey(species))
        {
            return null;
        }
        else {
            return foodSourcesBySpecies[species];
        }
    }

    /// <summary>
    /// Get an array of tile positions of Food Source with the given species name.Used to bypass having access to TileSystem.
    /// </summary>
    /// <param name="speciesName">Same as FoodSourceSpecies.SpeciesName</param>
    /// <returns>An array of tile positions of Food Source with the given species name</returns>
    public Vector3Int[] GetFoodSourcesLocationWithSpecies(string speciesName) {
        List<FoodSource> foods = GetFoodSourcesWithSpecies(speciesName);
        if (foods == null) return null;
        Vector3Int[] locations = new Vector3Int[foods.Count];
        for (int i = 0; i < foods.Count; i++) {
            locations[i] = TileSystem.WorldToCell(foods[i].transform.position);
        }

        return locations;
    }
    public Vector3[] GetFoodSourcesWorldLocationWithSpecies(string speciesName)
    {
        List<FoodSource> foods = GetFoodSourcesWithSpecies(speciesName);
        if (foods == null) return null;
        Vector3[] locations = new Vector3[foods.Count];
        for (int i = 0; i < foods.Count; i++)
        {
            locations[i] = foods[i].transform.position;
        }

        return locations;
    }
    public override void Serialize(SerializedMapObjects serializedMapObjects)
    {
        foreach (string speciesName in this.foodSourceSpecies.Keys)
        {
            serializedMapObjects.AddType(this.MapObjectName, new GridItemSet(this.GetSpeciesID(this.foodSourceSpecies[speciesName]), this.GetFoodSourcesWorldLocationWithSpecies(speciesName)));
        }
    }
    public override void Parse(SerializedMapObjects serializedMapObjects)
    {
        foreach (KeyValuePair<string, GridItemSet> keyValuePair in serializedMapObjects.ToDictionary())
        {
            if (keyValuePair.Key.Equals(this.MapObjectName))
            {
                foreach (Vector3 position in SerializationUtils.ParseVector3(keyValuePair.Value.coords))
                {
                    this.CreateFoodSource(keyValuePair.Value.name, position);
                }
            }
        }
    }
    protected override string GetMapObjectName()
    {
        // String used to identify serialized map objects being handled by this manager
        return "FoodSource";
    }
}