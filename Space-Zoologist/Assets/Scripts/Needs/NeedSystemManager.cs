using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages all need systems and the registration point for populations to register with specific need systems.
/// </summary>
public class NeedSystemManager : MonoBehaviour
{
    // Singleton
    public static NeedSystemManager ins;

    public Dictionary<string, NeedSystem> Systems => systems;

    [SerializeField] private LevelData levelData = default;
    private Dictionary<string, NeedSystem> systems = new Dictionary<string, NeedSystem>();

    private void Awake()
    {
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }
    }

    /// <summary>
    /// Initialize the universal need systems
    /// </summary>
    /// <remarks>Terrian/Atmoshpere -> FoodSource/Species -> Density, this order has to be fixed</remarks>
    private void Start()
    {
        // Referrance supprot systems
        ReservePartitionManager rpm = ReservePartitionManager.ins;
        EnclosureSystem enclosureSystem = EnclosureSystem.ins;
        TileSystem tileSystem = TileSystem.ins;

        // Add enviormental NeedSystem
        AddSystem(new AtmosphereNeedSystem(enclosureSystem));
        AddSystem(new TerrainNeedSystem(rpm, tileSystem));

        AddSystem(new FoodSourceNeedSystem(rpm));
        AddSystem(new SpeciesNeedSystem(rpm));

        // Add new FoodSourceNeedSystem
        //foreach (FoodSourceSpecies foodSourceSpecies in levelData.FoodSourceSpecies)
        //{
        //    AddSystem(new FoodSourceNeedSystem(rpm, foodSourceSpecies.SpeciesName));
        //}

        // Add new FoodSourceNeedSystem
        //foreach (AnimalSpecies animalSpecies in levelData.AnimalSpecies)
        //{
        //    AddSystem(new SpeciesNeedSystem(rpm, animalSpecies.SpeciesName));    
        //}

        // Add Density NeedSystem
        AddSystem(new DensityNeedSystem(rpm, tileSystem));

        // Intitialize the managers after NS's are added
        FoodSourceManager.ins.Initialize();
        PopulationManager.ins.Initialize();
    }

    /// <summary>
    /// Register a Population or FoodSource with the systems using the strings need names.b
    /// </summary>
    /// <param name="life">This could be a Population or FoodSource since they both inherit from Life</param>
    public void RegisterWithNeedSystems(Life life)
    {
        // Register to NS by NeedType (string)
        foreach (Need need in life.GetNeedValues().Values)
        {
            Debug.Assert(systems.ContainsKey(need.NeedType), $"No { need.NeedType } system");
            systems[need.NeedType].AddConsumer(life);
        }
    }

    public void UnregisterPopulationNeeds(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            Debug.Assert(systems.ContainsKey(need.NeedType), $"No { need } system");
            systems[need.NeedType].RemoveConsumer(life);
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
        foreach (KeyValuePair<string, NeedSystem> entry in systems)
        {
            NeedSystem system = entry.Value;

            if (system.IsDirty)
            {
                Debug.Log($"Updating {system.NeedName} NS by dirty flag");
                system.UpdateSystem();
            }
            else if(system.CheckState())
            {
                Debug.Log($"Updating {system.NeedName} NS by dirty pre-check");
                system.UpdateSystem();
            }
        }

        // Reset pop accessibility status
        foreach (Population pop in ReservePartitionManager.ins.PopulationAccessbilityStatus.Keys.ToList())
        {
            ReservePartitionManager.ins.PopulationAccessbilityStatus[pop] = false;
        }
    }

}