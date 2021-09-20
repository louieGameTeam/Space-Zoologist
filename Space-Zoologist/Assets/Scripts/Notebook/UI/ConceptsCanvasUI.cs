using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class ConceptsCanvasUI : NotebookUIChild
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform that expands and contracts when the canvas folds in/out")]
    private RectTransform foldoutRect;
    [SerializeField]
    [Tooltip("Toggle that expands/collapses the concept canvas")]
    private Toggle foldoutToggle;
    [SerializeField]
    [Tooltip("Time it takes for the canvas to expand/collapse")]
    private float foldoutTime = 0.3f;

    [Space]

    [SerializeField]
    [Tooltip("Reference to the game object at the root of the drawing canvas to enable/disable on foldout")]
    private GameObject drawingCanvasParent;
    [SerializeField]
    [Tooltip("Rect transform attached to the drawing canvas")]
    private RectTransform drawingCanvasRect;
    [SerializeField]
    [Tooltip("Reference to the script that handles drawing on the canvas")]
    private DrawingCanvas drawingCanvas;
    [SerializeField]
    [Tooltip("Script that is used to select a drawing mode for the canvas")]
    private DrawingCanvasModeGroupPicker modePicker;
    [SerializeField]
    [Tooltip("Picker group used to select the color of the drawing canvas")]
    private ColorToggleGroupPicker colorPicker;
    [SerializeField]
    [Tooltip("Script used to select a stroke weight for the canvas")]
    private StrokeWeightGroupPicker strokeWeightPicker;
    [SerializeField]
    [Tooltip("Button used to clear the canvas")]
    private Button clearButton;

    [Space]

    [SerializeField]
    [Tooltip("Reference to the scroll rect that moves the drafting area side to side")]
    private ScrollRect scroll;
    [SerializeField]
    [Tooltip("Orthographic size of the camera when viewing the concept canvas")]
    private float orthographicSize = 20f;
    [SerializeField]
    [Tooltip("Time it takes for the camera to move into zoomed position")]
    private float smoothingTime = 1f;
    [SerializeField]
    [Tooltip("Y-position that the camera moves to at the center of the drawing canvas")]
    private float yCenter = 5f;
    [SerializeField]
    [Tooltip("X-position that the camera moves to when viewing the leftmost part of the enclosure")]
    private float xLeft = -3f;
    #endregion

    #region Monobehaviour Messages
    private void OnEnable()
    {
        GameManager instance = GameManager.Instance;

        if(instance && foldoutToggle.isOn)
        {
            SetCameraPosition(scroll.normalizedPosition);
        }
    }
    private void OnDisable()
    {
        GameManager instance = GameManager.Instance;

        if(instance)
        {
            instance.m_cameraController.Unlock();
        }
    }
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Apply foldout state when toggle state changes
        foldoutToggle.isOn = false;
        foldoutToggle.onValueChanged.AddListener(ApplyFoldoutState);
        ApplyFoldoutState(false);

        // Set the object picked on each picker to whatever the canvas's current setting is
        modePicker.SetObjectPicked(drawingCanvas.CurrentMode);
        strokeWeightPicker.SetObjectPicked(drawingCanvas.CurrentWeight);

        // Set the color to the first in the list
        colorPicker.SetTogglePicked(0);
        drawingCanvas.CurrentColor = colorPicker.FirstObjectPicked;

        // Add listeners for groups that change the canvas parameters
        modePicker.OnToggleStateChanged.AddListener(() => drawingCanvas.CurrentMode = modePicker.FirstObjectPicked);
        colorPicker.OnToggleStateChanged.AddListener(() => drawingCanvas.CurrentColor = colorPicker.FirstObjectPicked);
        strokeWeightPicker.OnToggleStateChanged.AddListener(() => drawingCanvas.CurrentWeight = strokeWeightPicker.FirstObjectPicked);

        // Clear canvas when clear button clicked
        clearButton.onClick.AddListener(drawingCanvas.Clear);
    }
    #endregion

    #region Private Methods
    private void ApplyFoldoutState(bool state)
    {
        // Complete any tweening animations
        foldoutRect.DOKill();

        // Change the anchor to either the far right of the parent or the middle of the parent
        if (state)
        {
            foldoutRect.DOAnchorMax(new Vector2(1f, foldoutRect.anchorMax.y), foldoutTime)
                .OnComplete(() => drawingCanvasParent.gameObject.SetActive(true));

            // Try to get the game manager
            GameManager instance = GameManager.Instance;

            // If we get the game manager then set the camera position
            if(instance)
            {
                scroll.normalizedPosition = Vector2.zero;
                SetCameraPosition(scroll.normalizedPosition);
            }
        }
        else
        {
            drawingCanvasParent.gameObject.SetActive(false);
            foldoutRect.DOAnchorMax(new Vector2(0.5f, foldoutRect.anchorMax.y), foldoutTime);

            // Unlock the camera so it goes back to its previous position
            GameManager instance = GameManager.Instance;
            if (instance) instance.m_cameraController.Unlock();
        }
    }
    private void SetCameraPosition(Vector2 scrollPos)
    {
        Vector3 pos = new Vector3(xLeft, yCenter, GameManager.Instance.m_cameraController.transform.position.z);
        GameManager.Instance.m_cameraController.Lock(new CameraPositionLock(pos, orthographicSize, smoothingTime));
    }
    #endregion
}
