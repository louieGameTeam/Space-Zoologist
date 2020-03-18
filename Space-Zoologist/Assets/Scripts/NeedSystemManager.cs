using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all need systems and the registration point for populations to register with specific need systems.
/// </summary>
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

    /// <summary>
    /// Add a system to be managed. Allows 
    /// </summary>
    /// <param name="needSystem">The system to add</param>
    public void AddSystem(INeedSystem needSystem)
    {
        systems.Add(needSystem.Need, needSystem);
    }
}
