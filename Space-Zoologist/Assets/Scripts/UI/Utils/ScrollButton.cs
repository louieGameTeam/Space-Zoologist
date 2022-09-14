using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    [Tooltip("Reference to the scroll view that this button modifies")]
    private ScrollRect scrollRect;
    [SerializeField]
    [Tooltip("Direction that the button scrolls")]
    private Vector2 direction;
    [SerializeField]
    [Tooltip("Percentage of the scroll area that the button scrolls per second")]
    private float normalSpeed;

    [Tooltip("True if the pointer is down on this button and false if not")]
    private bool pointerIsDown = false;

    // Ensure that direction is normalized on start
    private void Awake()
    {
        direction = direction.normalized;
    }

    private void Update()
    {
        if(pointerIsDown)
        {
            scrollRect.normalizedPosition += direction * normalSpeed * Time.deltaTime;

        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        pointerIsDown = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        pointerIsDown = false;
    }
}
