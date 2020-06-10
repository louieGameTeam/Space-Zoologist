using System.Collections;
using System.Collections.Generic;
using System;
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
            // Check if need is a atmoshpere or a terrian need
            if (Enum.IsDefined(typeof(AtmoshpereComponent), need.NeedName))
            {
                systems["Atmoshpere"].AddPopulation(population);
            }
            else if (Enum.IsDefined(typeof(TileType), need.NeedName))
            {
                systems["Terrian"].AddPopulation(population);
            }
            else
            {
                // Foodsource need here
                Debug.Assert(systems.ContainsKey(need.NeedName), $"No { need.NeedName } system");
                systems[need.NeedName].AddPopulation(population);
            }
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
    /// Add a system so that populations can register with it via it's need name.
    /// </summary>
    /// <param name="needSystem">The system to add</param>
    public void AddSystem(NeedSystem needSystem)
    {
        systems.Add(needSystem.NeedName, needSystem);
    }

    /// <summary>
    /// Temporary update for the density, terrain and atmpshpere need system 
    /// </summary>
    public void UpdateSystems()
    {
        systems["Density"].UpdateSystem();
        systems["Terrian"].UpdateSystem();
        systems["Atmoshpere"].UpdateSystem();
    }
}
