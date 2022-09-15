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
    public TextMeshProUGUI TitleText => titleText;
    public TextMeshProUGUI MessageText => messageText;
    public GenericButton PrimaryButton => primaryButton;
    public bool HasSecondaryButton => hasSecondaryButton;
    public GenericButton SecondaryButton => secondaryButton;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform for the window that shrinks in and out of view")]
    private RectTransform window = null;

    [Space]

    [SerializeField]
    [Tooltip("Image used to create the overlay over all other UI elements")]
    private Image overlay = null;
    [SerializeField]
    [Tooltip("Text used to display the title of the window")]
    private TextMeshProUGUI titleText = null;
    [SerializeField]
    [Tooltip("Text used to display the message of the window")]
    private TextMeshProUGUI messageText = null;
    [SerializeField]
    [Tooltip("Time it takes for the overlay to fade in")]
    private float fadeTime = 0.3f;
    [SerializeField]
    [Tooltip("Time it takes for the window to animate")]
    private float windowAnimateTime = 1f;
    [SerializeField]
    [Tooltip("Anchor position that the window starts at when opened")]
    private Vector2 openingPosition = Vector2.zero;
    [SerializeField]
    [Tooltip("Anchor position that the window rests at after it is opened")]
    private Vector2 restingPosition = Vector2.zero;
    [SerializeField]
    [Tooltip("Easing style used when opening the window")]
    private Ease openingEase = Ease.Unset;
    [SerializeField]
    [Tooltip("Easing style used when closing the window")]
    private Ease closingEase = Ease.Unset;

    [Space]

    [SerializeField]
    [Tooltip("Primary button in the window")]
    private GenericButton primaryButton = null;
    [SerializeField]
    [Tooltip("Determines if this generic window has another button")]
    private bool hasSecondaryButton = false;
    [SerializeField]
    [Tooltip("Secondary button in the window")]
    private GenericButton secondaryButton = null;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        primaryButton.Button.onClick.AddListener(() => Close(primaryButton.ButtonAction.Invoke));
        if (hasSecondaryButton)
        {
            secondaryButton.Button.onClick.AddListener(() => Close(secondaryButton.ButtonAction.Invoke));
        }
    }
    #endregion

    #region Public Methods
    public void AddPrimaryAction(UnityAction action)
    {
        primaryButton.ButtonAction.AddListener(action);
    }
    public void AddSecondaryAction(UnityAction action)
    {
        secondaryButton.ButtonAction.AddListener(action);
    }
    public void Open()
    {
        // Once set up, set the game object to true
        gameObject.SetActive(true);
        window.gameObject.SetActive(false);

        // Start overlay color as clear
        overlay.color = Color.clear;
        overlay.DOColor(Color.black.SetAlpha(0.5f), fadeTime)
            // When the overlay color animation finishes, then do the window motion animation
            .OnComplete(() =>
            {
                window.gameObject.SetActive(true);

                // Animate the position of the window
                window.anchoredPosition = openingPosition;
                window.DOAnchorPos(Vector2.zero, windowAnimateTime).SetEase(openingEase);
            });
    }
    public virtual void Close(UnityAction action = null)
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
                        // If a closing action was supplied then invoke it
                        if (action != null) action.Invoke();
                    });
            });
    }
    #endregion
}
