using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FoodSource : MonoBehaviour
{
    // using enum to create a dropdown list
    public enum FoodTypes { SpaceMaple, Food2, Food3, Food4, Food5 };
    [SerializeField] private FoodTypes type = default;
    public FoodTypes Type { get => type; }

    // ScriptableObject to read from
    [SerializeField] private FoodScriptableObject species = default;
    public FoodScriptableObject Species { get => species; }

    // For debugging, might be removed later
    // How much of each need is provided, raw value of needs
    [SerializeField] private float[] rawValues;
    public float[] RawValues { get => rawValues; }

    // How well each need is provided
    [SerializeField] private NeedCondition[] conditions;
    public NeedCondition[] Conditions { get => conditions; }

    [SerializeField] private float totalOutput;
    public float TotalOutput { get => totalOutput; }

    public WorldAtmosphere atm;

    // Start is called before the first frame update
    void Awake()
    {
        if (species == null)
        {
            throw new System.NullReferenceException("Error: species is not set");
        }

        int numNeeds = species.Needs.Length;
        rawValues = new float[numNeeds];
        conditions = new NeedCondition[numNeeds];
        totalOutput = 0;

    }

    /// <summary>
    /// Detects what is in the environment and populate rawValues[].
    /// </summary>
    public void DetectEnvironment()
    {
        NeedScriptableObject[] needs = species.Needs;
        float[] weights = species.Severities;
        PlantNeedType[] types = species.Types;
        TileSystem tileSystem = FindObjectOfType<TileSystem>();

        // TODO Implement liquid
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] > 0)
            { // Lazy evaluation, only detect if it matters
                // Determine need values
                switch (types[i])
                {
                    case PlantNeedType.Terrain:
                        // get tiles around the food source and return as an array of integers
                        List<TerrainTile> terrainTiles = FoodUtils.GetAllTilesWithinRadius(transform.position, species.Radius);

                        // quick check for no tiles read
                        if (terrainTiles.Count == 0) { rawValues[i] = 0; break; }

                        List<TileType> tiles = new List<TileType>();
                        foreach (TerrainTile tile in terrainTiles)
                        {
                            tiles.Add(tile.type); // TileType tile.type is defined in TerrainNeedScriptableObject
                        }

                        // imported from TerrainNeedScriptableObject
                        float total_value = 0;
                        for (int ind = 0; ind < tiles.Count; ind++)
                        {
                            try
                            {
                                total_value += species.TileDic[tiles[ind]];
                            }
                            catch (KeyNotFoundException)
                            {
                                // tiles is not contained in tileValue
                                Debug.LogError("Tile not found in TileDic.");
                            }
                        }

                        // maybe consider swapping tiles.length with (1+2*radius+2*radius^2) i.e. 1, 5, 13, 25, ..., i.e. the max number of tiles the plant can reach
                        // the latter suggests that less space might mean worse terrain for plant (as its roots have to be more crammed and get less resource overall)
                        rawValues[i] = total_value / (1 + 2 * species.Radius + 2 * species.Radius * species.Radius);
                        break;

                    // NullReferenceError if no atmosphere here (plant shouldn't exist here in the first place)
                    case PlantNeedType.GasX:
                        // Read value from some class that handles atmosphere
                        rawValues[i] = EnclosureSystem.ins.GetAtmosphericComposition(tileSystem.WorldToCell(transform.position)).GasX;
                        break;
                    case PlantNeedType.GasY:
                        // Read value from some class that handles atmosphere
                        rawValues[i] = EnclosureSystem.ins.GetAtmosphericComposition(tileSystem.WorldToCell(transform.position)).GasY;
                        break;
                    case PlantNeedType.GasZ:
                        // Read value from some class that handles atmosphere
                        rawValues[i] = EnclosureSystem.ins.GetAtmosphericComposition(tileSystem.WorldToCell(transform.position)).GasZ;
                        break;
                    case PlantNeedType.Temperature:
                        // Read value from some class that handles temperature
                        rawValues[i] = EnclosureSystem.ins.GetAtmosphericComposition(tileSystem.WorldToCell(transform.position)).Temperature;
                        break;
                    case PlantNeedType.RLiquid:
                    case PlantNeedType.YLiquid:
                    case PlantNeedType.BLiquid:
                        // Assuming that RYB is consecutive, this shouldn't change. 0 = R, 1 = Y, 2 = B
                        int color = types[i] - PlantNeedType.RLiquid;

                        // get liquid tiles around the food source and return as an array of tiles
                        Dictionary<float[], float>.KeyCollection liquids = tileSystem.DistancesToClosestTilesOfEachBody(tileSystem.WorldToCell(transform.position), ReservePartitionManager.ins.Liquid, species.Radius).Keys;
                        conditions[i] = NeedCondition.Bad;

                        // liquid_body is a body of liquid of size 3 that contains [R, Y, B] values
                        foreach(float[] liquid_body in liquids) {
                            NeedCondition temp = needs[i].GetCondition(liquid_body[color]);  
                            if (temp > conditions[i]) {
                                conditions[i] = temp;
                                rawValues[i] = liquid_body[color];
                            }
                        }
                        
                        continue; // already calculated condition
                    default:
                        Debug.LogError("Error: No need name matches.");
                        rawValues[i] = 0;
                        break;
                }
                conditions[i] = needs[i].GetCondition(rawValues[i]);
            }
        }

        // calculate output based on conditions
        totalOutput = FoodUtils.CalculateOutput(species, conditions);
    }
}
