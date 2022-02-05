using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GenericFoldout : MonoBehaviour
{
    #region Public Typedefs
    [System.Serializable]
    public class FoldoutAnchor
    {
        public Vector2 anchor;
        public Vector2 sizeDelta;

        public Tween Apply(RectTransform rectTransform, float time)
        {
            rectTransform.DOKill();
            rectTransform.DOAnchorMax(anchor, time);
            return rectTransform.DOSizeDelta(sizeDelta, time);
        }
    }
    #endregion

    #region Public Properties
    public Toggle FoldoutToggle => foldoutToggle;
    public bool IsExpanded => foldoutToggle.isOn;
    #endregion

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
    [SerializeField]
    [Tooltip("Anchors of the rect transform when the canvas is folded out")]
    private FoldoutAnchor foldoutAnchors;
    [SerializeField]
    [Tooltip("Anchors of the rect transform when the canvas is folded in")]
    private FoldoutAnchor foldinAnchors;
    [SerializeField]
    [Tooltip("Content of foldout")]
    private GameObject content;
    #endregion

    #region Monobehaviour Messages

    private void Awake()
    {
        // Apply foldout state when toggle state changes
        foldoutToggle.onValueChanged.AddListener(ApplyFoldoutState);
        ApplyFoldoutState(foldoutToggle.isOn);
    }
    #endregion

    #region Private Methods
    private void ApplyFoldoutState(bool state)
    {
        // Change the anchor to either the far right of the parent or the middle of the parent
        if (state)
        {
            foldoutAnchors.Apply(foldoutRect, foldoutTime).OnComplete(() => content.SetActive(true));
        }
        else
        {
            content.SetActive(false);
            foldinAnchors.Apply(foldoutRect, foldoutTime);
        }
    }
    #endregion

    [ContextMenu("Toggle")]
    private void DebugToggle()
    {
        foldoutToggle.isOn = !foldoutToggle.isOn;
    }
}
