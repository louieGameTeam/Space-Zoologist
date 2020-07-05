using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Component of a gameobject to store an image of a selected item and report back the mouse events that occur while the item resides underneath the cursor.
/// </summary>
[RequireComponent(typeof(Image))]
public class CursorItem : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    // Events and delegates to handle the different mouse events. Other classes subscribe to the events during a call to CursorItem.Begin and unsubscribe during the CursorItem.Stop call.
    public delegate void ClickHandler(PointerEventData eventData);
    public event ClickHandler onClick;
    public delegate void PointerDownHandler(PointerEventData eventData);
    public event PointerDownHandler onPointerDown;
    public delegate void PointerUpHandler(PointerEventData eventData);
    public event PointerUpHandler onPointerUp;

    // The sprite to display under the cursor.
    public Sprite Sprite => image.sprite;
    // The image to display the sprite.
    private Image image = default;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    /// <summary>
    /// Start displaying a sprite and hook callbacks to associated mouse events.
    /// </summary>
    /// <param name="sprite">The sprite to display under the cursor.</param>
    /// <param name="clickHandler">Callback to receive mouse click events. Optional.</param>
    /// <param name="pointerDownHandler">Callback to receive mouse down events. Optional.</param>
    /// <param name="pointerUpHandler">Callback to receive mouse up events. Optional.</param>
    public void Begin(Sprite sprite, ClickHandler clickHandler = null, PointerDownHandler pointerDownHandler = null, PointerUpHandler pointerUpHandler = null)
    {
        image.enabled = true;
        image.sprite = sprite;
        onClick += clickHandler;
        onPointerDown += pointerDownHandler;
        onPointerUp += pointerUpHandler;
    }

    /// <summary>
    /// Stop displaying the sprite and unhook callbacks from their associated mouse events.
    /// </summary>
    public void Stop(ClickHandler clickHandler = null, PointerDownHandler pointerDownHandler = null, PointerUpHandler pointerUpHandler = null)
    {
        onClick -= clickHandler;
        onPointerDown -= pointerDownHandler;
        onPointerUp -= pointerUpHandler;
        image.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        onPointerUp?.Invoke(eventData);
    }
}
