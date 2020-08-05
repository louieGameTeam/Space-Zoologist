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
    public Dictionary<NeedType, NeedSystem> Systems => systems;

    private Dictionary<NeedType, NeedSystem> systems = new Dictionary<NeedType, NeedSystem>();
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] FoodSourceManager FoodSourceManager = default;
    [SerializeField] EnclosureSystem EnclosureSystem = default;
    [SerializeField] TileSystem TileSystem = default;
    [SerializeField] ReservePartitionManager ReservePartitionManager = default;

    /// <summary>
    /// Initialize the universal need systems
    /// </summary>
    /// <remarks>Terrian/Atmoshpere -> FoodSource/Species -> Density, this order has to be fixed</remarks>
    private void Start()
    {
        // Add enviormental NeedSystem
        AddSystem(new AtmosphereNeedSystem(EnclosureSystem));
        AddSystem(new TerrainNeedSystem(ReservePartitionManager, TileSystem));
        AddSystem(new LiquidNeedSystem(ReservePartitionManager, TileSystem));


        // FoodSource and Species NS
        AddSystem(new FoodSourceNeedSystem(ReservePartitionManager));
        AddSystem(new SpeciesNeedSystem(ReservePartitionManager));

        // Add Density NeedSystem
        AddSystem(new DensityNeedSystem(ReservePartitionManager, TileSystem));


        FoodSourceManager.Initialize();
        PopulationManager.Initialize();
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
    private void AddSystem(NeedSystem needSystem)
    {
        if (!this.systems.ContainsKey(needSystem.NeedType))
        {
            systems.Add(needSystem.NeedType, needSystem);
        }
        else
        {
            Debug.Log($"{needSystem.NeedType} need system already existed");
        }
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
        foreach (KeyValuePair<NeedType, NeedSystem> entry in systems)
        {
            NeedSystem system = entry.Value;

            if (system.IsDirty)
            {
                Debug.Log($"Updating {system.NeedType} NS by dirty flag");
                system.UpdateSystem();
            }
            else if(system.CheckState())
            {
                Debug.Log($"Updating {system.NeedType} NS by dirty pre-check");
                system.UpdateSystem();
            }
        }

        // Reset pop accessibility status
        PopulationManager.ResetAccessibilityStatus(); 

        // Reset food source accessibility status
        FoodSourceManager.UpdateAccessibleTerrainInfoForAll();
    }

}