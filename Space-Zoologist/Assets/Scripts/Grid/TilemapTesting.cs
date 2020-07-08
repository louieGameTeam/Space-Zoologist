using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTesting : MonoBehaviour
{
    [SerializeField] Tilemap tilemap = default;
    [SerializeField] TileSystem tileSystem = default;
    [SerializeField] GameObject tileLocation = default;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Tilemap size: " + tilemap.size);
        TerrainTile tile = tileSystem.GetTerrainTileAtLocation(tilemap.WorldToCell(tileLocation.transform.position));
        Debug.Log("Tile at location (" + tileLocation.transform.position.x+"),"+tileLocation.transform.position.y+"): " 
         + tile.type);
    }
}
