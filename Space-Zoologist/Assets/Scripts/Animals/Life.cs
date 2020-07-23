using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Common interface for the Population and FoodSource
/// </summary>
public interface Life
{
    /// <summary>
    /// Returns a reference to the NeedValues
    /// </summary>
    /// <returns>A reference</returns>
    Dictionary<string, float> GetNeedValues();

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
    void UpdateNeed(string need, float value);

    /// <summary>
    /// Get the accessibility status, true if accessibility or accessible terrain had changed
    /// </summary>
    bool GetAccessibilityStatus();
}
