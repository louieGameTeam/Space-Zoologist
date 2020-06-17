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
        AddSystem(new TerrianNeedSystem(FindObjectOfType<ReservePartitionManager>(), FindObjectOfType<TileSystem>()));
    }

    /// <summary>
    /// Register a Population or FoodSource with the systems using the strings need names.b
    /// </summary>
    /// <param name="life">This could be a Population or FoodSource since they both inherit from Life</param>
    public void RegisterWithNeedSystems(Life life)
    {
        foreach (string need in life.NeedsValues.Keys)
        {
            // Check if need is a atmoshpere or a terrian need
            if (Enum.IsDefined(typeof(AtmoshpereComponent), need))
            {
                systems["Atmoshpere"].AddPopulation(life);
            }
            else if (Enum.IsDefined(typeof(TileType), need))
            {
                systems["Terrian"].AddPopulation(life);
            }
            else
            {
                // Foodsource need here
                Debug.Assert(systems.ContainsKey(need), $"No { need } system");
                systems[need].AddPopulation(life);
            }
        }
    }

    public void UnregisterPopulationNeeds(Life life)
    {
        foreach (string need in life.NeedsValues.Keys)
        {
            Debug.Assert(systems.ContainsKey(need));
            systems[need].RemovePopulation(life);
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
