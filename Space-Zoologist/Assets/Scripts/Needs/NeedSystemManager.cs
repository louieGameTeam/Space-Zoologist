using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Manages all need systems and the registration point for populations to register with specific need systems.
/// </summary>
public class NeedSystemManager : MonoBehaviour
{
    public Dictionary<string, NeedSystem> Systems => systems;

    [SerializeField] private LevelData levelData = default;
    private Dictionary<string, NeedSystem> systems = new Dictionary<string, NeedSystem>();

    /// <summary>
    /// Initialize the universal need systems
    /// </summary>
    /// <remarks>Terrian -> FoodSource/Species -> Density, this order has to be fixed</remarks>
    private void Awake()
    {
        // Referrance to the RPM
        ReservePartitionManager rpm = FindObjectOfType<ReservePartitionManager>();

        // Add enviormental NeedSystem
        AddSystem(new AtmoshpereNeedSystem(FindObjectOfType<EnclosureSystem>()));
        AddSystem(new TerrainNeedSystem(FindObjectOfType<ReservePartitionManager>(), FindObjectOfType<TileSystem>()));

        // Add new FoodSourceNeedSystem
        foreach (FoodSourceSpecies foodSourceSpecies in levelData.FoodSourceSpecies)
        {
            AddSystem(new FoodSourceNeedSystem(rpm, foodSourceSpecies.SpeciesName));
        }
        // Add new FoodSourceNeedSystem
        foreach (AnimalSpecies animalSpecies in levelData.AnimalSpecies)
        {
            AddSystem(new SpeciesNeedSystem(rpm, animalSpecies.SpeciesName));
        }

        // Add Density NeedSystem
        AddSystem(new DensityNeedSystem(FindObjectOfType<ReservePartitionManager>(), FindObjectOfType<TileSystem>()));
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
                systems["Atmoshpere"].AddConsumer(life);
            }
            else if (Enum.IsDefined(typeof(TileType), need))
            {
                systems["Terrian"].AddConsumer(life);
                Debug.Log($"Registed {life} with Terrian ({need}) NeedSystem");
            }
            else
            {
                // Foodsource need here
                Debug.Assert(systems.ContainsKey(need), $"No { need } system");
                systems[need].AddConsumer(life);
                Debug.Log($"Register {life} with {need} system");
            }
        }
    }

    public void UnregisterPopulationNeeds(Life life)
    {
        foreach (string need in life.NeedsValues.Keys)
        {
            Debug.Assert(systems.ContainsKey(need));
            systems[need].RemoveConsumer(life);
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

            if (system.isDirty)
            {
                system.UpdateSystem();
            }
        }
    }
}