using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodUtils : MonoBehaviour
{

    /// <summary>
    /// Calculate the output of a food source given its species and conditions
    /// </summary>
    public static float CalculateOutput(FoodScriptableObject fso, NeedCondition[] conditions){
        int[] intConditions = new int[conditions.Length];
        //convert to int, 0 = bad, 1 = neutral, 2 = good
        for (int i = 0; i < conditions.Length; i++) intConditions[i] = (int)conditions[i];
        return CalculateOutput(fso.BaseOutput, fso.Severities, fso.TotalSeverity, intConditions);
    }

    /// <summary>
    /// Calculate the output of a food source given its base output, weights, total weight, and conditions
    /// </summary>
    //values = raw values, tWeight = total weight
    public static float CalculateOutput(float base_output, float[] weights, float tWeight, int[] conditions){
    	float total_weight = tWeight; //just to clarify

        //Calculate total output once we have weights, ranges, values, and conditions
        float total_output = 0;
        for(int i = 0; i < weights.Length; i++){
            //total output of each need is weight of the need/total weight * condition (bad = 0, med = 1, good = 2) * base output of the plant
            total_output += conditions[i] * weights[i]/total_weight * base_output;
        }
        return total_output;
    }

    /// <summary>
    /// Get TerrainTiles at world_pos with a radius of radius.
    /// </summary>
    public static List<TerrainTile> GetTiles(Vector3 world_pos, int radius)
    {
        GetTerrainTile api = FindObjectOfType<GetTerrainTile>();

        //list of tiles to return
        List<TerrainTile> tiles = new List<TerrainTile>();

        //position of object in terms of tilemap
        Vector3Int cell_pos = ReservePartitionManager.ins.WorldToCell(world_pos);

        //prototype nested loop -- could be a little more efficient
        for (int r = cell_pos.y - radius; r <= cell_pos.y + radius; r++)
        {
            for (int c = cell_pos.x - radius; c <= cell_pos.x + radius; c++)
            {
                //if in terms of abs distance
                //float dist = Mathf.Sqrt(Mathf.Pow(r-cell_pos.y,2) + Mathf.Pow(c-cell_pos.x,2));

                //tile-based distance
                int dist = Mathf.Abs(r - cell_pos.y) + Mathf.Abs(c - cell_pos.x);

                //tile is within range: get it
                if (dist <= radius)
                {
                    Vector3Int pos = new Vector3Int(c, r, 0);
                    //if there's a tile -- condition may be changed later, such as if the tile is a certain type
                    TerrainTile tile = api.GetTerrainTileAtLocation(pos);
                    if (tile != null)
                    {
                        tiles.Add(tile);//Get the tile
                    }
                }
            }

        }
        return tiles;
    }


}