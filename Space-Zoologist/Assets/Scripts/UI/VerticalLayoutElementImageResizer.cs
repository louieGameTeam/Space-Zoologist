using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class VerticalLayoutElementImageResizer : UIBehaviour
{

    [SerializeField]
    [Tooltip("Image component")]
    private Image image = null;
    [SerializeField]
    [Tooltip("Layout element component that will be adjusted")]
    private LayoutElement layoutElement = null;
    [SerializeField]
    [Tooltip("Rect transform to be referenced")]
    private RectTransform rectTransformRef = null;

    private float prevSizeDelta_x = 0;

    //Self-readjusts sizing when dimensions change(the idea is for this script to be standalone)
    protected override void OnRectTransformDimensionsChange()
    {
        ResizeImageToFill(rectTransformRef);
    }

    public void ResizeImageToFill(RectTransform sizeRef)
    {
        //Don't bother if the sizeDelta X hasn't changed
        if (sizeRef.sizeDelta.x == prevSizeDelta_x)
            return;
        Sprite sprite = image.sprite;
        if (sprite == null)
            return;
        //Uses the minHeight property of layoutElement to force the image to fill to max size
        //Changing minWidth doesn't work with vertical layouts
        float spriteAspectRatio = sprite.bounds.size.y / sprite.bounds.size.x;
        float minImageHeight = sizeRef.sizeDelta.x * spriteAspectRatio;
        layoutElement.minHeight = minImageHeight;
        prevSizeDelta_x = sizeRef.sizeDelta.x;
    }
}
