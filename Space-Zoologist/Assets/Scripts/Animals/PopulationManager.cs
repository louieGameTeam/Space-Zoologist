using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    private List<Population> populations = default;

    private void Update()
    {

    }

    public void CreatePopulation(Species species, Vector2Int location)
    {
        GameObject gameObject = Instantiate(new GameObject(), this.transform);
        gameObject.AddComponent<Population>();
        gameObject.GetComponent<Population>().InitializeFromSpecies(species);
    }

}
