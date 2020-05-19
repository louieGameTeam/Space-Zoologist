using System.Collections.Generic;
using UnityEngine;

// Where should data about each need be stored? On need itself or?
// Holds information about each species in game
public class SpeciesData : MonoBehaviour
{
    public Species Data {get; set;}
    public List<SpeciesNeed> DiscoveredNeeds {get; set;}
    public string Description {get; set;}
}
