using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class MapDesigningTool : MonoBehaviour
{
    // Start is called before the first frame update
    private TileType selectedTile;
    private Vector2 tileScrollPosition;
    private string sceneName;
    private LevelIO levelIO;
    private TilePlacementController tilePlacementController;
    [SerializeField] bool godMode = true;
    private bool DisplayLiquidBodyInfo = true;
    private bool DisplayPreviewBodies;
    private Tilemap[] tilemaps;
    private TileSystem tileSystem;
    private Vector2 scrollPosition1;
    private Vector2 scrollPosition2;
    private Camera mainCamera;
    private Dictionary<TileLayerManager, Dictionary<LiquidBody, bool>> ManagersToToggles = new Dictionary<TileLayerManager, Dictionary<LiquidBody, bool>>();
    private void Awake()
    {
        this.levelIO = FindObjectOfType<LevelIO>();
        this.tilePlacementController = FindObjectOfType<TilePlacementController>();
        this.mainCamera = this.gameObject.GetComponent<Camera>();
        this.tilemaps = FindObjectsOfType<Tilemap>();
        this.tileSystem = FindObjectOfType<TileSystem>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.tilePlacementController.StartPreview(this.selectedTile.ToString(), this.godMode);
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.tilePlacementController.StopPreview();
        }
        if (Input.GetMouseButtonDown(1))
        {
            this.tilePlacementController.RevertChanges();
        }
    }
    private void OnGUI()
    {

        GUILayout.BeginArea(new Rect(0, 0, 200, 600));
        TileSelectionBlock();
        MapIOBlock();
        GUILayout.EndArea();
        MouseHUD();
        LiquidBlock();
    }
    private void TileSelectionBlock()
    {
        GUILayout.BeginVertical();
        GUILayout.Box("Select with RMB");
        this.godMode = GUILayout.Toggle(this.godMode, "God Mode");
        this.tileScrollPosition = GUILayout.BeginScrollView(this.tileScrollPosition, GUILayout.Width(200), GUILayout.Height(200));
        foreach (TileType tileType in (TileType[])Enum.GetValues(typeof(TileType)))
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(tileType.ToString()))
            {
                this.selectedTile = tileType;
            }
            if (this.selectedTile == tileType)
            {
                GUILayout.Box("Selected");
            }
            GUILayout.EndHorizontal();
        }
        this.tilePlacementController.isErasing = GUILayout.Toggle(this.tilePlacementController.isErasing, "Eraser Mode");
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
    private void MapIOBlock()
    {
        GUILayout.BeginVertical();
        this.sceneName = GUILayout.TextField(this.sceneName);
        if (GUILayout.Button("Save") && !this.sceneName.Equals("") && this.sceneName != null)
        {
            levelIO.Save(this.sceneName);
        }
        if (GUILayout.Button("Load"))
        {
            levelIO.Load(this.sceneName);
        }
        GUILayout.EndVertical();
    }
    private void MouseHUD()
    {
        GUILayout.BeginArea(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y -150 , 200, 150));
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        GUILayout.Box("World Pos: " + mousePos.ToString());
        Vector3Int cellPosition = this.tileSystem.WorldToCell(mousePos);
        GUILayout.Box("Cell Pos: " + cellPosition);
        GameTile gameTile = this.tileSystem.GetGameTileAt(cellPosition);
        string name = gameTile ? gameTile.name : "Null";
        LiquidBody liquid = this.tileSystem.GetLiquidBodyAt(cellPosition);
        string bodyID = "Null";
        string con = "Null";
        if (liquid != null)
        {
            bodyID = liquid.bodyID == 0 ? "Preview Body" : liquid.bodyID.ToString();
            con = string.Join(", ", liquid.contents);
        }
        GUILayout.Box("Tile: " + name);
        GUILayout.Box("Liquid Body: " + bodyID);
        GUILayout.Box("Contents: " + con);
        GUILayout.EndArea();
    }
    private void LiquidBlock()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, 600));
        GUILayout.BeginVertical();
        GUILayout.Box("Liquid Developer Debugger");
        this.DisplayLiquidBodyInfo = GUILayout.Toggle(this.DisplayLiquidBodyInfo, "Liquid Body Info Display");
        if (this.DisplayLiquidBodyInfo)
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                if (tilemap.TryGetComponent(out TileLayerManager tileLayerManager))
                {
                    if (!tileLayerManager.holdsContent)
                    {
                        continue;
                    }
                    if (!this.ManagersToToggles.ContainsKey(tileLayerManager))
                    {
                        this.ManagersToToggles.Add(tileLayerManager, new Dictionary<LiquidBody, bool>());
                    }
                    GUILayout.Box("Tilemap: " + tilemap.name);
                    GUILayout.Box("Active Liquid Bodies: " + tileLayerManager.liquidBodies.Count.ToString());
                    this.scrollPosition1 = GUILayout.BeginScrollView(scrollPosition1, GUILayout.Width(200), GUILayout.Height(210));
                    foreach (LiquidBody liquidBody in tileLayerManager.liquidBodies)
                    {
                        if (!this.ManagersToToggles[tileLayerManager].ContainsKey(liquidBody))
                        {
                            this.ManagersToToggles[tileLayerManager].Add(liquidBody, false);
                        }
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
                        bool validCrossReference = true;
                        foreach (Vector3Int tile in liquidBody.tiles)
                        {
                            if (tileLayerManager.positionsToTileData[tile].currentLiquidBody != liquidBody)
                            {
                                validCrossReference = false;
                                break;
                            }
                        }
                        GUILayout.Box("Referenced Bodies: " + liquidBody.referencedBodies.Count);
                        GUILayout.Box("Valid Cross Reference: " + validCrossReference.ToString());
                        ManagersToToggles[tileLayerManager][liquidBody] = GUILayout.Toggle(ManagersToToggles[tileLayerManager][liquidBody], "View Area");
                        if (ManagersToToggles[tileLayerManager][liquidBody])
                        {
                            // View Area
                        }
                    }
                    GUILayout.EndScrollView();
                    int bodyCount = 0;
                    this.scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, GUILayout.Width(200), GUILayout.Height(210));
                    GUILayout.Box("Active Preview Bodies: " + tileLayerManager.previewBodies.Count.ToString());
                    this.DisplayPreviewBodies = GUILayout.Toggle(this.DisplayPreviewBodies, "Display Preview Bodies");
                    foreach (LiquidBody liquidBody in tileLayerManager.previewBodies)
                    {
                        bodyCount++;
                        GUILayout.Box("Body No. " + bodyCount);
                        GUILayout.Box("Composition");
                        for (int i = 0; i < liquidBody.contents.Length; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Box(liquidBody.contents[i].ToString("n3"));
                            liquidBody.contents[i] = GUILayout.HorizontalSlider(liquidBody.contents[i], 0.0f, 1.0f);
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.Box("Tile Count: " + liquidBody.tiles.Count);
                        bool validCrossReference = true;
                        foreach (Vector3Int tile in liquidBody.tiles)
                        {
                            if (tileLayerManager.positionsToTileData[tile].currentLiquidBody != liquidBody)
                            {
                                validCrossReference = false;
                                break;
                            }
                        }
                        GUILayout.Box("Valid Cross Reference" + validCrossReference.ToString());
                        if (this.DisplayPreviewBodies)
                        {
                            // View Area
                        }
                    }
                    GUILayout.EndScrollView();
                }
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
