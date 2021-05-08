using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilePreviewTester : MonoBehaviour
{
    private TilePlacementController tilePlacementController;
    [SerializeField] private List<TileType> selectableTiles = default;
    [SerializeField] bool isBlockMode = default;
    private TileType selectedTile = default;

    
    void Awake()
    {
        tilePlacementController = GetComponent<TilePlacementController>();
        selectedTile = selectableTiles[0];
        tilePlacementController.isBlockMode = isBlockMode;
    }

    private void Start()
    {
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
            tilePlacementController.StartPreview(selectedTile.ToString());
        }
        if (Input.GetMouseButtonUp(0))
        {
            tilePlacementController.StopPreview();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            tilePlacementController.RevertChanges();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            tilePlacementController.isBlockMode = !tilePlacementController.isBlockMode;
        }
        //Debug.Log(tilePlacementController.PlacedTileCount());
    }
}
