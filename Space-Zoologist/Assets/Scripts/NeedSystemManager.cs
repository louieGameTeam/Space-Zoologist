using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedSystemManager : MonoBehaviour
{

    private Dictionary<NeedType, INeedSystem> systems = new Dictionary<NeedType, INeedSystem>();

    public void RegisterPopulation(Population population, NeedType need)
    {
        if (!systems.ContainsKey(need))
        {
            Debug.Log($"Trying to register a population to a non-existant system: {need}");
            return;
        }
        systems[need].RegisterPopulation(population);
    }
    public void UnregisterPopulation(Population population, NeedType need)
    {
        systems[need].UnregisterPopulation(population);
    }

    public void AddSystem(INeedSystem needSystem)
    {
        systems.Add(needSystem.Need, needSystem);
    }
}
