using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrawingCursor
{
    #region Public Properties
    public bool IsDrawing => CurrentDrawingObject;
    public DrawingCanvas CurrentCanvas { get; private set; }
    public Vector2 StartingPosition { get; private set; }
    public Vector2 CurrentPosition { get; private set; }
    public CanvasDrawingObject CurrentDrawingObject { get; private set; }
    #endregion

    #region Private Editor Fields

    #endregion

    #region Public Methods
    public void StartDrawing(DrawingCanvas canvas, Vector2 startingPosition)
    {
        // If we are still drawing then finish drawing now
        if (IsDrawing) FinishDrawing();

        // Set the current owner of the cursor
        CurrentCanvas = canvas;

        // Get the current drawing object based on the 
        CanvasDrawingObject prefabObject = canvas.CurrentDrawingObject;

        if(prefabObject)
        {
            // Setup the starting position
            StartingPosition = startingPosition;
            CurrentPosition = startingPosition;

            // Instantiate the drawing object under the canvas
            CurrentDrawingObject = Object.Instantiate(prefabObject, canvas.transform);
            CurrentDrawingObject.StartDrawing(this);
        }
    }
    public void UpdateDrawing(Vector2 currentPosition)
    {
        if(IsDrawing)
        {
            CurrentPosition = currentPosition;
            CurrentDrawingObject.UpdateDrawing();
        }
    }
    public void FinishDrawing()
    {
        if(IsDrawing)
        {
            CurrentDrawingObject.FinishDrawing();
            CurrentDrawingObject = null;
        }
    }
    #endregion
}
