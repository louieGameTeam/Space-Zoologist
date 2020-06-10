using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class CursorItem : MonoBehaviour, IPointerClickHandler
{
    public delegate void ClickHandler(PointerEventData eventData);
    public event ClickHandler onClick;

    public Sprite Sprite => image.sprite;
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

    public void Begin(Sprite sprite, ClickHandler clickHandler)
    {
        image.enabled = true;
        image.sprite = sprite;
        onClick += clickHandler;
    }

    public void Stop(ClickHandler clickHandler)
    {
        onClick -= clickHandler;
        image.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(eventData);
        }
    }
}
