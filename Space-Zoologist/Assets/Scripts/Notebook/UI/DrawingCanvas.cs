using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawingCanvas : MonoBehaviour, IDragHandler
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
    private Material drawingMaterial;
    #endregion

    #region Public Methods
    public void OnDrag(PointerEventData data)
    {
        // Get the position of the mouse inside the rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, null, out Vector2 localMousePoint);

        // Transform local mouse point to the bottom left, instead of measuring from center
        localMousePoint -= rectTransform.rect.size / 2f;

        // Draw a circle at this position
        drawingTexture.FillCircle((int)localMousePoint.x, (int)localMousePoint.y, CurrentStrokeThickness, CurrentColor);
        drawingTexture.Apply();
    }
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Create a new texture to use for freehand drawings
        drawingTexture = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
        for (int i = 0; i < drawingTexture.width; ++i)
        {
            for (int j = 0; j < drawingTexture.height; ++j)
            {
                drawingTexture.SetPixel(i, j, backgroundColor);
            }
        }
        drawingTexture.filterMode = FilterMode.Point;
        drawingTexture.Apply();

        // Get the material on the image and set the texture of the material
        GetComponent<Image>().material.SetTexture("_MainTex", drawingTexture);
    }
    #endregion
}
