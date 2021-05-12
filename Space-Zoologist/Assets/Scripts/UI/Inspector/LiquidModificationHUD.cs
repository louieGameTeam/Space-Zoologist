using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidModificationHUD : MonoBehaviour
{
    private TileSystem tileSystem;
    private Camera mainCamera;
    private bool isOpened = false;
    private Vector3 worldPos;
    private LiquidBody liquidBody;

    void Start()
    {
        this.tileSystem = FindObjectOfType<TileSystem>();
        this.mainCamera = FindObjectOfType<Camera>();
    }
    void OnGUI()
    {
        if (this.isOpened)
        {
            Vector3 screenPoint = this.mainCamera.WorldToScreenPoint(worldPos);
            GUILayout.BeginArea(new Rect(screenPoint.x, Screen.height - screenPoint.y - 150, 200, 150));
            GUILayout.Box("LiquidBodyID: " + liquidBody.bodyID);
            GUILayout.Box("Composition");
            for (int i = 0; i < liquidBody.contents.Length; i++)
            {
                GUILayout.BeginHorizontal();
                try
                {
                    liquidBody.contents[i] = float.Parse(GUILayout.TextField(liquidBody.contents[i].ToString("n3")));
                }
                catch { }
                liquidBody.contents[i] = GUILayout.HorizontalSlider(liquidBody.contents[i], 0.0f, 1.0f);
                GUILayout.EndHorizontal();
            }
            GUILayout.Box("Tile Count: " + liquidBody.tiles.Count);
            GUILayout.EndArea();
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(2)) //If clicking MMB on liquid tile, open HUD
        {
            Vector3 mousePos = this.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = this.tileSystem.WorldToCell(mousePos);
            LiquidBody liquid = this.tileSystem.GetLiquidBodyAt(cellPosition);
            if (liquid != null)
            {
                this.worldPos = new Vector3(mousePos.x, mousePos.y, mousePos.z);
                this.isOpened = true;
                this.liquidBody = this.tileSystem.GetLiquidBodyAt(cellPosition);
            }
            else //If not clicked on a liquid tile, close HUD
            {
                this.isOpened = false;
            }
        }
        if (Input.GetMouseButtonDown(1)) //RMB to close
        {
            this.isOpened = false;
        }
    }
}