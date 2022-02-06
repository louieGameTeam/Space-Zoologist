using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextUIHandler : MonoBehaviour
{
    private Text DebugText;
    private TileDataController gridSystem;

    private void Start()
    {
        if (!GameManager.Instance.IsDebug)
            gameObject.SetActive(false);

        DebugText = GetComponent<Text>();

        gridSystem = GameManager.Instance.m_gridSystem;
    }

    void Update()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int pos = gridSystem.WorldToCell(worldPos);
        TileData tileData = gridSystem.GetTileData(pos);

        if (tileData != null)
            DebugText.text = tileData.ToString();
    }
}
