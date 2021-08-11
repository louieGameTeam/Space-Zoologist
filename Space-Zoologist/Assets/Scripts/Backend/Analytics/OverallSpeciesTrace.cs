using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OverallSpeciesTrace
{
    // A list containing names of the species present in the level.
    [SerializeField] private List<string> species;
    // A boolean indicating whether the player met the thriving requirements for at least one species in the level.
    [SerializeField] private bool metThrivingNeeds;
    // A list containing the species names whose thriving needs were met in the level.
    [SerializeField] private List<string> thrivingSpecies;
    // A list of singular species traces containing more detailed information.
    [SerializeField] private List<SpeciesTrace> traces;

    // PUBLIC GETTERS/SETTERS
    public List<string> Species
    {
        get { return species; }
        set { species = value; }
    }

    public bool MetThrivingNeeds
    {
        get { return metThrivingNeeds; }
        set { metThrivingNeeds = value; }
    }

    public List<string> ThrivingSpecies
    {
        get { return thrivingSpecies; }
        set { thrivingSpecies = value; }
    }

    public List<SpeciesTrace> Traces
    {
        get { return traces; }
        set { traces = value; }
    }
}
