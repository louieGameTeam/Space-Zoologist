using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implemented by classes who need to validate the placement of something.
/// </summary>
public interface IValidatePlacement
{
    /// <summary>
    /// Determine if mouse position is in a valid location for placement of something.
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    bool IsPlacementValid(Vector3 mousePosition);
}
