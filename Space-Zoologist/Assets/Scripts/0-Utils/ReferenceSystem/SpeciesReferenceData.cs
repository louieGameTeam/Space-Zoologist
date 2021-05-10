using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesReferenceData : MonoBehaviour
{
    [SerializeField] public LevelDataReference LevelDataReference = default;
    public Dictionary<string, FoodSourceSpecies> FoodSources = new Dictionary<string, FoodSourceSpecies>();
    public Dictionary<string, AnimalSpecies> AnimalSpecies = new Dictionary<string, AnimalSpecies>();

    // Ensure the Species are all indexed by their name
    public void Start()
    {
        foreach (FoodSourceSpecies foodSource in this.LevelDataReference.LevelData.FoodSourceSpecies)
        {
            foreach (Item item in this.LevelDataReference.LevelData.Items)
            {
                if (item.Type.Equals(ItemType.Food) && item.ID.Equals(foodSource.SpeciesName))
                {
                    this.FoodSources.Add(item.ID, foodSource);
                }
            }
        }
        foreach (AnimalSpecies animalSpecies in this.LevelDataReference.LevelData.AnimalSpecies)
        {
            this.AnimalSpecies.Add(animalSpecies.name, animalSpecies);
        }
    }
}
