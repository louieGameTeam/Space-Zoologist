using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawingCanvas : MonoBehaviour,  IBeginDragHandler, IDragHandler
{
    #region Constants 
    public static readonly int[] STROKE_THICKNESS = { 10, 20, 40 };
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
    public int CurrentStrokeThickness => STROKE_THICKNESS[(int)currentWeight];
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform of this canvas")]
    private RectTransform rectTransform;
    [SerializeField]
    [Tooltip("Image used to render the drawing material")]
    private Image image;
    [SerializeField]
    [Tooltip("Material to draw on")]
    private Material drawingMaterial;
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
    private Texture2D drawingTexture;
    private Vector2 previousTexturePosition = Vector2.zero;
    #endregion

    #region Public Methods
    public void OnBeginDrag(PointerEventData data)
    {
        previousTexturePosition = MousePositionToPositionInTexture(data.position);
    }
    public void OnDrag(PointerEventData data)
    {
        Vector2 currentTexturePosition = MousePositionToPositionInTexture(data.position);

        // Draw a circle at this position
        drawingTexture.StrokeThickLine((int)previousTexturePosition.x, 
            (int)previousTexturePosition.y, 
            (int)currentTexturePosition.x, 
            (int)currentTexturePosition.y, 
            CurrentStrokeThickness, 
            currentColor);
        drawingTexture.Apply();

        // Set previous position to current before continuing
        previousTexturePosition = currentTexturePosition;
    }
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Make the image render the material
        image.material = drawingMaterial;
        // Create a new drawing texture
        drawingTexture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        drawingTexture.SetAllPixels(backgroundColor);
        drawingTexture.Apply();
        // Make the material show the texture that will actually be modified
        drawingMaterial.SetTexture("_MainTex", drawingTexture);
    }
    #endregion

    #region Private Methods
    private Vector2 MousePositionToPositionInTexture(Vector2 mousePosition)
    {
        // Get the position of the mouse inside the rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, null, out Vector2 localMousePoint);

        // Move mouse point to correct place in texture
        return localMousePoint - rectTransform.rect.size / 2f;
    }
    #endregion
}
