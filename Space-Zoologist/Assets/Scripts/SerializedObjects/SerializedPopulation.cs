using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedPopulation
{
    public MapItemSet population;
    public SerializedPopulation(AnimalSpecies animalSpecies, GameObject[] animals)
    {
        Vector3[] positions = new Vector3[animals.Length];
        for (int i = 0; i < animals.Length; i++)
        {
            positions[i] = animals[i].transform.position;
        }
        this.population = new MapItemSet(animalSpecies.name, positions);
    }
}
