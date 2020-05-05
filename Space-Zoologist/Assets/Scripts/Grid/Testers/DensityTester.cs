using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DensityTester : MonoBehaviour
{
    /// <summary>
    /// Mask for showing the demo.
    /// </summary>
    public Tilemap mask;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Graph", 0.3f);
    }


    /// <summary>
    /// Graphing for demo purposes, may be worked into the game as a sort of inspection mode?
    /// </summary>
    public void Graph()
    {
        //colors
        Dictionary<Vector3Int, float> col = new Dictionary<Vector3Int, float>();

        //max density for color comparison
        float maxDensity = -1;

        //find max density and calculate density for each tile
        foreach (KeyValuePair<Vector3Int, long> pair in PopDensityManager.ins.GetPopDensityMap())
        {
            //calculate density
            float density = PopDensityManager.ins.GetPopDensityAt(pair.Key);

            col.Add(pair.Key, density);

            if (density > maxDensity)
            {
                maxDensity = density;
            }
        }

        //set color based on the fraction density/maxdensity
        foreach (KeyValuePair<Vector3Int, float> pair in col)
        {
            //By default the flag is TileFlags.LockColor
            mask.SetTileFlags(pair.Key, TileFlags.None);

            //set color of tile, close to maxDensity = red, close to 0 = green, in the middle = orange
            mask.SetColor(pair.Key, new Color(pair.Value / maxDensity, 1 - pair.Value / maxDensity, 0, 255.0f / 255));
        }

        //debug
        print(maxDensity);
    }
}
