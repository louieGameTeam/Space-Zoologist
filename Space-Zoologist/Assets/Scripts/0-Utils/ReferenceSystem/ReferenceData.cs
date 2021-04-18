using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceData : MonoBehaviour
{
    [SerializeField] public LevelDataReference LevelDataReference = default;
    public Dictionary<string, FoodSourceSpecies> FoodSources = new Dictionary<string, FoodSourceSpecies>();
    public Dictionary<string, AnimalSpecies> Species = new Dictionary<string, AnimalSpecies>();

    // Ensure the Species are all indexed by their name
    public void Start()
    {
        foreach(FoodSourceSpecies foodSource in this.LevelDataReference.LevelData.FoodSourceSpecies)
        {
            foreach(Item item in this.LevelDataReference.LevelData.Items)
            {
                if (item.Type.Equals(ItemType.Food) && item.ID.Equals(foodSource.SpeciesName))
                {
                    this.FoodSources.Add(item.ID, foodSource);
                }
            }
        }
        foreach (AnimalSpecies species in this.LevelDataReference.LevelData.AnimalSpecies)
        {
            foreach (Item item in this.LevelDataReference.LevelData.Items)
            {
                if (item.Type.Equals(ItemType.Pod) && item.ID.Equals(species.SpeciesName))
                {
                    this.Species.Add(item.ID, species);
                }
            }
        }
    }
}
