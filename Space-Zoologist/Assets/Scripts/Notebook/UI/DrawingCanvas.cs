using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [SerializeField]
    private int texturePixelSize;
    [SerializeField, Range(0, 1)]
    private float canvasBackgroundTransparency;
    private Texture2D DrawingTexture;
    private Material DrawingMaterial;
    private Color defaultColor;
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

    #region Monobehaviour Callbacks
    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        DrawingTexture = new Texture2D((int)rectTransform.rect.width / texturePixelSize, (int)rectTransform.rect.height / texturePixelSize);
        defaultColor = Color.white;
        defaultColor.a = canvasBackgroundTransparency;
        for (int i = 0; i < DrawingTexture.width; ++i)
        {
            for (int j = 0; j < DrawingTexture.height; ++j)
            {
                DrawingTexture.SetPixel(i, j, defaultColor);
            }
        }
        DrawingTexture.Apply();
        DrawingTexture.filterMode = FilterMode.Point;
        DrawingMaterial = GetComponent<Image>().material;
        DrawingMaterial.SetTexture("_MainTex", DrawingTexture);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // calculate line

            PaintCircleOnTexture(Input.mousePosition, 8, Color.red);
        }
    }

    private void PaintCircleOnTexture(Vector3 mouseCoordinate, float radius, Color color)
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);

        float canvasWidth = corners[2].x - corners[0].x;
        float canvasHeight = corners[2].y - corners[0].y;
        // 0 is bottom left, 2 is top right

        for (int i = 0; i < DrawingTexture.width; ++i)
        {
            for (int j = 0; j < DrawingTexture.height; ++j)
            {
                Vector3 pixelWorldPosition = new Vector3((float)i / DrawingTexture.width * canvasWidth, (float)j / DrawingTexture.height * canvasHeight, 0) + corners[0];

                if (Vector3.Distance(pixelWorldPosition, mouseCoordinate) < radius)
                {
                    DrawingTexture.SetPixel(i, j, color);
                }
            }
        }

        DrawingTexture.Apply();
        DrawingMaterial.SetTexture("_MainTex", DrawingTexture);
    }

    private void EraseCircleOnTexture(Vector3 mouseCoordinate, float radius, Color color)
    {
        Vector3[] corners = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(corners);

        float canvasWidth = corners[2].x - corners[0].x;
        float canvasHeight = corners[2].y - corners[0].y;
        // 0 is bottom left, 2 is top right

        for (int i = 0; i < DrawingTexture.width; ++i)
        {
            for (int j = 0; j < DrawingTexture.height; ++j)
            {
                Vector3 pixelWorldPosition = new Vector3((float)i / DrawingTexture.width * canvasWidth, (float)j / DrawingTexture.height * canvasHeight, 0) + corners[0];

                if (Vector3.Distance(pixelWorldPosition, mouseCoordinate) < radius)
                {
                    DrawingTexture.SetPixel(i, j, defaultColor);
                }
            }
        }

        DrawingTexture.Apply();
        DrawingMaterial.SetTexture("_MainTex", DrawingTexture);
    }

    private void EraseAllColorsOnTexture()
    {
        for (int i = 0; i < DrawingTexture.width; ++i)
        {
            for (int j = 0; j < DrawingTexture.height; ++j)
            {
                DrawingTexture.SetPixel(i, j, defaultColor);
            }
        }

        DrawingTexture.Apply();
        DrawingMaterial.SetTexture("_MainTex", DrawingTexture);
    }

    private void LateUpdate()
    {
    }
    #endregion
}
