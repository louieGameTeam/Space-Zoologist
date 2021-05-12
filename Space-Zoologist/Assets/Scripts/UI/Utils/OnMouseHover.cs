using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Add to any GameObjecto setup hover display
/// </summary>
public class OnMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemSelectedEvent MouseEnterEvent = new ItemSelectedEvent();
    public UnityEvent MouseExitEvent = new UnityEvent(); 

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Entered");
        this.MouseEnterEvent.Invoke(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.MouseExitEvent.Invoke();
    }
}
