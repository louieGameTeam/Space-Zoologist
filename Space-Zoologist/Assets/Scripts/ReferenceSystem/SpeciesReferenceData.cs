using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesReferenceData : MonoBehaviour
{
    // Using singleton pattern so Species can be easily indexed by name
    [SerializeField] private List<Species> AddAllSpecies = new List<Species>();
    public Dictionary<string, Species> AllSpecies = new Dictionary<string, Species>();

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

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
