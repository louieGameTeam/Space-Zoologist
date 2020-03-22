using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColorManager : MonoBehaviour
{
    private float[,] interpolationArray = null;
    private Tilemap tilemap;
    private TileSystem tileSystem;
    public int affectedRange;
    public List<TerrainTile> managedTiles = new List<TerrainTile>();
    public List<TerrainTile> linkedTiles = new List<TerrainTile>();
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tileSystem = FindObjectOfType<TileSystem>();
    }
    public virtual void SetTileColor(float[] composition, Vector3Int cellLocation, TerrainTile tile)
    {
        tilemap.SetTileFlags(cellLocation, TileFlags.None);
        Color color = RYBConverter.ToRYBColor(composition, interpolationArray);
        tilemap.SetColor(cellLocation, color);
    }
}
