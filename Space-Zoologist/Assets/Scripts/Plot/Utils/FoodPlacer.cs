using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodPlacementData
{
    [SerializeField] public Vector3Int location = default;
    [SerializeField] public Item item = default;
}   

public class FoodPlacer : MonoBehaviour
{
    [SerializeField] List<FoodPlacementData> FoodPlacementDatas = default;
    [SerializeField] FoodSourceStoreSection FoodSourceStoreSection = default;

    public void PlaceFood()
    {
/*        foreach (FoodPlacementData food in this.FoodPlacementDatas)
        {
            FoodSourceStoreSection.ManuallyPlaceItem(food.item, food.location);
        }*/
    }
}
