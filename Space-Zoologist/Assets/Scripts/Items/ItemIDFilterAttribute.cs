using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this attribute to a serialized instance of 'ItemID'
/// to limit the object's controls in the Unity Editor
/// </summary>
public class ItemIDFilterAttribute : PropertyAttribute
{
    #region Public Properties
    public ItemRegistry.Category Category { get; private set; }
    #endregion

    #region Constructors
    public ItemIDFilterAttribute(ItemRegistry.Category category)
    {
        Category = category;
    }
    #endregion
}
