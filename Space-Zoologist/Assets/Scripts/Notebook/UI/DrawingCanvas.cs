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
    private RectTransform rectTransform;
    [SerializeField]
    [Tooltip("Image used to render the drawing material")]
    private RawImage rawImage;
    [SerializeField]
    [Tooltip("Background color of the canvas")]
    private Color backgroundColor;

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

    #region Private Fields
    private Texture2D drawingTexture = null;
    private Vector2 previousTexturePosition = Vector2.zero;
    #endregion

    #region Public Methods
    public void OnBeginDrag(PointerEventData data)
    {
        if(drawingTexture != null)
        {
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
            drawingTexture.SetAllPixels(backgroundColor).Apply();
        }
    }
    #endregion

    #region Monobehaviour Messages
    private void OnEnable()
    {
        // When the canvas is enabled, try to initialize the texture
        if (drawingTexture == null)
        {
            StartCoroutine(WaitForValidRect());
        }
    }
    #endregion

    #region Private Methods
    private IEnumerator WaitForValidRect()
    {
        // Wait until the rect area is valid. This is necessary because the rect is invalid while
        // we are waiting for Unity's layouting system to update
        yield return new WaitUntil(() => rectTransform.rect.width > 0f && rectTransform.rect.height > 0f);

        // Create the texture
        drawingTexture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        drawingTexture.wrapMode = TextureWrapMode.Clamp;
        drawingTexture.SetAllPixels(backgroundColor).Apply();

        // Set the texture of the raw image
        rawImage.texture = drawingTexture;
    }
    private Vector2 MousePositionToPositionInTexture(Vector2 mousePosition)
    {
        // Get the position of the mouse inside the rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, null, out Vector2 localMousePoint);
        
        // Move mouse point to correct point inside texture
        localMousePoint += rectTransform.rect.size / 2f;

        // Return the final position
        return localMousePoint;
    }
    #endregion
}
