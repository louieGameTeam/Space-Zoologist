﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [Tooltip("Reference to the script that handles drawing on the canvas")]
    private DrawingCanvas drawingCanvas;
    [SerializeField]
    [Tooltip("Picker group used to select the color of the drawing canvas")]
    private ColorToggleGroupPicker colorPicker;
    #endregion

    #region Monobehaviour Messages
    public override void Setup()
    {
        base.Setup();
        // Apply foldout state when toggle state changes
        foldoutToggle.isOn = false;
        foldoutToggle.onValueChanged.AddListener(ApplyFoldoutState);
        ApplyFoldoutState(false);
        // Add listeners for groups that change the canvas parameters
        colorPicker.OnToggleStateChanged.AddListener(() =>
        {
            if(colorPicker.ObjectsPicked.Count > 0)
            {
                drawingCanvas.CurrentColor = colorPicker.ObjectsPicked[0];
            }
        });
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
                .OnComplete(() => drawingCanvas.gameObject.SetActive(true));
        }
        else
        {
            drawingCanvas.gameObject.SetActive(false);
            foldoutRect.DOAnchorMax(new Vector2(0.5f, foldoutRect.anchorMax.y), foldoutTime);
        }
    }
    #endregion
}
