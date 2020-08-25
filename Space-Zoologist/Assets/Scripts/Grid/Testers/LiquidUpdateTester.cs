using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidUpdateTester : MonoBehaviour
{
    private TileSystem tileSystem;
    private Camera mainCamera;
    private TileContentsManager tileContentsManager;
    [SerializeField] float[] comp = { 1, 1, 1 };
    // Start is called before the first frame update
    private void Awake()
    {
        tileSystem = FindObjectOfType<TileSystem>();
        mainCamera = FindObjectOfType<Camera>();
        tileContentsManager = FindObjectOfType<TileContentsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector3Int cellLocation = tileSystem.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            tileSystem.ChangeLiquidBodyComposition(cellLocation, comp, true);
            // Debug.Log(tileContentsManager.tileContents[cellLocation][0].ToString());
        }
    }
}
