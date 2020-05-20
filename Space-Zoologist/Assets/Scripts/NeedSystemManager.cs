using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all need systems and the registration point for populations to register with specific need systems.
/// </summary>
public class NeedSystemManager : MonoBehaviour
{
    private Dictionary<string, NeedSystem> systems = new Dictionary<string, NeedSystem>();

    public void RegisterPopulationNeeds(Population population)
    {
        foreach (Need need in population.Species.Needs.Values)
        {
            Debug.Assert(systems.ContainsKey(need.NeedName), $"No { need.NeedName } system");
            systems[need.NeedName].AddPopulation(population);
        }
    }

    public void UnregisterPopulationNeeds(Population population)
    {
        foreach (Need need in population.Species.Needs.Values)
        {
            Debug.Assert(systems.ContainsKey(need.NeedName));
            systems[need.NeedName].RemovePopulation(population);
        }
    }

    /// <summary>
    /// Add a system to be managed.
    /// </summary>
    /// <param name="needSystem">The system to add</param>
    public void AddSystem(NeedSystem needSystem)
    {
        systems.Add(needSystem.NeedName, needSystem);
    }
}
