using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesReferenceData : MonoBehaviour
{
    public Dictionary<ItemID, FoodSourceSpecies> FoodSources = new Dictionary<ItemID, FoodSourceSpecies>();
    public Dictionary<ItemID, AnimalSpecies> AnimalSpecies = new Dictionary<ItemID, AnimalSpecies>();

    // Ensure the Species are all indexed by their name
    public void Start()
    {
        foreach (FoodSourceSpecies foodSource in LevelDataReference.instance.LevelData.FoodSourceSpecies)
        {
            foreach (LevelData.ItemData data in LevelDataReference.instance.LevelData.ItemQuantities)
            {
                Item item = data.itemObject;
                if (item)
                {
                    if (item.Type.Equals(ItemRegistry.Category.Food) && item.ID.Equals(foodSource.ID))
                    {
                        this.FoodSources.Add(item.ID, foodSource);
                    }
                }
            }
        }
        foreach (AnimalSpecies animalSpecies in LevelDataReference.instance.LevelData.AnimalSpecies)
        {
            this.AnimalSpecies.Add(animalSpecies.ID, animalSpecies);
        }
    }
}
