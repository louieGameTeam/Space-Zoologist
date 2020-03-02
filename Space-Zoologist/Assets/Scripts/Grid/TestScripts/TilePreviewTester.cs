using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilePreviewTester : MonoBehaviour
{
    [SerializeField] private TilePreview tilePreview;
    [SerializeField] private List<TileBase> selectableTiles = default;
    [SerializeField] bool isBlockMode = default;
    private TileBase selectedTile = default;

    [TextArea]
    [SerializeField] string displayText = default;

    [SerializeField] private Canvas canvas;
    
    void Awake()
    {
        selectedTile = selectableTiles[0];
        tilePreview.isBlockMode = isBlockMode;
    }

    private void Start()
    {
        var text = canvas.gameObject.AddComponent<Text>();
        text.text = displayText;

        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.material = ArialFont.material;

        tilePreview.isBlockMode = isBlockMode;
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
            tilePreview.StartPreview(selectedTile);
        }
        if (Input.GetMouseButtonUp(0))
        {
            tilePreview.StopPreview();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            tilePreview.isBlockMode = !tilePreview.isBlockMode;
        }
    }
}
