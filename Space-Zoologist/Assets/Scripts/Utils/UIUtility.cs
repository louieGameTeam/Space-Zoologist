using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public class UIUtility : MonoBehaviour
{
    public static UIUtility ins;
    [SerializeField] List<RectTransform> UIElements = default;

    private void Awake()
    {
        // Variable initializations
        if (ins != null && this != ins)
        {
            Destroy(this);
        }
        else
        {
            ins = this;
        }
    }

    public bool IsCursorOverUI(PointerEventData eventData)
    {
        foreach (RectTransform UIElement in this.UIElements)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(UIElement, eventData.position))
            {
                return true;
            }
        }
        return false;
    }
}
