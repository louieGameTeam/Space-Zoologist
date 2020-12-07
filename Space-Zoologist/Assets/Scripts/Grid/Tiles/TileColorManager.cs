using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColorManager : MonoBehaviour
{
    protected Tilemap tilemap;
    [SerializeField] public ColoringMethod coloringMethod;
    public List<GameTile> managedTiles { get { return managedTerrainTiles; } }
    [SerializeField] protected List<GameTile> managedTerrainTiles = new List<GameTile>();
    public List<GameTile> linkedTiles { get { return linkedTerrainTiles; } }
    [SerializeField] protected List<GameTile> linkedTerrainTiles = new List<GameTile>();
    private TileSystem tileSystem;
    private TilePlacementController tilePlacementController;
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tileSystem = GetComponentInParent<TileSystem>();
        tilePlacementController = GetComponentInParent<TilePlacementController>();
    }
    public void SetTileColor(Vector3Int cellLocation, GameTile tile)
    {
        float[] composition = tileSystem.GetTileContentsAt(cellLocation, tile);
        coloringMethod.SetColor(composition, cellLocation, tile, tilemap, managedTerrainTiles, linkedTerrainTiles, tileSystem, tilePlacementController);
    }
}
