using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Common interface for the Population and FoodSource objects that interact with the NeedSystems
/// </summary>
public interface Life
{
    /// <summary>
    /// Special terrain water need
    /// </summary>
    /// <remarks>
    /// This is used by food sources that need
    /// water as a terrain need and not a liquid need
    /// </remarks>
    /// <example>
    /// Kelp needs to be placed in water to grow, 
    /// but it still has liquid needs 
    /// for specific water compositions
    /// </example>
    Need TerrainWaterNeed { get; }

    /// <summary>
    /// Returns a reference to the NeedValues
    /// </summary>
    /// <returns>A reference</returns>
    Dictionary<ItemID, Need> GetNeedValues();

    /// <summary>
    /// Gets the position of the transform of the game object
    /// </summary>
    /// <returns>The position vector</returns>
    Vector3 GetPosition();

    /// <summary>
    /// Update the given need of the population with the given value.
    /// </summary>
    /// <param name="need">The need to update</param>
    /// <param name="value">The need's new value</param>
    void UpdateNeed(ItemID need, float value);

    /// <summary>
    /// Get the accessibility status, true if accessibility or accessible terrain had changed
    /// </summary>
    bool GetAccessibilityStatus();
}
