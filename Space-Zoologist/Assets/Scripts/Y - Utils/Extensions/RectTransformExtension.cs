using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtension
{
    public static void SetOffsets(this RectTransform rectTransform, RectOffset offset)
    {
        rectTransform.offsetMin = new Vector2(offset.left, offset.bottom);
        rectTransform.offsetMax = new Vector2(offset.right, offset.top);
    }
}
