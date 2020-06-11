using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AtmosphereTester : MonoBehaviour
{
    public Tilemap mask;

    public void Start()
    {
        GetComponent<EnclosureSystem>().FindEnclosedAreas();
        InvokeRepeating("UpdateAtmosphere", 1.0f, 1.0f);
    }

    private void UpdateAtmosphere()
    {
        GetComponent<EnclosureSystem>().FindEnclosedAreas();
        Graph();
    }

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

        // find max density and calculate density for each tile
        foreach (KeyValuePair<Vector3Int, byte> pair in EnclosureSystem.ins.PositionToAtmosphere)
        {

            // By default the flag is TileFlags.LockColor
            mask.SetTileFlags(pair.Key, TileFlags.None);

            // skip walls
            if (pair.Value == 255)
            {
                mask.SetColor(pair.Key, Color.black);
            }
            else
            {
                // set color of tile, based on gasXYZ
                mask.SetColor(pair.Key, colors[pair.Value]);
            }
        }
    }
}
