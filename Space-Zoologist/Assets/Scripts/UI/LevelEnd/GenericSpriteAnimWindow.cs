using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Similar to generic window, but sprite and animator based
/// </summary>
public class GenericSpriteAnimWindow : MonoBehaviour
{
    #region Public Properties
    public RectTransform Window => window;
    public GenericButton PrimaryButton => primaryButton;
    public bool HasSecondaryButton => hasSecondaryButton;
    public GenericButton SecondaryButton => secondaryButton;
    public Animator Animator => animator;
    
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform for the window that shrinks in and out of view")]
    private RectTransform window = null;

    [SerializeField]
    private UnityEvent OnWindowOpened;

    [Space]

    [SerializeField]
    [Tooltip("Image used to create the overlay over all other UI elements")]
    private Image overlay = null;
    [SerializeField]
    [Tooltip("Time it takes for the overlay to fade in")]
    private float fadeTime = 0.3f;
    [SerializeField]
    [Tooltip("Animator for transitions")]
    private Animator animator = null;

    [SerializeField]
    [Tooltip("Name of animator param to trigger on show")]
    private string triggerParam;
    
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
    
    public void Open(Action openDelegate = null)
    {
        // Once set up, set the game object to true
        gameObject.SetActive(true);
        window.gameObject.SetActive(false);

        // Start overlay color as clear
        overlay.color = Color.clear;
        overlay.DOColor(Color.black.SetAlpha(0.5f), fadeTime)
            // When the overlay color animation finishes, then show the window
            .OnComplete(() =>
            {
                window.gameObject.SetActive(true);
                openDelegate?.Invoke();
                animator.SetTrigger(triggerParam);
            });
        
        OnWindowOpened?.Invoke();
    }
    
    public virtual void Close(UnityAction action = null)
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
    }
    #endregion
}
