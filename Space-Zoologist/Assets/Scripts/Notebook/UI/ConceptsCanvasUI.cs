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
    #endregion

    #region Monobehaviour Messages
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
        }
        else
        {
            drawingCanvasParent.gameObject.SetActive(false);
            foldoutRect.DOAnchorMax(new Vector2(0.5f, foldoutRect.anchorMax.y), foldoutTime);
        }
    }
    #endregion
}
