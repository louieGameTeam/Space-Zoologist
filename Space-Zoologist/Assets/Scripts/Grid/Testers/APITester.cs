using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APITester : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Camera = default;
    private GetTerrainTile getTerrainTile;
    private Grid grid;
    void Awake()
    {
        getTerrainTile = GetComponent<GetTerrainTile>();
        grid = GetComponent<Grid>();
        Camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentMouseCellPosition = grid.WorldToCell(mouseWorldPosition);
        TerrainTile tile = getTerrainTile.GetTerrainTileAtLocation(currentMouseCellPosition);
/*        Debug.Log(getTerrainTile.GetTerrainTileAtLocation(currentMouseCellPosition));
        Debug.Log(getTerrainTile.GetTileContentsAtLocation(currentMouseCellPosition, tile));*/
    }
}
