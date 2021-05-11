using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridOverlay : MonoBehaviour
{
    [SerializeField] Tilemap gridOverlay = default;
    [SerializeField] Color gridColor = default;
    bool isOn = false;
    Dictionary<Vector3Int, Color> previousColors = new Dictionary<Vector3Int, Color>();
    // Start is called before the first frame update
    void Start()
    {
        //gridOverlay.color = new Color(0, 0, 0, 0);
    }

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
        if (isOn)
        {
            gridOverlay.color = new Color(0, 0, 0, 0);
        }
        else
        {
            gridOverlay.color = gridColor;
        }
    }

    public void HighlightTile(Vector3Int tilePosition, Color color)
    {
        if (previousColors.ContainsKey(tilePosition))
        {
            return;
        }
        previousColors.Add(tilePosition, gridOverlay.GetColor(tilePosition));
        gridOverlay.SetTileFlags(tilePosition, TileFlags.None);
        gridOverlay.SetColor(tilePosition, color);
    }
}
