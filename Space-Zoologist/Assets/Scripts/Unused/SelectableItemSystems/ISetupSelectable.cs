using UnityEngine;

// Trying to be explicit with how setting up selectable items works
public interface ISetupSelectable
{
    /// <summary>
    /// Add the methods you want to be invoked when this item is selected
    /// </summary>
    /// <param name="action"></param>
    void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action);
}
