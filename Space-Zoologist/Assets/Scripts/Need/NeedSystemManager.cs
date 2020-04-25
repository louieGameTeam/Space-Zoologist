using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all need systems and the registration point for populations to register with specific need systems.
/// </summary>
public class NeedSystemManager : MonoBehaviour
{

    private Dictionary<NeedType, INeedSystem> systems = new Dictionary<NeedType, INeedSystem>();

    [SerializeField] public FoodSourceManager foodMan = default;

    public void RegisterPopulation(Population population, NeedType need)
    {
        Debug.Log($"Registering {population} with {need}");

        if (!systems.ContainsKey(need))
        {
            Debug.Log($"Adding new system: {need}");

            FoodDistributionSystem system = new FoodDistributionSystem();
            system.Initialize(need);

            systems.Add(need, system);
        }
        systems[need].RegisterPopulation(population);
    }

    public void UnregisterPopulation(Population population, NeedType need)
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

    public void UpdateSystem(NeedType need)
    {
        if (!systems.ContainsKey(need))
        {
            Debug.Log($"Adding new system: {need}");

            FoodDistributionSystem system = new FoodDistributionSystem();
            system.Initialize(need);

            systems.Add(need, system);
        }

        // Update for food sources
        if(need == NeedType.SpaceMaple)
        {
            Debug.Log($"update system: {need}");
            systems[need].Update(foodMan.getFoodByType(need));
        }
    }
}
