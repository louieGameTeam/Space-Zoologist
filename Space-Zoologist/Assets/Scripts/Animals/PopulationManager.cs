using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    private List<Population> populations = new List<Population>();

    public void CreatePopulation(Species species, Vector2Int origin)
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.parent = this.transform;
        gameObject.name = species.SpeciesName;
        gameObject.AddComponent<Population>();
        gameObject.GetComponent<Population>().InitializeFromSpecies(species, origin);
    }
}
