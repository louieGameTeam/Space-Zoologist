using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum SelectionType {Tile, Animal, FoodSource, None }
public class MapDesignUIHandler : MonoBehaviour
{
    [SerializeField] private InputField fileNameInputField = null;
    [SerializeField] private TMPro.TextMeshProUGUI tileDataText = null;
    private TilePlacementController tilePlacementController;
    private TileDataController tileDataController;
    private string fileName;

    private void Start()
    {
        tilePlacementController = GameManager.Instance.m_tilePlacementController;
        tileDataController = GameManager.Instance.m_tileDataController;
    }

    private void Update()
    {
        DisplayTileData();
    }

    private void DisplayTileData()
    {
        Vector3Int gridPos = tileDataController.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        var tile = tileDataController.GetTileData(gridPos);
        tileDataText.text = 
            $"Map Size: {tileDataController.GetReserveDimensions()}\n" +
            $"({gridPos.x},{gridPos.y})\n";
        if (tile != null)
        {
            var gameTile = tile.currentTile;
            tileDataText.text += 
                $"{(gameTile != null ? tile.currentTile.type.ToString() : "GameTile Null")}\n" +
                $"Placeable: {tile.isTilePlaceable.ToString().ToUpper()}";
        }
        else
        {
            tileDataText.text += "No TileData";
        }
    }

    public void OnButtonSetFileName() { fileName = fileNameInputField.text; }
    public void OnButtonSaveLevel() { GameManager.Instance.SaveMap(fileName); }
    public void OnButtonLoadLevel() { GameManager.Instance.LoadMap(fileName); }
    public void OnErasingToggle(bool val) { tilePlacementController.isErasing = val; }
    public void OnChangingTilePlaceableToggle(bool val) { tilePlacementController.isTogglingPlaceable = val; }
    public void OnChangingTilePlaceableValue(bool val) { tilePlacementController.setPlaceableToValue = val; }
}
