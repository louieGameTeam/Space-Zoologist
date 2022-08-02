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
    private TileDataController m_gridSystemReference => GameManager.Instance.m_tileDataController;
    
    public GameObject CreateFoodSource(FoodSourceSpecies species, Vector2 position, int ttb = -1)
    {
        GameObject newFoodSourceGameObject = Instantiate(foodSourcePrefab, position, Quaternion.identity, this.transform);
        ItemName speciesName = species.ID.Data.Name;
        newFoodSourceGameObject.name = speciesName.Get(ItemName.Type.Serialized);
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
                speciesName.AnyNameContains("Gold Space Maple") || speciesName.AnyNameContains("Space Maple") ? TileDataController.ConstructionCluster.ConstructionType.TREE : TileDataController.ConstructionCluster.ConstructionType.ONEFOOD);
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

        // Invoke the event that occurs when a new food source is created
        EventManager.Instance.InvokeEvent(EventType.NewFoodSource, newFoodSourceGameObject.GetComponent<FoodSource>());
        EventManager.Instance.InvokeEvent(EventType.FoodSourceChange, newFoodSourceGameObject.GetComponent<FoodSource>());
        return newFoodSourceGameObject;
    }

    public GameObject CreateFoodSource(string foodsourceSpeciesID, Vector2 position, int ttb = -1)
    {
        ItemID id = ItemRegistry.FindHasName(foodsourceSpeciesID);
        return CreateFoodSource(GameManager.Instance.FoodSources[id], position, ttb);
    }

    public void DestroyFoodSource(FoodSource foodSource) {
        foodSources.Remove(foodSource);
        foodSourcesBySpecies[foodSource.Species].Remove(foodSource);
        m_gridSystemReference.RemoveFoodReference(m_gridSystemReference.WorldToCell(foodSource.Position));
        Destroy(foodSource.gameObject);

        EventManager.Instance.InvokeEvent(EventType.FoodSourceChange, foodSource);
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

    /// <summary>
    /// Get a list of Food Source with the given species name.
    /// </summary>
    /// <param name="speciesName">Same as FoodSourceSpecies.SpeciesName</param>
    /// <returns>An list of Food Source with the given species name</returns>
    public List<FoodSource> GetFoodSourcesWithSpecies(string speciesName) {
        ItemID id = ItemRegistry.FindHasName(speciesName);
        // Given species doesn't exist in the level
        if (!GameManager.Instance.FoodSources.ContainsKey(id))
        {
            Debug.Log("Food source not in level data");
            return null;
        } 
        FoodSourceSpecies species = GameManager.Instance.FoodSources[id];

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
        foreach (ItemID speciesID in GameManager.Instance.FoodSources.Keys)
        {
            string speciesName = speciesID.Data.Name.Get(ItemName.Type.Serialized);
            serializedMapObjects.AddType(this.MapObjectName, new GridItemSet(speciesName, this.GetFoodSourcesWorldLocationWithSpecies(speciesName)));
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
}