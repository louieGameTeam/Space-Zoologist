using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilePreviewTester : MonoBehaviour
{
    private TilePlacementController tilePlacementController;
    [SerializeField] private List<TerrainTile> selectableTiles = default;
    [SerializeField] bool isBlockMode = default;
    private TerrainTile selectedTile = default;

    [TextArea]
    [SerializeField] string displayText = default;

    [SerializeField] private Canvas canvas = default;
    
    void Awake()
    {
        tilePlacementController = GetComponent<TilePlacementController>();
        selectedTile = selectableTiles[0];
        tilePlacementController.isBlockMode = isBlockMode;
    }

    private void Start()
    {
        var text = canvas.gameObject.AddComponent<Text>();
        text.text = displayText;

        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.material = ArialFont.material;

        tilePlacementController.isBlockMode = isBlockMode;
    }

    void Update()
    {
        for (var i = 49; i <= 57; i++)
        {
            if (Input.GetKeyUp((KeyCode)i))
            {
                selectedTile = selectableTiles[i - 49];
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            tilePlacementController.StartPreview(selectedTile);
        }
        if (Input.GetMouseButtonUp(0))
        {
            tilePlacementController.StopPreview();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            tilePlacementController.RevertChanges();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tilePlacementController.ConfirmPlacement();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            tilePlacementController.isBlockMode = !tilePlacementController.isBlockMode;
        }
    }
}
