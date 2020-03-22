using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassColorManager : MonoBehaviour
{
    public float[] gasComposition = new float[] { 0.5f, 0.2f, 0.3f };
    private Tilemap tilemap;
    private TileSystem getTerrainTile;
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        getTerrainTile = FindObjectOfType<TileSystem>();
    }
    private void Update()
    {
        foreach (Vector3Int cellLocation in tilemap.cellBounds.allPositionsWithin)
        {

        }
    }
}
