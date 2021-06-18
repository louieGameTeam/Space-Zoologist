using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTester : MonoBehaviour
{
    private LevelDataReference LevelDataReference;
    private FoodSourceStoreSection FoodSourceStoreSection;
    private TileSystem TileSystem;
    private Camera Camera;
    private void Awake()
    {
        this.LevelDataReference = FindObjectOfType<LevelDataReference>();
        this.FoodSourceStoreSection = FindObjectOfType<FoodSourceStoreSection>();
        this.TileSystem = FindObjectOfType<TileSystem>();
        this.Camera = FindObjectOfType<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3Int cellPos = TileSystem.WorldToCell(this.Camera.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetMouseButtonDown(2))
        {
            this.FoodSourceStoreSection.ManuallyPlaceItem(this.LevelDataReference.LevelData.items[0], cellPos);
        }
    }
}
