using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodReferenceData : MonoBehaviour
{
    [SerializeField] public LevelDataReference LevelDataReference = default;
    public Dictionary<string, FoodSourceSpecies> FoodSources = new Dictionary<string, FoodSourceSpecies>();

    // Ensure the Species are all indexed by their name
    public void Start()
    {
        foreach(FoodSourceSpecies foodSource in this.LevelDataReference.LevelData.FoodSourceSpecies)
        {
            foreach(Item item in this.LevelDataReference.LevelData.Items)
            {
                if (item.Type.Equals(ItemType.Food) && item.ID.Equals(foodSource.name))
                {
                    this.FoodSources.Add(item.ID, foodSource);
                }
            }
        }
    }
}
