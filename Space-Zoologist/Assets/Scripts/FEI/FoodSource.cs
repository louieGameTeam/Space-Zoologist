using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FoodSource : MonoBehaviour
{
    //using enum to create a dropdown list
    public enum FoodTypes { SpaceMaple, Food2, Food3, Food4, Food5 };
    [SerializeField] private FoodTypes type = default;
    public FoodTypes Type { get => type; }

    //ScriptableObject to read from
    [SerializeField] private FoodScriptableObject species = default;
    public FoodScriptableObject Species { get => species; }

    // For debugging, might be removed later
    //How much of each need is provided, raw value of needs
    [SerializeField] private float[] rawValues;
    public float[] RawValues { get => rawValues; }

    //How well each need is provided
    [SerializeField] private NeedCondition[] conditions;
    public NeedCondition[] Conditions { get => conditions; }

    [SerializeField] private float totalOutput;
    public float TotalOutput { get => totalOutput; }

    public WorldAtmosphere atm;

    // Start is called before the first frame update
    void Start()
    {
        int numNeeds = species.Needs.Length;
        rawValues = new float[numNeeds];
        conditions = new NeedCondition[numNeeds];
        totalOutput = 0;

        if (species == null) {
            throw new System.NullReferenceException("Error: species is not set");
        }

        DetectEnvironment();
    }

    /// <summary>
    /// Detects what is in the environment and populate rawValues[].
    /// </summary>
    public void DetectEnvironment()
    {
        NeedScriptableObject[] needs = species.Needs;
        float[] weights = species.Severities;
        PlantNeedType[] types = species.Types;

        //TODO Implement liquid
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] > 0)
            { //Lazy evaluation, only detect if it matters
                //Determine need values
                switch (types[i])
                {
                    case PlantNeedType.Terrain:
                        //get tiles around the food source and return as an array of integers
                        TerrainTile[] terrainTiles = FoodUtils.GetTiles(transform.position, species.Radius).ToArray();

                        //quick check for no tiles read
                        if (terrainTiles.Length == 0) { rawValues[i] = 0; break; }

                        List<TileType> tiles = new List<TileType>();
                        foreach (TerrainTile tile in terrainTiles)
                        {
                            tiles.Add(tile.type);//TileType tile.type is defined in TerrainNeedScriptableObject
                        }

                        //imported from TerrainNeedScriptableObject
                        float value = 0;
                        for (int ind = 0; ind < tiles.Count; ind++)
                        {
                            try
                            {
                                value += species.TileDic[tiles[ind]];
                            }
                            catch (KeyNotFoundException)
                            {
                                //tiles is not contained in tileValue
                                Debug.LogError("Tile not found in TileDic.");
                            }
                        }

                        //maybe consider swapping tiles.length for (1+2*radius+2*radius*radius) i.e. 1, 5, 13, 25, ...
                        //because less space might suggest worse terrain for plant (as its roots have to be more crammed and get less resource overall)
                        float avgValue = value / tiles.Count;
                        rawValues[i] = avgValue;
                        break;
                    case PlantNeedType.GasX:
                        //Read value from some class that handles atmosphere
                        rawValues[i] = atm.GasX;
                        break;
                    case PlantNeedType.GasY:
                        //Read value from some class that handles atmosphere
                        rawValues[i] = atm.GasY;
                        break;
                    case PlantNeedType.GasZ:
                        //Read value from some class that handles atmosphere
                        rawValues[i] = atm.GasZ;
                        break;
                    case PlantNeedType.Temperature:
                        //Read value from some class that handles temperature
                        rawValues[i] = atm.Temp;
                        break;
                    case PlantNeedType.RLiquid:
                        //TODO
                        //get liquid tiles around the food source and return as an array of tiles
                        float[,] RLiquid = new float[,] { { 1, 1, 0 }, { 0.5f, 0.5f, 0.5f }, { 0.2f, 0.8f, 0.4f } };

                        conditions[i] = NeedCondition.Bad;
                        for (int r = 0; r < RLiquid.GetLength(0); r++) {
                            NeedCondition temp = needs[i].GetCondition(RLiquid[r, 0]);
                            if (temp > conditions[i]) {
                                conditions[i] = temp;
                                rawValues[i] = RLiquid[r, 0];
                            }
                        }
                        continue; //already calculated condition
                    case PlantNeedType.YLiquid:
                        //TODO
                        //get liquid tiles around the food source and return as an array of tiles
                        float[,] YLiquid = new float[,] { { 1, 1, 0 }, { 0.5f, 0.5f, 0.5f }, { 0.2f, 0.8f, 0.4f } };

                        conditions[i] = NeedCondition.Bad;
                        for (int r = 0; r < YLiquid.GetLength(0); r++)
                        {
                            NeedCondition temp = needs[i].GetCondition(YLiquid[r, 1]);
                            if (temp > conditions[i])
                            {
                                conditions[i] = temp;
                                rawValues[i] = YLiquid[r, 0];
                            }
                        }
                        continue; //already calculated condition
                    case PlantNeedType.BLiquid:
                        //TODO
                        //get liquid tiles around the food source and return as an array of tiles
                        float[,] BLiquid = new float[,] { { 1, 1, 0 }, { 0.5f, 0.5f, 0.5f }, { 0.2f, 0.8f, 0.4f } };

                        conditions[i] = NeedCondition.Bad;
                        for (int r = 0; r < BLiquid.GetLength(0); r++)
                        {
                            NeedCondition temp = needs[i].GetCondition(BLiquid[r, 2]);
                            if (temp > conditions[i])
                            {
                                conditions[i] = temp;
                                rawValues[i] = BLiquid[r, 0];
                            }
                        }
                        continue; //already calculated condition
                    default:
                        Debug.LogError("Error: No need name matches.");
                        rawValues[i] = 0;
                        break;
                }
                conditions[i] = needs[i].GetCondition(rawValues[i]);
            }
        }

        //calculate output based on conditions
        totalOutput = FoodUtils.CalculateOutput(species, conditions);
    }
}
