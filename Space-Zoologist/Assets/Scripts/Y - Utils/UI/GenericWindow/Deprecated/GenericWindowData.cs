using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class GenericWindowData
{
    #region Public Properties
    public Sprite Background => background;
    public string Message => message;
    public Vector2 StartingAnchorPosition => startingAnchorPosition;
    public Ease StartingAnimationEase => startingAnimationEase;
    public Ease EndingAnimationEase => endingAnimationEase;
    public GenericButtonData PrimaryButtonData => primaryButtonData;
    public bool HasSecondaryButton => hasSecondaryButton;
    public GenericButtonData SecondaryButtonData => secondaryButtonData;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Image used for the background of the generic window")]
    private Sprite background;
    [SerializeField]
    [Tooltip("Text to display in the window")]
    private string message;

    [Space]

    [SerializeField]
    [Tooltip("Starting position for the window animation")]
    private Vector2 startingAnchorPosition;
    [SerializeField]
    [Tooltip("Ease of the window animation")]
    private Ease startingAnimationEase;
    [SerializeField]
    [Tooltip("Ease of the window when it goes away")]
    private Ease endingAnimationEase;
    
    [Space]
    
    [SerializeField]
    [Tooltip("Button data for the primary button in the window")]
    private GenericButtonData primaryButtonData;
    [SerializeField]
    [Tooltip("True if the window has a secondary button and false if it should be hidden")]
    private bool hasSecondaryButton = true;
    [SerializeField]
    [Tooltip("Button data for the secondary button in the window")]
    private GenericButtonData secondaryButtonData;
    #endregion
}
