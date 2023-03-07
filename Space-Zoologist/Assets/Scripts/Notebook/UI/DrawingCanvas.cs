using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawingCanvas : MonoBehaviour,  IBeginDragHandler, IDragHandler
{
    #region Constants 
    public static readonly int[] STROKE_THICKNESS = { 5, 10, 20 };
    #endregion

    #region Public Typedefs
    public enum StrokeWeight { Small, Medium, Large }
    public enum Mode { Drawing, Erasing }
    #endregion

    #region Private Properties
    private Color DrawingColor => currentMode == Mode.Drawing ? currentColor : backgroundColor;
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
    public int CurrentStrokeThickness => STROKE_THICKNESS[(int)currentWeight];
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform of this canvas")]
    private RectTransform rectTransform = null;
    [SerializeField]
    [Tooltip("Image used to render the drawing material")]
    private RawImage rawImage = null;
    [SerializeField]
    [Tooltip("Background color of the canvas")]
    private Color backgroundColor = Color.white;

    [Space]

    [SerializeField]
    [Tooltip("Current mode of drawing")]
    private Mode currentMode = Mode.Drawing;
    [SerializeField]
    [Tooltip("Current color used for drawing")]
    private Color currentColor = Color.white;
    [SerializeField]
    [Tooltip("Current weight of the lines to draw")]
    private StrokeWeight currentWeight = StrokeWeight.Small;
    #endregion

    #region Private Fields
    private Texture2D drawingTexture = null;
    private Vector2 previousTexturePosition = Vector2.zero;
    private Color32[] clearTextureColorArray;
    #endregion

    #region Public Methods
    public void OnBeginDrag(PointerEventData data)
    {
        if(drawingTexture != null)
        {
            // SummaryManager is backend, should not be used in editor
            SummaryManager summaryManager = (SummaryManager)FindObjectOfType(typeof(SummaryManager));
            if(summaryManager)
                summaryManager.CurrentSummaryTrace.NumDrawToolUsed += 1;
            
            previousTexturePosition = MousePositionToPositionInTexture(data.position);

            // Fill a circle as the cap of this line
            drawingTexture.FillCircle((int)previousTexturePosition.x,
                (int)previousTexturePosition.y,
                CurrentStrokeThickness / 2,
                DrawingColor);
        }
    }
    public void OnDrag(PointerEventData data)
    {
        if(drawingTexture != null)
        {
            Vector2 currentTexturePosition = MousePositionToPositionInTexture(data.position);

            // Draw a line connecting this and the previous position
            drawingTexture.StrokeThickLine((int)previousTexturePosition.x,
                    (int)previousTexturePosition.y,
                    (int)currentTexturePosition.x,
                    (int)currentTexturePosition.y,
                    CurrentStrokeThickness,
                    DrawingColor)
                // Draw a circle to cap this line
                .FillCircle((int)currentTexturePosition.x,
                    (int)currentTexturePosition.y,
                    CurrentStrokeThickness / 2,
                    DrawingColor)
                // Apply the changes
                .Apply();

            // Set previous position to current before continuing
            previousTexturePosition = currentTexturePosition;
        }
    }
    public void Clear()
    {
        if(drawingTexture != null)
        {
            drawingTexture.SetAllPixels(clearTextureColorArray).Apply();
        }
    }
    #endregion

    #region Monobehaviour Messages
    private void OnEnable()
    {
        // When the canvas is enabled, try to initialize the texture
        if (drawingTexture == null)
        {
            StartCoroutine(InitializeTextureOnceRectIsValid());
        }
        else
        {
            // Recreate the clear color array
            CreateClearColorArray();
        }
    }

    private void OnDisable()
    {
        // When not drawing, free up memory
        clearTextureColorArray = null;
    }

    #endregion

    #region Private Methods
    private IEnumerator InitializeTextureOnceRectIsValid()
    {
        // Set the raw image to be clear at first
        rawImage.color = Color.clear;
        // Wait until the rect area is valid. This is necessary because the rect is invalid while
        // we are waiting for Unity's layouting system to update
        yield return new WaitUntil(() => rectTransform.rect.width > 0f && rectTransform.rect.height > 0f);

        // Create the texture
        drawingTexture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        drawingTexture.wrapMode = TextureWrapMode.Clamp;
        
        // Create clear color array to allow for faster clearing later
        CreateClearColorArray();
        
        drawingTexture.SetAllPixels(clearTextureColorArray).Apply();

        rawImage.color = Color.white;
        // Set the texture of the raw image
        rawImage.texture = drawingTexture;

    }

    private void CreateClearColorArray()
    {
        int width = drawingTexture.width;
        int height = drawingTexture.height;

        Color32 bg = backgroundColor;
        clearTextureColorArray = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                clearTextureColorArray[y * width + x] = bg;
            }
        }
    }
    
    private Vector2 MousePositionToPositionInTexture(Vector2 mousePosition)
    {
        // Get the position of the mouse inside the rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, null, out Vector2 localMousePoint);
        
        // Return the final position
        return localMousePoint;
    }
    #endregion
}
