using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implemented by store menus to ensure they have their dependencies and handle placement of items.
/// </summary>
public interface IStoreMenu
{
    /// <summary>
    /// Inject the shared dependencies for the menus
    /// </summary>
    /// <param name="levelData"></param>
    /// <param name="cursorItem"></param>
    /// <param name="UIElements"></param>
    void SetupDependencies(LevelDataReference levelData, CursorItem cursorItem, List<RectTransform> UIElements);

    /// <summary>
    /// Replacement for Start which is called after SetupDependencies
    /// </summary>
    void Initialize();

    /// <summary>
    /// Determine if mouse position is in a valid location for placement of something.
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    bool IsPlacementValid(Vector3 mousePosition);
}
