using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColoringMethod : MonoBehaviour
{
    private float[,] interpolationArray = null;
    protected TilePlacementController tilePlacementController;
    [SerializeField] public int affectedRange;
    private void Awake()
    {
        tilePlacementController = FindObjectOfType<TilePlacementController>();
    }
    public virtual void SetTileColor(float[] composition, Vector3Int cellLocation, TerrainTile tile, Tilemap tilemap, List<TerrainTile> managedTiles, List<TerrainTile> linkedTiles)
    {
        tilemap.SetTileFlags(cellLocation, TileFlags.None);
        Color color = RYBConverter.ToRYBColor(composition, interpolationArray);
        tilemap.SetColor(cellLocation, color);
    }
}
