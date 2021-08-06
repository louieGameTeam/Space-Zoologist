using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CursorTexture
{
    // The exact hotspot on the texture
    public Vector2 Hotspot
    {
        get
        {
            if (texture != null) return new Vector2(texture.width * hotspotAnchor.x, texture.height * hotspotAnchor.y);
            else return Vector2.zero;
        }
    }

    [SerializeField]
    [Tooltip("Reference to the texture that displays on the cursor")]
    private Texture2D texture;
    [SerializeField]
    [Tooltip("Hotspot of this cursor, ranges from [0, 0] at the top left to [1, 1] at the bottom right")]
    private Vector2 hotspotAnchor = Vector2.zero;
    [SerializeField]
    [Tooltip("Cursor mode for this cursor texture")]
    private CursorMode mode = CursorMode.Auto;

    // Set the current cursor to this cursor
    public void SetCursor()
    {
        Cursor.SetCursor(texture, Hotspot, mode);
    }
}
