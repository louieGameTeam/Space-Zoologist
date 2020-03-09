using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedSystemsTester : MonoBehaviour
{
    [SerializeField] private PopulationManager populationManager = default;
    [SerializeField] private GameObject needSystemGameObject = default;
    private INeedSystem needSystem = default;
    [SerializeField] private List<Species> species = default;


    // Start is called before the first frame update
    void Start()
    {
        if (!populationManager) populationManager = FindObjectOfType<PopulationManager>();
        needSystem = needSystemGameObject.GetComponent<TestSystem>();
        NeedSystemManager.AddSystem(needSystem);
        foreach(Species s in species)
        {
            populationManager.CreatePopulation(s, Vector2Int.zero);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
