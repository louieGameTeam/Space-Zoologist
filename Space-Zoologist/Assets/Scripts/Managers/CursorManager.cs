using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D CursorIconDefault;
    public Texture2D CursorIconInspector;

    void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.InspectorToggled, () => {
                Cursor.SetCursor(((bool) EventManager.Instance.EventData ? CursorIconInspector : null), Vector2.zero, CursorMode.Auto);
                });
    }
}
