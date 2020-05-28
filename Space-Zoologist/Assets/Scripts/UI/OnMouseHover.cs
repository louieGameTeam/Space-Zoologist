using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Add to any GameObjecto setup hover display
/// </summary>
public class OnMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject DisplayToAppear;

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.DisplayToAppear.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.DisplayToAppear.SetActive(false);
    }

    public void InitializeDisplay(GameObject display)
    {
        this.DisplayToAppear = display;
    }
}
