using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The interface of a system that calculates a particular need.
/// </summary>
public interface INeedSystem
{
    /// <summary>
    /// Binds a population to the system so that the system can calculate the population's associated need.
    /// </summary>
    /// <param name="population">The population to register</param>
    void RegisterPopulation(Population population);

    /// <summary>
    /// Unbinds the population from the system so that the system will no longer update the population's associated need.
    /// </summary>
    /// <param name="population">The population to unregister</param>
    void UnregisterPopulation(Population population);

    /// <summary>
    /// The need that the system calculates and updates.
    /// </summary>
    NeedType Need { get; }
}


