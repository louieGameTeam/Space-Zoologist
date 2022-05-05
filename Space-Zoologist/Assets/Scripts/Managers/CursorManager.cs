using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D CursorIconDefault;
    public Texture2D CursorIconInspector;
    public Vector2 CursorHotspot;

    void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.InspectorToggled, (eventData) => {
                bool inspecting = (bool) eventData;
                Cursor.SetCursor((inspecting ? CursorIconInspector : null), inspecting ? CursorHotspot : Vector2.zero, CursorMode.Auto);
                });
    }
}
