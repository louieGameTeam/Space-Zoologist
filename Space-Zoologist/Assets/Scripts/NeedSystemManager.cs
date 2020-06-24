using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Manages all need systems and the registration point for populations to register with specific need systems.
/// </summary>
public class NeedSystemManager : MonoBehaviour
{
    [SerializeField] private LevelData levelData = default;

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

        //// Add SpeciesNeedSystems
        //foreach (AnimalSpecies animalSpecies in levelData.AnimalSpecies)
        //{
        //    AddSystem(new SpeciesNeedSystem(animalSpecies.name, FindObjectOfType<ReservePartitionManager>()));
        //}

        //// Add FoodSourceNeedSystems to NeedSystemManager
        //foreach (FoodSourceSpecies foodSourceSpecies in levelData.FoodSourceSpecies)
        //{
        //    AddSystem(foodSourceNeedSystem);
        //}
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
                //Debug.Log($"Register {need} in {life}");
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
    /// <remarks>
    /// The order of the NeedSystems' update metter,
    /// this should be their relative order(temp) :
    /// Terrian/Atmosphere -> Species -> FoodSource -> Density
    /// This order can be gerenteed in how NeedSystems is add to the manager in Awake()
    /// </remarks>
    public void UpdateSystems()
    {
        // TODO: Systems should only update when the state is "dirty"
        foreach (KeyValuePair<string, NeedSystem> entry in systems)
        {
            NeedSystem system = entry.Value;
            system.UpdateSystem();
        }
    }
}
