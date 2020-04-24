using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Tester : MonoBehaviour
{
    public FoodSource info;
    public Dictionary<string, int> counts;
    public Tilemap from;
    public Text text;
    public Dictionary<string, string> nameSwap;
    

    // Start is called before the first frame update
    void Start()
    {
        // detects the environment every 0.2 second
        InvokeRepeating("UpdateDebugMenu", 0, 0.2f);
    }

   /// <summary>
   /// Updates debug menu
   /// </summary>
    public void UpdateDebugMenu()
    {
        info.DetectEnvironment();
        List<TerrainTile> ts = FoodUtils.GetTiles(transform.position, info.Species.Radius);
        counts = new Dictionary<string, int>();
        for (int i = 0; i < ts.Count; i++)
        {
            if (counts.ContainsKey(ts[i].name)) {
                counts[ts[i].name] += 1;
            }
            else
            {
                counts.Add(ts[i].name, 1);
            }
        }
        string end = "";
        end += $"Total Output: {info.TotalOutput}\n";
        end += $"Position on Tilemap: {ReservePartitionManager.ins.WorldToCell(transform.position)}\n";
        end += $"Terrain raw value: {info.RawValues[0]}, {info.Conditions[0]}\n";
        foreach (KeyValuePair<string,int> kp in counts){
            end += $"{kp.Key}: {kp.Value}\n";
        }
        text.text = end;
    }
}
