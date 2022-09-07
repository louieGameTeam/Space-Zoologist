using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class TutorialHighlight : MonoBehaviour
{
    #region Public Properties
    public RectTransform Root => root;
    public bool IsAnimating => animationRoutine != null;
    /// <summary>
    /// Return the corner that the pointer should be in
    /// x = -1 for left side, x = +1 for right side,
    /// y = -1 for bottom side, y = +1 for top side
    /// </summary>
    public Vector2 PointerCorner
    {
        get
        {
            // Default corner is upper left
            Vector2 corner = new Vector2(-1f, 1f);

            // Get world corners of the root rect
            Vector3[] worldCorners = new Vector3[4];
            root.GetWorldCorners(worldCorners);

            // Compute the space that the pointer needs to comfortably fit horizontally or vertically on screen
            float requiredHorizontalSpace = pointerMoveDistance + pointer.rectTransform.rect.width;
            float requiredVerticalSpace = pointerMoveDistance + pointer.rectTransform.rect.height;

            // If bottom left corner is too far off to the left,
            // then the pointer needs to point to the right side of the highlight
            if (worldCorners[0].x < requiredHorizontalSpace) corner.x = 1f;
            // If the top right corner is too far up,
            // then the pointer needs to point to the bottom side of the highlight
            if (worldCorners[2].y > Screen.height - requiredVerticalSpace) corner.y = -1f;

            return corner;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform at the root of the highlight object")]
    private RectTransform root = null;
    [SerializeField]
    [Tooltip("Reference to the image that displays the pointing arrow")]
    private Image pointer = null;
    [SerializeField]
    [Tooltip("Panel that highlights over the item the user should click")]
    private Image highlightPanel = null;

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
    #endregion

    #region Private Fields
    private Coroutine animationRoutine;
    private float highlightPanelAlpha = -1f;
    #endregion

    #region Monobehaviour Messages
    private void OnEnable()
    {
        StartAnimating();
    }
    private void OnDisable()
    {
        StopAnimating();
    }
    #endregion

    #region Public Methods
    public void Target(RectTransform target)
    {
        // Set the root as a child of the target
        root.SetParent(target);
        root.SetAsLastSibling();
        root.localScale = Vector3.one;

        // Stretch across the whole parent
        root.anchorMax = Vector2.one;
        root.anchorMin = root.offsetMin = root.offsetMax = Vector2.zero;

        // Set the anchor of the pointer to the correct part of the parent
        float anchorX = PointerCorner.x < 0f ? 0.1f : 0.9f;
        float anchorY = PointerCorner.y < 0f ? 0.1f : 0.9f;
        pointer.rectTransform.anchorMin = pointer.rectTransform.anchorMax = new Vector2(anchorX, anchorY);

        // Set the scale so the pointer flips based on what corner it is pointing to
        pointer.rectTransform.localScale = new Vector3(-PointerCorner.x, PointerCorner.y, 1f);

        // Start animating on the target
        StartAnimating();
    }
    public void StartAnimating()
    {
        // If we are still animating then stop
        if (IsAnimating) StopAnimating();

        // Set the positions of the rect transforms
        pointer.rectTransform.anchoredPosition = PointerCorner * pointerMoveDistance;
        highlightPanel.rectTransform.localScale = Vector3.one * highlightGrowScale;

        // Set them to invisible before starting the animation
        pointer.color = pointer.color.SetAlpha(0f);
        // Store original panel alpha so it can be restored later
        if (highlightPanelAlpha < 0) highlightPanelAlpha = highlightPanel.color.a;
        highlightPanel.color = highlightPanel.color.SetAlpha(0f);

        // Start the highlight animation
        root.gameObject.SetActive(true);
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
            pointer.rectTransform.DOAnchorPos(PointerCorner * pointerMoveDistance, focusTime);
            pointer.DOColor(pointer.color.SetAlpha(0f), focusTime);

            // Animate highlight panel scale and color
            highlightPanel.rectTransform.DOScale(Vector3.one * highlightGrowScale, focusTime);
            yield return highlightPanel.DOColor(highlightPanel.color.SetAlpha(0f), focusTime).WaitForCompletion();
        }
    }
    #endregion
}
