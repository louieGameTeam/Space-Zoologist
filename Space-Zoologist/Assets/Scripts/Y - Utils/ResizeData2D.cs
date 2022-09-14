using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResizeData2D
{
    public enum ResizeType
    {
        Exact, PreserveAspectHorizontal, PreserveAspectVertical
    }

    [SerializeField]
    [Tooltip("Type of resize to be performed")]
    private ResizeType type = ResizeType.Exact;
    [SerializeField]
    [Tooltip("New width of the object")]
    private float width = 1.0f;
    [SerializeField]
    [Tooltip("New height of the object")]
    private float height = 1.0f;

    public Vector2 Resize(Vector2 originalSize)
    {
        return Resize(originalSize.x, originalSize.y);
    }
    public Vector2 Resize(float oldWidth, float oldHeight)
    {
        return Resize(oldWidth / oldHeight);
    }
    public Vector2 Resize(float aspect)
    {
        switch(type)
        {
            case ResizeType.Exact: return new Vector2(width, height);
            case ResizeType.PreserveAspectHorizontal: return new Vector2(width, width * (1f / aspect));
            case ResizeType.PreserveAspectVertical: return new Vector2(height * aspect, width);
            default: return new Vector2(width, height);
        }
    }
}
