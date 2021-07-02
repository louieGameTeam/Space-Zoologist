using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedPopulation
{
    public MapItemSet population;
    public SerializedPopulation(AnimalSpecies animalSpecies, Vector3[] animals)
    {
        this.population = new MapItemSet(animalSpecies.name, animals);
    }
}
