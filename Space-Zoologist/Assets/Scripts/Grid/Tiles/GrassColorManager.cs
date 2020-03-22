using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassColorManager : TileColorManager
{
    public float[] gasComposition = new float[] { 0.5f, 0.2f, 0.3f };
    [SerializeField] TerrainTile liquid;
    [SerializeField] TerrainTile dirt;
    [SerializeField] TerrainTile sand;
    private float[] colorShitfDirt = new float[] { 0, 0, 0 };
    private float[] colorShitfSand = new float[] { 0, -0.2f, -0.1f };
    private Tilemap tilemap;
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
}
