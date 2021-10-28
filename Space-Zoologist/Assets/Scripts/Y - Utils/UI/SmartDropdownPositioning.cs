using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Treats the game object as a dropdown and tries to drive the anchors and
/// position of the object in the parent so that it the game view
/// does not cut it off
/// </summary>
public class SmartDropdownPositioning : UIBehaviour
{
    #region Private Properties
    private RectTransform RectTransform => transform as RectTransform;
    #endregion

    #region Private Methods
    private void RecalculatePosition()
    {
        // Get the world corners of the rect transform
        Vector3[] corners = new Vector3[4];
        RectTransform.GetWorldCorners(corners);

        // Try to get a canvas in the parent
        Canvas canvas = GetComponentInParent<Canvas>();

        if(canvas)
        {
            // Get the rect transform from the canvas
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

            // Get the world corners of the canvas
            Vector3[] canvasCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);

            // Too far off to the left
            if(corners[0].x < canvasCorners[0].x)
            {
                // Anchor it to the left of the parent
                RectTransform.anchorMin = new Vector2(0, RectTransform.anchorMin.y);
                RectTransform.anchorMax = new Vector2(0, RectTransform.anchorMin.y);

                // Pivot around my left side
                RectTransform.pivot = new Vector2(0, RectTransform.pivot.y);
            }
            // Too far up
            if(corners[1].y > canvasCorners[1].y)
            {
                // Anchor it to the bottom of the parent
                RectTransform.anchorMin = new Vector2(RectTransform.anchorMin.x, 0);
                RectTransform.anchorMax = new Vector2(RectTransform.anchorMin.x, 0);

                // Pivot around my top side
                RectTransform.pivot = new Vector2(RectTransform.pivot.x, 1);
            }
            // Too far off to the right
            if(corners[2].x > canvasCorners[2].x)
            {
                // Anchor it to the right of the parent
                RectTransform.anchorMin = new Vector2(1, RectTransform.anchorMin.y);
                RectTransform.anchorMax = new Vector2(1, RectTransform.anchorMin.y);

                // Pivot around my right side
                RectTransform.pivot = new Vector2(1, RectTransform.pivot.y);
            }
            // Too far down
            if (corners[3].y < canvasCorners[3].y)
            {
                // Anchor it to the top of the parent
                RectTransform.anchorMin = new Vector2(RectTransform.anchorMin.x, 1);
                RectTransform.anchorMax = new Vector2(RectTransform.anchorMin.x, 1);

                // Pivot around my bottom side
                RectTransform.pivot = new Vector2(RectTransform.pivot.x, 0);
            }

            // Make sure the anchored position is at the pivot
            RectTransform.anchoredPosition = Vector2.zero;
        }
    }
    #endregion
}
