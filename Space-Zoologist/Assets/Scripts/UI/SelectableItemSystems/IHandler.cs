using UnityEngine;

public interface IHandler
{
    /// <summary>
    /// Add this as a listener to the ItemSelectedEvent to determine what should happen with the selected GameObject
    /// </summary>
    /// <param name="itemSelected"></param>
    void OnItemSelectedEvent(GameObject itemSelected);
}
