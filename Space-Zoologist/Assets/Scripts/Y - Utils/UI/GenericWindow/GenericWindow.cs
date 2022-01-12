using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;
using DG.Tweening;

public class GenericWindow : MonoBehaviour
{
    #region Public Properties
    public RectTransform Window => window;
    public Button PrimaryButton => primaryButton;
    public bool HasSecondaryButton => hasSecondaryButton;
    public Button SecondaryButton => secondaryButton;
    public UnityEvent WindowClosedEvent => windowClosedEvent;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform for the window that shrinks in and out of view")]
    private RectTransform window;

    [Space]

    [SerializeField]
    [Tooltip("Image used to create the overlay over all other UI elements")]
    private Image overlay;
    [SerializeField]
    [Tooltip("Time it takes for the overlay to fade in")]
    private float fadeTime = 0.3f;
    [SerializeField]
    [Tooltip("Time it takes for the window to animate")]
    private float windowAnimateTime = 1f;
    [SerializeField]
    [Tooltip("Anchor position that the window starts at when opened")]
    private Vector2 openingPosition;
    [SerializeField]
    [Tooltip("Anchor position that the window rests at after it is opened")]
    private Vector2 restingPosition;
    [SerializeField]
    [Tooltip("Easing style used when opening the window")]
    private Ease openingEase;
    [SerializeField]
    [Tooltip("Easing style used when closing the window")]
    private Ease closingEase;

    [Space]

    [SerializeField]
    [Tooltip("Primary button in the window")]
    private Button primaryButton;
    [SerializeField]
    [Tooltip("Determines if this generic window has another button")]
    private bool hasSecondaryButton;
    [SerializeField]
    [Tooltip("Secondary button in the window")]
    private Button secondaryButton;
    [SerializeField]
    [Tooltip("Event invoked after the window has finished the closing animation")]
    private UnityEvent windowClosedEvent;
    #endregion

    #region Private Fields
    private static string defaultPrefabName => nameof(GenericWindow);
    private static string defaultPrefabPath => defaultPrefabName;
    #endregion

    #region Public Methods
    public void Open(UnityAction primaryAction, UnityAction secondaryAction = null)
    {
        // Once set up, set the game object to true
        gameObject.SetActive(true);
        window.gameObject.SetActive(false);

        // Make the primary button close the window and finish with the primary action
        primaryButton.onClick.AddListener(() => Close(primaryAction));

        // If a secondary action was specified and we have a secondary button then
        // make the secondary button close with the secondary action
        if (secondaryAction != null && hasSecondaryButton)
        {
            secondaryButton.onClick.AddListener(() => Close(secondaryAction));
        }

        // Start overlay color as clear
        overlay.color = Color.clear;
        overlay.DOColor(Color.black.SetAlpha(0.5f), fadeTime)
            // When the overlay color animation finishes, then do the window motion animation
            .OnComplete(() =>
            {
                window.gameObject.SetActive(true);
                window.anchoredPosition = openingPosition;
                window.DOAnchorPos(Vector2.zero, windowAnimateTime).SetEase(openingEase);
            });
    }
    public void Close(UnityAction closeAction = null)
    {
        // Animate the window back to the starting anchor
        window.DOAnchorPos(openingPosition, windowAnimateTime)
            // Set the ease for the ending animation
            .SetEase(closingEase)
            // When the animation finishes, then animate the overlay
            .OnComplete(() =>
            {
                // Disable the window and animate the overlay color
                window.gameObject.SetActive(false);
                overlay.DOColor(Color.clear, fadeTime)
                    // When the overlay color finishes, then disable the whole object
                    // and invoke the window closed event
                    .OnComplete(() => {
                        gameObject.SetActive(false);
                        if (closeAction != null) closeAction.Invoke();
                        windowClosedEvent.Invoke();
                    });
            });
    }
    #endregion
}
