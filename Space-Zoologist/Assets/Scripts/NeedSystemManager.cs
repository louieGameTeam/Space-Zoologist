using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all need systems and the registration point for populations to register with specific need systems.
/// </summary>
public class NeedSystemManager : MonoBehaviour
{
    private Dictionary<NeedName, INeedSystem> systems = new Dictionary<NeedName, INeedSystem>();

    public void RegisterPopulation(Population population, NeedName need)
    {
        if (!systems.ContainsKey(need))
        {
            Debug.Log($"Trying to register a population to a non-existant system: {need}");
            return;
        }
        systems[need].RegisterPopulation(population);
    }
    public void UnregisterPopulation(Population population, NeedName need)
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
