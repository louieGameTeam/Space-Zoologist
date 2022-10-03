using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDesignHighlighter : MonoBehaviour
{
    [Header("Highlight Colors")]
    [SerializeField] private Color unPlaceableHighlight;

    private TileDataController tileDataController;
    private bool highlightingPlaceables = false;
    private void Start()
    {
        tileDataController = GameManager.Instance.m_tileDataController;
    }

    private void Update()
    {
        if(highlightingPlaceables && Input.GetMouseButtonDown(2))
        {
            HighlightUnplaceables();
        }
    }

    public void ToggleShowPlaceableHighlight(bool val)
    {
        highlightingPlaceables = val;
        if(!val)
            ClearHighlights();
        else
            HighlightUnplaceables();
    }

    public void ClearHighlights()
    {
        tileDataController.ClearHighlights();
    }

    public void HighlightUnplaceables()
    {
        var toHighlight = new List<Vector3Int>();
        for(int i = 0;i < tileDataController.GetReserveDimensions().x;i++)
        {
            for (int j = 0; j < tileDataController.GetReserveDimensions().y; j++)
            {
                var tileData = tileDataController.GetTileData(new Vector3Int(i,j));
                if(tileData != null && !tileData.isTilePlaceable)
                {
                    toHighlight.Add(new Vector3Int(i,j));
                }
            }
        }
        tileDataController.HighlightTiles(toHighlight, unPlaceableHighlight);
    }
}
