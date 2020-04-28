using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    /// <summary>
    /// Set up the ItemSelectedEvent (UnityEvent<SelectableItem>) reference. This should be have listeners to the methods you want to invoke when an item is selected.
    /// </summary>
    /// <param name="action"></param>
    void SetupHandler(SelectableItem item);

    /// <summary>
    /// Add this as a listener to the ItemSelectedEvent to determine what should happen with the selected GameObject
    /// </summary>
    /// <param name="itemSelected"></param>
    void OnItemSelectedEvent(GameObject itemSelected);
}
