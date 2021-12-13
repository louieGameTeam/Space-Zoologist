using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CreateFoodCallback(FoodSource food);
/// <summary>
/// Manager of all the FoodSource instance
/// </summary>
public class FoodSourceManager : GridObjectManager
{
    public List<FoodSource> FoodSources => foodSources;
    private List<FoodSource> foodSources = new List<FoodSource>();
    private Dictionary<FoodSourceSpecies, List<FoodSource>> foodSourcesBySpecies = new Dictionary<FoodSourceSpecies, List<FoodSource>>();

    // FoodSourceSpecies to string name
    [SerializeField] private GameObject foodSourcePrefab = default;
    private GridSystem m_gridSystemReference;

    public void Initialize()
    {
        m_gridSystemReference = GameManager.Instance.m_gridSystem;
    }
    
    // TODO: combine two version into one
    public GameObject CreateFoodSource(FoodSourceSpecies species, Vector2 position, int ttb = -1)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        newFoodSourceGameObject.name = species.SpeciesName;
        FoodSource foodSource = newFoodSourceGameObject.GetComponent<FoodSource>();
        foodSource.InitializeFoodSource(species, position);
        foodSources.Add(foodSource);
        Vector2 pos = position;
        pos.x -= species.Size.x / 2;
        pos.y -= species.Size.y / 2;

        m_gridSystemReference.AddFoodReferenceToTile(m_gridSystemReference.WorldToCell(pos), species.Size, newFoodSourceGameObject);
        if (ttb > 0)
        {
            m_gridSystemReference.CreateRectangleBuffer(new Vector2Int((int)pos.x, (int)pos.y), ttb, species.Size,
                species.SpeciesName.Equals("Gold Space Maple") || species.SpeciesName.Equals("Space Maple") ? GridSystem.ConstructionCluster.ConstructionType.TREE : GridSystem.ConstructionCluster.ConstructionType.ONEFOOD);
            foodSource.isUnderConstruction = true;
            m_gridSystemReference.ConstructionFinishedCallback(() =>
            {
                foodSource.isUnderConstruction = false;
            });
        }

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
        ((FoodSourceNeedSystem)GameManager.Instance.NeedSystems[NeedType.FoodSource]).AddFoodSource(foodSource);

        // Register with NeedSystemManager
        GameManager.Instance.RegisterWithNeedSystems(foodSource);

        EventManager.Instance.InvokeEvent(EventType.NewFoodSource, newFoodSourceGameObject.GetComponent<FoodSource>());

        return newFoodSourceGameObject;
    }

    public GameObject CreateFoodSource(string foodsourceSpeciesID, Vector2 position)
    {
        return CreateFoodSource(GameManager.Instance.FoodSources[foodsourceSpeciesID], position);
    }

    public void DestroyFoodSource(FoodSource foodSource) {
        foodSources.Remove(foodSource);
        ((FoodSourceNeedSystem)GameManager.Instance.NeedSystems[NeedType.FoodSource]).RemoveFoodSource(foodSource);
        foodSourcesBySpecies[foodSource.Species].Remove(foodSource);
        GameManager.Instance.UnregisterWithNeedSystems(foodSource);
        m_gridSystemReference.RemoveFood(m_gridSystemReference.WorldToCell(foodSource.gameObject.transform.position));
        Destroy(foodSource.gameObject);
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
        if (GameManager.Instance.FoodSources.ContainsValue(species)) {
            for (var pair = GameManager.Instance.FoodSources.GetEnumerator(); pair.MoveNext() != false;) {
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
        if (!GameManager.Instance.FoodSources.ContainsKey(speciesName))
        {
            Debug.Log("Food source not in level data");
            return null;
        } 
        FoodSourceSpecies species = GameManager.Instance.FoodSources[speciesName];

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
            locations[i] = m_gridSystemReference.WorldToCell(foods[i].transform.position);
        }
        //Debug.Log("Returned locations");
        return locations;
    }
    public Vector3[] GetFoodSourcesWorldLocationWithSpecies(string speciesName)
    {
        List<FoodSource> foods = GetFoodSourcesWithSpecies(speciesName);
        if (foods == null)
        {
            //Debug.Log("returned null");
            return null;
        }
        
        Vector3[] locations = new Vector3[foods.Count];
        for (int i = 0; i < foods.Count; i++)
        {
            locations[i] = foods[i].transform.position;
        }
        return locations;
    }
    public override void Serialize(SerializedMapObjects serializedMapObjects)
    {
        foreach (string speciesName in GameManager.Instance.FoodSources.Keys)
        {
            serializedMapObjects.AddType(this.MapObjectName, new GridItemSet(this.GetSpeciesID(GameManager.Instance.FoodSources[speciesName]), this.GetFoodSourcesWorldLocationWithSpecies(speciesName)));
        }
    }
    public override void Parse()
    {
        DestroyAll();
        foreach (KeyValuePair<string, GridItemSet> keyValuePair in SerializedMapObjects)
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

    /// <summary>
    /// Debug function to remove all food sources
    /// </summary>
    public void DestroyAll()
    {
        while (foodSources.Count > 0)
        {
            this.DestroyFoodSource(foodSources[foodSources.Count - 1]);
        }
    }

    public void placeFood(Vector3Int mouseGridPosition, FoodSourceSpecies species, int ttb = -1)
    {
        Vector3 FoodLocation = m_gridSystemReference.CellToWorld(mouseGridPosition);
        FoodLocation.x += (float)species.Size.x / 2;
        FoodLocation.y += (float)species.Size.y / 2;
        CreateFoodSource(species, FoodLocation, ttb);
    }
}