using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enable on a gameobject with a population component to show information, such as animal count, above the population origin.
/// </summary>
public class PopulationDataDisplay : MonoBehaviour
{
    [SerializeField] private Text infoDisplay = default;

    Population population = default;

    void Start()
    {
        population = GetComponent<Population>();
    }

    void Update()
    {
        infoDisplay.text = $"Count: {population.Count}";
    }
}
