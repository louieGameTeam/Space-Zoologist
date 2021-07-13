using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidUpdateTester : MonoBehaviour
{
    private GridSystem gridSystem;
    private Camera mainCamera;
    [SerializeField] float[] comp = { 1, 1, 1 };
    // Start is called before the first frame update
    private void Awake()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        mainCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int cellPosition = gridSystem.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        //Vector3Int cellLocation2 = new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z);
        if (Input.GetKeyDown(KeyCode.C))
        {
            gridSystem.ChangeComposition(cellPosition, comp);
            // tileSystem.ChangeLiquidBodyComposition(cellLocation, comp, true);
            //comp2 = tileSystem.GetTileContentsAtLocation(cellLocation, tileSystem.GetTerrainTileAtLocation(cellLocation));
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log(cellPosition.ToString());
            LiquidBody liquidBody = gridSystem.GetLiquidBodyAt(cellPosition);
            Debug.Log("Liquid body: " + liquidBody.bodyID + " Contents: " + liquidBody.contents[0] + "," + liquidBody.contents[1] + "," + liquidBody.contents[2] + " Count: " + liquidBody.tiles.Count + " Body also contains pos: " + liquidBody.tiles.Contains(cellPosition));
        }
    }
}
