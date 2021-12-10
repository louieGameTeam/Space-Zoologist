using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericWindowData
{
    #region Public Properties
    public Sprite Background => background;
    public string Message => message;
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

    #region Constructors
    public GenericWindowData(string message, GenericButtonData buttonData) : this(message, buttonData, null) 
    {
        hasSecondaryButton = false;
    }
    public GenericWindowData(string message, GenericButtonData primaryButtonData, GenericButtonData secondaryButtonData)
    {
        this.message = message;
        this.primaryButtonData = primaryButtonData;
        this.secondaryButtonData = secondaryButtonData;
        hasSecondaryButton = true;
    }
    #endregion
}
