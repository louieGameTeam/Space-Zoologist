using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionalResizeData2D
{
    // Public accessors
    public bool WillResize => willResize;

    [SerializeField]
    [Tooltip("Determine if the resize will take place or not")]
    private bool willResize;
    [SerializeField]
    [Tooltip("Data used to resize")]
    private ResizeData2D resizeData;

    public Vector2 Resize(Vector2 oldSize)
    {
        return Resize(oldSize.x, oldSize.y);
    }
    public Vector2 Resize(float oldWidth, float oldHeight)
    {
        if (willResize) return resizeData.Resize(oldWidth, oldHeight);
        else return new Vector2(oldWidth, oldHeight);
    }
}
