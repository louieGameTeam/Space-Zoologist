using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

// To run NS updates that can run in paralle
public struct BatchRoutine : IJob
{
    [ReadOnly]
    public NativeArray<UpdateRoutine> routines;

    public bool IsInBatch()
    {
        return true;
    }

    public void Execute()
    {
        foreach (UpdateRoutine routine in this.routines)
        {
            routine.Schedule();
        }
    }
}

// To run a signle NS update
public struct UpdateRoutine : IJob
{
    public NeedSystem system;

    public UpdateRoutine(NeedSystem system)
    {
        this.system = system;
    }

    public void Execute()
    {
        if (system.IsDirty)
        {
            Debug.Log($"Updating {system.NeedType} NS by dirty flag");
            system.UpdateSystem();
        }
        else if (system.CheckState())
        {
            Debug.Log($"Updating {system.NeedType} NS by dirty pre-check");
            system.UpdateSystem();
        }
    }
}

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

    private Dictionary<NeedType, UpdateRoutine> updateRoutines = new Dictionary<NeedType, UpdateRoutine>();

    private NativeArray<UpdateRoutine> updateRoutinsInBatchOne;
    private NativeArray<UpdateRoutine> updateRoutinsInBatchTwo;
    private NativeArray<UpdateRoutine> updateRoutinsInBatchThree;

    private BatchRoutine batchRoutineOne;
    private BatchRoutine batchRoutineTwo;
    private BatchRoutine batchRoutineThree;

    /// <summary>
    /// Initialize the manager, create need sysetems and set up routines
    /// </summary>
    /// <remarks>Terrian/Atmoshpere/Liquid -> FoodSource/Species -> Density/Symbiosis, this is the fixed order</remarks>
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

        // Add Symbiosis NeedSystem
        AddSystem(new SymbiosisNeedSystem(ReservePartitionManager));

        // Set up rountin batches
        //setupRoutineBatches();

        FoodSourceManager.Initialize();
        PopulationManager.Initialize();
    }

    private void setupRoutineBatches()
    {
        updateRoutinsInBatchOne = new NativeArray<UpdateRoutine>(3, Allocator.Persistent);
        updateRoutinsInBatchTwo = new NativeArray<UpdateRoutine>(2, Allocator.Persistent);
        updateRoutinsInBatchThree = new NativeArray<UpdateRoutine>(2, Allocator.Persistent);

        // Batch 1
        this.updateRoutinsInBatchOne[0] = this.updateRoutines[NeedType.Atmosphere];
        this.updateRoutinsInBatchOne[1] = this.updateRoutines[NeedType.Terrain];
        this.updateRoutinsInBatchOne[2] = this.updateRoutines[NeedType.Liquid];

        // Batch 2
        this.updateRoutinsInBatchTwo[0] = this.updateRoutines[NeedType.FoodSource];
        this.updateRoutinsInBatchTwo[1] = this.updateRoutines[NeedType.Species];

        // Batch 3
        this.updateRoutinsInBatchThree[0] = this.updateRoutines[NeedType.Density];
        this.updateRoutinsInBatchThree[1] = this.updateRoutines[NeedType.Symbiosis];

        // Set up routine batches
        this.batchRoutineOne.routines = this.updateRoutinsInBatchOne;
        this.batchRoutineTwo.routines = this.updateRoutinsInBatchTwo;
        this.batchRoutineThree.routines = this.updateRoutinsInBatchThree;
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

    /// <summary>
    /// Unregister a consumer
    /// </summary>
    /// <param name="life">The consumer to be unregistered from the need systems</param>
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

            // Create update routine
            this.updateRoutines.Add(needSystem.NeedType, new UpdateRoutine(needSystem));
        }
        else
        {
            Debug.Log($"{needSystem.NeedType} need system already existed");
        }
    }

    public void ForceUpdateSystems()
    {
        foreach (KeyValuePair<NeedType, NeedSystem> entry in systems)
        {
            entry.Value.UpdateSystem();
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
        // Update populations' accessible map when terrain was modified
        if (this.TileSystem.HasTerrainChanged)
        {
            this.ReservePartitionManager.UpdateAccessMapChangedAt(this.TileSystem.chagnedTiles);
        }

        // Go throught batches and call updates
        //JobHandle batchOneJobHandle = this.batchRoutineOne.Schedule();
        //JobHandle batchTwoJobHandle = this.batchRoutineTwo.Schedule(batchOneJobHandle);
        //JobHandle batchThreeJobHandle = this.batchRoutineTwo.Schedule(batchTwoJobHandle);

        ////// Wait till all updates are complete
        //batchThreeJobHandle.Complete();

        foreach (UpdateRoutine routine in this.updateRoutines.Values)
        {
            routine.Schedule();
        }

        // Update systems
        //foreach (NeedSystem system in this.systems.Values)
        //{
        //    if (system.IsDirty)
        //    {
        //        Debug.Log($"Updating {system.NeedType} NS by dirty flag");
        //        system.UpdateSystem();
        //    }
        //    else if (system.CheckState())
        //    {
        //        Debug.Log($"Updating {system.NeedType} NS by dirty pre-check");
        //        system.UpdateSystem();
        //    }
        //}


        // Reset pop accessibility status
        PopulationManager.UdateAllPopulationStateForChecking(); 

        // Reset food source accessibility status
        FoodSourceManager.UpdateAccessibleTerrainInfoForAll();

        // Reset terrain modified flag
        TileSystem.HasTerrainChanged = false;
    }

}