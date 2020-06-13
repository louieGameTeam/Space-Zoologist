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

    /// <summary>
    /// Initialize the universal need systems
    /// </summary>
    private void Awake()
    {
        // Add Density, atomsphere/tempeture and terrain NeedSystem
        AddSystem(new DensityNeedSystem(FindObjectOfType<ReservePartitionManager>(), FindObjectOfType<TileSystem>()));
        AddSystem(new AtmoshpereNeedSystem(FindObjectOfType<EnclosureSystem>()));
        AddSystem(new TerrianNeedSystem(FindObjectOfType<ReservePartitionManager>()));
    }

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
    /// Update all the need system that is mark "dirty"
    /// </summary>
    public void UpdateSystems()
    {
        // Systems should only update when the state is "dirty"
        foreach (KeyValuePair<string, NeedSystem> entry in systems)
        {
            NeedSystem system = entry.Value;

            if(system.isDirty)
            {
                system.UpdateSystem();
                //Debug.Log(String.Format("Updating {0}", system.NeedName));
            }
        }
    }
}
