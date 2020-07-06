using UnityEngine;

/// <summary>
/// Handles sprite rendering object selection through ItemSelectedEvent (UnityEvent<GameObject>)
/// </summary>

public class SelectableSprite : MonoBehaviour
{
    public ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    public void SetupItemSelectedHandler(ItemSelectedEvent action)
    {
        this.OnItemSelectedEvent = action;
    }

    void OnMouseDown()
    {
        this.OnItemSelectedEvent.Invoke(this.gameObject);
    }
}