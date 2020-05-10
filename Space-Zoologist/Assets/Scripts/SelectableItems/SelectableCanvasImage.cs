using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles canvas object selection through ItemSelectedEvent (UnityEvent<GameObject>)
/// </summary>

[System.Serializable]
public class ItemSelectedEvent : UnityEvent<GameObject> { }
public class SelectableCanvasImage : MonoBehaviour
{
    // What should happen when the item is selected
    private ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    public void SetupItemSelectedHandler(ItemSelectedEvent action)
    {
        this.OnItemSelectedEvent = action;
    }

    public void ItemSelected()
    {
        this.OnItemSelectedEvent.Invoke(this.gameObject);
    }
}