using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodReference", menuName = "ReferenceData/FoodReference")]
public class FoodReferenceData : ScriptableObject
{
    // Using singleton pattern so Species can be easily indexed by name
    [SerializeField] private List<FoodSourceSpecies> AddAllSpecies = new List<FoodSourceSpecies>();
    public Dictionary<string, FoodSourceSpecies> AllSpecies = new Dictionary<string, FoodSourceSpecies>();

    public FoodSourceSpecies FindSpecies(string species)
    {
        if (this.AllSpecies.ContainsKey(species))
        {
            return this.AllSpecies[species];
        }
        else
        {
            return null;
        }
    }

    // Ensure the Species are all indexed by their name
    public void OnValidate()
    {
        foreach(FoodSourceSpecies species in this.AddAllSpecies)
        {
            if (!this.AllSpecies.ContainsKey(species.SpeciesName))
            {
                this.AllSpecies.Add(species.SpeciesName, species);
            }
        }
    }
}
