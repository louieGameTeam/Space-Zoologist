using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AtmosphereTester : MonoBehaviour
{
    public Tilemap mask;
    public void Graph()
    {
        List<AtmosphericComposition> Atmospheres = EnclosureSystem.ins.Atmospheres;
        for ( int i = 0; i < Atmospheres.Count; i++) {
            print(i+1 + "th atmosphere: " + Atmospheres[i]);
        }
        Color[] colors = new Color[Atmospheres.Count];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Atmospheres[i].GasX, Atmospheres[i].GasY, Atmospheres[i].GasZ);
        }

        //find max density and calculate density for each tile
        foreach (KeyValuePair<Vector3Int, byte> pair in EnclosureSystem.ins.PositionToAtmosphere)
        {
            //skip walls
            if (pair.Value == 255) {
                mask.SetTileFlags(pair.Key, TileFlags.None);
                mask.SetColor(pair.Key, Color.black);
                continue;
            }

            //By default the flag is TileFlags.LockColor
            mask.SetTileFlags(pair.Key, TileFlags.None);

            //set color of tile, close to maxDensity = red, close to 0 = green, in the middle = orange
            mask.SetColor(pair.Key, colors[pair.Value]);
        }
    }
}
