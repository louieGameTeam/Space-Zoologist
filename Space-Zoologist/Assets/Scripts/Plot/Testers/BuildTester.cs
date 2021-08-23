using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTester : MonoBehaviour
{
    private LevelDataReference LevelDataReference;
    private FoodSourceStoreSection FoodSourceStoreSection;
    private GridSystem GridSystem;
    private Camera Camera;
    private void Awake()
    {
        this.LevelDataReference = FindObjectOfType<LevelDataReference>();
        this.FoodSourceStoreSection = FindObjectOfType<FoodSourceStoreSection>();
        this.GridSystem = FindObjectOfType<GridSystem>();
        this.Camera = FindObjectOfType<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3Int cellPos = GridSystem.WorldToCell(this.Camera.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetMouseButtonDown(2))
        {
            this.FoodSourceStoreSection.ManuallyPlaceItem(this.LevelDataReference.LevelData.itemQuantities[0].itemObject, cellPos);
        }
    }
}
