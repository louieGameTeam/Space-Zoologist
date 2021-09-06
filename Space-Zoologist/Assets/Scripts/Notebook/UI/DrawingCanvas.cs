using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingCanvas : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Constants 
    public static readonly float[] STROKE_THICKNESS = { 50, 100, 200 };
    #endregion

    #region Public Typedefs
    public enum StrokeWeight { Small, Medium, Large }
    public enum Mode { Drawing, Erasing }
    #endregion

    #region Public Properties
    public Mode CurrentMode 
    {
        get => currentMode;
        set => currentMode = value;
    }
    public Color CurrentColor
    {
        get => currentColor;
        set => currentColor = value;
    }
    public StrokeWeight CurrentWeight
    {
        get => currentWeight;
        set => currentWeight = value;
    }
    // List of drawing objects matching the "currentMode"
    public CanvasDrawingObject[] DrawingObjects => new CanvasDrawingObject[2] { freehandLinePrefab, null };
    // Current drawing object based on "currentMode"
    public CanvasDrawingObject CurrentDrawingObject => DrawingObjects[(int)currentMode];
    public float CurrentStrokeThickness => STROKE_THICKNESS[(int)currentWeight];
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the cursor used to draw objects")]
    private DrawingCursor cursor = new DrawingCursor();
    [SerializeField]
    [Tooltip("Reference to the prefab to create for freehand lines")]
    private FreehandLine freehandLinePrefab;

    [Space]

    [SerializeField]
    [Tooltip("Current mode of drawing")]
    private Mode currentMode;
    [SerializeField]
    [Tooltip("Current color used for drawing")]
    private Color currentColor;
    [SerializeField]
    [Tooltip("Current weight of the lines to draw")]
    private StrokeWeight currentWeight;
    #endregion

    #region Public Methods
    public void OnBeginDrag(PointerEventData data)
    {
        Debug.Log("Started dragging");
        cursor.StartDrawing(this, data.position);
    }
    public void OnDrag(PointerEventData data)
    {
        cursor.UpdateDrawing(data.position);
    }
    public void OnEndDrag(PointerEventData data)
    {
        cursor.FinishDrawing();
    }
    #endregion
}
