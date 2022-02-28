using System;
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
    public Func<ItemID, bool> Filter { get; private set; }
    #endregion

    #region Constructors
    public ItemIDFilterAttribute(ItemRegistry.Category category)
        : this(id => id.Category == category) { }
    public ItemIDFilterAttribute(string nameFilter)
        : this(id => id.Data.Name.AnyNameContains(nameFilter)) { }
    public ItemIDFilterAttribute(Func<ItemID, bool> filter)
    {
        Filter = filter;
    }
    #endregion
}
