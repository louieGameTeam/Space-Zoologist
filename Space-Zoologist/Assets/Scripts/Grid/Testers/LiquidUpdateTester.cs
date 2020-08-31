using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidUpdateTester : MonoBehaviour
{
    private TileSystem tileSystem;
    private Camera mainCamera;
    private TileContentsManager tileContentsManager;
    [SerializeField] float[] comp = { 1, 1, 1 };
    private float[] comp2;
    // Start is called before the first frame update
    private void Awake()
    {
        tileSystem = FindObjectOfType<TileSystem>();
        mainCamera = FindObjectOfType<Camera>();
        tileContentsManager = FindObjectOfType<TileContentsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int cellLocation = tileSystem.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        //Vector3Int cellLocation2 = new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z);
        if (Input.GetKeyDown(KeyCode.C))
        {

            tileSystem.ChangeLiquidBodyComposition(cellLocation, comp, true);
            //comp2 = tileSystem.GetTileContentsAtLocation(cellLocation, tileSystem.GetTerrainTileAtLocation(cellLocation));
        }
        //Debug.Log(("reference",ReferenceEquals(tileSystem.GetTileContentsAtLocation(cellLocation, tileSystem.GetTerrainTileAtLocation(cellLocation)), tileSystem.GetTileContentsAtLocation(cellLocation2, tileSystem.GetTerrainTileAtLocation(cellLocation2)))));
        //Debug.Log(("value", Equals(tileSystem.GetTileContentsAtLocation(cellLocation, tileSystem.GetTerrainTileAtLocation(cellLocation))[0], tileSystem.GetTileContentsAtLocation(cellLocation2, tileSystem.GetTerrainTileAtLocation(cellLocation2))[0])));
        //Debug.Log(ReferenceEquals(comp, tileSystem.GetTileContentsAtLocation(cellLocation, tileSystem.GetTerrainTileAtLocation(cellLocation))));
        Debug.Log(tileSystem.GetTileContentsAtLocation(cellLocation, tileSystem.GetTerrainTileAtLocation(cellLocation))[0].ToString());
    }
}
