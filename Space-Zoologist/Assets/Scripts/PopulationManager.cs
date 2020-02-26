using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    // Temporary
    [SerializeField] private Species speciesToMake = default;

    private List<Population> populations = default;

    private float interval = 5.0f;
    private float elapsed = 0.0f;
    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= interval)
        {
            elapsed = 0.0f;
            CreatePopulation(speciesToMake);
        }
    }

    public void CreatePopulation(Species species)
    {
        GameObject gameObject = Instantiate(new GameObject(), this.transform);
        gameObject.AddComponent<Population>().species = species;
    }

}
