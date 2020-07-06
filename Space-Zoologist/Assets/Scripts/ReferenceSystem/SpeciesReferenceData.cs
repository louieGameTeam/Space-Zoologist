using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeciesReference", menuName = "ReferenceData/SpeciesReference")]
public class SpeciesReferenceData : ScriptableObject
{
    // Using singleton pattern so Species can be easily indexed by name
    [SerializeField] private List<Species> AddAllSpecies = new List<Species>();
    public Dictionary<string, Species> AllSpecies = new Dictionary<string, Species>();

    public Species FindSpecies(string species)
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
        foreach(Species species in this.AddAllSpecies)
        {
            if (!this.AllSpecies.ContainsKey(species.SpeciesName))
            {
                this.AllSpecies.Add(species.SpeciesName, species);
            }
        }
    }
}
