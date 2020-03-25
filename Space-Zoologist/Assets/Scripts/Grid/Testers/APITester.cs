using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APITester : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Camera = default;
    private TileSystem getTerrainTile;
    private Grid grid;
    public TerrainTile liquid;
    void Awake()
    {
        getTerrainTile = GetComponent<TileSystem>();
        grid = GetComponent<Grid>();
        Camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentMouseCellPosition = grid.WorldToCell(mouseWorldPosition);
        //Debug.Log(currentMouseCellPosition);
        TerrainTile tile = getTerrainTile.GetTerrainTileAtLocation(currentMouseCellPosition);
        //Debug.Log(getTerrainTile.GetTerrainTileAtLocation(currentMouseCellPosition));
          float[] comp = getTerrainTile.GetTileContentsAtLocation(currentMouseCellPosition, tile);
        //Debug.Log(getTerrainTile.CellLocationsOfClosestTiles(currentMouseCellPosition, liquid));
        //Debug.Log(getTerrainTile.DistanceToClosestTile(currentMouseCellPosition, liquid, 10));
    }
}
