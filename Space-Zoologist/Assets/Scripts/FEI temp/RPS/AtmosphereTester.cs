using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AtmosphereTester : MonoBehaviour
{
    public Tilemap mask;
    public void Graph()
    {
        Color[] colors = new Color[ReservePartitionManager.ins.Atmospheres.Count];
        for(int i = 0; i < colors.Length; i++) {
            colors[i] = new Color(Random.value*0.5f+0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f);
            print(i + ": " + colors[i]);
        }

        //find max density and calculate density for each tile
        foreach (KeyValuePair<Vector3Int, byte> pair in ReservePartitionManager.ins.PositionToAtmosphere)
        {
            //skip walls
            if(pair.Value == 255) continue;

            //By default the flag is TileFlags.LockColor
            mask.SetTileFlags(pair.Key, TileFlags.None);

            //set color of tile, close to maxDensity = red, close to 0 = green, in the middle = orange
            mask.SetColor(pair.Key, colors[pair.Value]);
        }


        //debug
        print(colors.Length);
    }
}
