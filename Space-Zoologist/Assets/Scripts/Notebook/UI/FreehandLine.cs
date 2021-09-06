using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreehandLine : CanvasDrawingObject
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Line renderer used to create the line")]
    private TrailRenderer trail;
    #endregion

    #region Public Methods
    public override void StartDrawing(DrawingCursor cursor)
    {
        // Call the base method
        base.StartDrawing(cursor);

        // Set the position to the position of the cursor
        transform.position = cursor.CurrentPosition;

        // Set color and thickness of the trail
        trail.startWidth = trail.endWidth = cursor.CurrentCanvas.CurrentStrokeThickness;
        trail.startColor = trail.endColor = cursor.CurrentCanvas.CurrentColor;
    }
    public override void UpdateDrawing()
    {
        transform.position = CurrentCursor.CurrentPosition;
    }
    public override void FinishDrawing()
    {
        // Do nothing at all
    }
    #endregion
}
