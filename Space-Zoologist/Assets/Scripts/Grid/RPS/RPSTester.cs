using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RPSTester : MonoBehaviour
{
    [SerializeField] Species species;
    [SerializeField] Vector2Int spawnLocation;
    [SerializeField] GameObject PopulationPrefab;
    Population testee;
    [SerializeField] Tilemap mask;

    // Start is called before the first frame update
    void Start()
    {
        GameObject animal = Instantiate(PopulationPrefab);
        testee = animal.GetComponent<Population>();
        testee.Initialize(species, spawnLocation, null);
        GetComponent<ReservePartitionManager>().AddPopulation(testee);
        Graph();
    }

    public void Graph()
    {
        List<Vector3Int> list = GetComponent<ReservePartitionManager>().GetLocationWithAccess(testee);

        //set color based on the fraction density/maxdensity
        foreach (Vector3Int pos in list)
        {
            //By default the flag is TileFlags.LockColor
            mask.SetTileFlags(pos, TileFlags.None);

            //set color of tile, close to maxDensity = red, close to 0 = green, in the middle = orange
            mask.SetColor(pos, new Color(0, 1, 0, 255.0f / 255));
        }

        //debug
        print(list.Count);
    }
}
