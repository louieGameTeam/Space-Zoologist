using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesReferenceData : MonoBehaviour
{
    private LevelDataReference levelDataReference = default;
    public Dictionary<string, FoodSourceSpecies> FoodSources = new Dictionary<string, FoodSourceSpecies>();
    public Dictionary<string, AnimalSpecies> AnimalSpecies = new Dictionary<string, AnimalSpecies>();

    // Ensure the Species are all indexed by their name
    public void Start()
    {
        levelDataReference = FindObjectOfType<LevelDataReference>();
        foreach (FoodSourceSpecies foodSource in this.levelDataReference.LevelData.FoodSourceSpecies)
        {
            foreach (LevelData.ItemData data in levelDataReference.LevelData.ItemQuantities)
            {
                Item item = data.itemObject;
                if (item)
                {
                    if (item.Type.Equals(ItemType.Food) && item.ID.Equals(foodSource.SpeciesName))
                    {
                        this.FoodSources.Add(item.ID, foodSource);
                    }
                }
            }
        }
        foreach (AnimalSpecies animalSpecies in this.levelDataReference.LevelData.AnimalSpecies)
        {
            this.AnimalSpecies.Add(animalSpecies.SpeciesName, animalSpecies);
        }
    }
}
