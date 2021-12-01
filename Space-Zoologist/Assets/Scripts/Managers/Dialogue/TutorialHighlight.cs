using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class TutorialHighlight : MonoBehaviour
{
    #region Public Properties
    public bool IsAnimating => animationRoutine != null;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform at the root of the highlight object")]
    private RectTransform root;
    [SerializeField]
    [Tooltip("Reference to the image that displays the pointing arrow")]
    private Image pointer;
    [SerializeField]
    [Tooltip("Panel that highlights over the item the user should click")]
    private Image highlightPanel;

    [Space]

    [SerializeField]
    [Tooltip("Distance that the pointer moves during the animation")]
    private float pointerMoveDistance = 100f;
    [SerializeField]
    [Tooltip("Scale that the highlight panel grows up to during the animation")]
    private float highlightGrowScale = 2f;
    [SerializeField]
    [Tooltip("Time it takes for the highlight to focus on the object")]
    private float focusTime = 0.3f;
    [SerializeField]
    [Tooltip("Time that the effect pauses in the focus position before resuming")]
    private float pauseTime = 1f;

    [Space]

    [SerializeField]
    [Tooltip("If true, the animation plays as soon as it starts")]
    private bool playOnAwake = false;
    #endregion

    #region Private Fields
    private Coroutine animationRoutine;
    private float highlightPanelAlpha = -1f;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        if (playOnAwake) StartAnimating();
    }
    #endregion

    #region Public Methods
    public void Highlight(RectTransform target)
    {
        // Set the root as a child of the target
        root.SetParent(target);
        root.SetAsLastSibling();

        // Stretch across the whole parent
        root.anchorMax = Vector2.one;
        root.anchorMin = root.offsetMin = root.offsetMax = Vector2.zero;

        // Start animating the highlight
        StartAnimating();
    }
    public void StartAnimating()
    {
        // If we are still animating then stop
        if (IsAnimating) StopAnimating();

        // Set the positions of the rect transforms
        pointer.rectTransform.anchoredPosition = new Vector2(-1, 1) * pointerMoveDistance;
        highlightPanel.rectTransform.localScale = Vector3.one * highlightGrowScale;

        // Set them to invisible before starting the animation
        pointer.color = pointer.color.SetAlpha(0f);
        // Store original panel alpha so it can be restored later
        if (highlightPanelAlpha < 0) highlightPanelAlpha = highlightPanel.color.a;
        highlightPanel.color = highlightPanel.color.SetAlpha(0f);

        // Start the highlight animation
        animationRoutine = StartCoroutine(HighlightAnimation());
    }
    public void StopAnimating()
    {
        if(IsAnimating)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }
    }
    #endregion

    #region Private Methods
    private IEnumerator HighlightAnimation()
    {
        while(true)
        {
            // Animate pointer position and color
            pointer.rectTransform.DOAnchorPos(Vector2.zero, focusTime);
            pointer.DOColor(pointer.color.SetAlpha(1f), focusTime);

            // Animate highlight panel scale and color
            highlightPanel.rectTransform.DOScale(Vector3.one, focusTime);
            yield return highlightPanel.DOColor(highlightPanel.color.SetAlpha(highlightPanelAlpha), focusTime).WaitForCompletion();

            // Pause for the specified time
            yield return new WaitForSeconds(pauseTime);

            // Animate pointer position and color
            pointer.rectTransform.DOAnchorPos(new Vector2(-1, 1f) * pointerMoveDistance, focusTime);
            pointer.DOColor(pointer.color.SetAlpha(0f), focusTime);

            // Animate highlight panel scale and color
            highlightPanel.rectTransform.DOScale(Vector3.one * highlightGrowScale, focusTime);
            yield return highlightPanel.DOColor(highlightPanel.color.SetAlpha(0f), focusTime).WaitForCompletion();
        }
    }
    #endregion
}
