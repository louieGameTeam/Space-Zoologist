using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedSystemsTester : MonoBehaviour
{
    [SerializeField] private PopulationManager populationManager = default;
    [SerializeField] private NeedSystemManager needSystemManager = default;
    [SerializeField] private NeedSystem spaceMapleSystem = default;
    [SerializeField] private NeedSystem aridBushSystem = default;
    [SerializeField] private List<AnimalSpecies> species = default;

    [SerializeField] private Text spaceMapleSystemText = default;
    [SerializeField] private Text aridBushSystemText = default;

    void Start()
    {
        if (!populationManager) populationManager = FindObjectOfType<PopulationManager>();
        if (!needSystemManager) needSystemManager = FindObjectOfType<NeedSystemManager>();
        foreach(AnimalSpecies s in species)
        {
            populationManager.CreatePopulation(s, Vector2Int.zero);
        }
    }


}
