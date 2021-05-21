using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridOverlay : MonoBehaviour
{
    [SerializeField] Tilemap gridOverlay = default;
    bool isOn = false;
    Dictionary<Vector3Int, Color> previousColors = new Dictionary<Vector3Int, Color>();

    public void ClearColors()
    {
        foreach(KeyValuePair<Vector3Int, Color> tileColor in previousColors)
        {
            gridOverlay.SetColor(tileColor.Key, tileColor.Value);
        }
        previousColors.Clear();
    }

    public void ToggleGridOverlay()
    {
        gridOverlay.gameObject.SetActive(!gridOverlay.gameObject.activeSelf);
        ClearColors();
    }

    public void HighlightTile(Vector3Int tilePosition, Color color)
    {
        if (!previousColors.ContainsKey(tilePosition))
        {
            previousColors.Add(tilePosition, gridOverlay.GetColor(tilePosition));
        }
        gridOverlay.SetTileFlags(tilePosition, TileFlags.None);
        gridOverlay.SetColor(tilePosition, color);
    }
}
