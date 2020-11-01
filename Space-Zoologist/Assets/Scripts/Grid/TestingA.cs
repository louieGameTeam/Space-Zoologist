using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingA : MonoBehaviour
{
    private TileSystem TileSystem;
    private GameTile tile1;
    [SerializeField] private Camera currentCamera;
    private Vector3Int currentMouseCellPosition;
    private void Start()
    {
        TileSystem = FindObjectOfType<TileSystem>();
    }
    private void Update()
    {
        Vector3 mouseWorldPosition = currentCamera.ScreenToWorldPoint(Input.mousePosition);
        currentMouseCellPosition = TileSystem.WorldToCell(mouseWorldPosition);
        if (Input.GetKeyDown(KeyCode.V))
        {
            tile1 = this.TileSystem.GetTerrainTileAtLocation(currentMouseCellPosition);
        }
        if (tile1 != null)
        {
            Debug.Log(ReferenceEquals(tile1, this.TileSystem.GetTerrainTileAtLocation(currentMouseCellPosition)));
        }
    }
}
