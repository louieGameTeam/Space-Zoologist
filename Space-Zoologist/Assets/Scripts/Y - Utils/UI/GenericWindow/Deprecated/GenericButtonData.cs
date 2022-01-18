using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GenericButtonData
{
    #region Public Properties
    public string ButtonText => buttonText;
    public UnityEvent ButtonAction => buttonAction;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Text to display in the button")]
    private string buttonText;
    [SerializeField]
    [Tooltip("Action invoked when the button is clicked")]
    private UnityEvent buttonAction;
    #endregion

    #region Constructors
    public GenericButtonData(string buttonText, UnityAction listener)
    {
        this.buttonText = buttonText;

        // Create the action and add a listener
        buttonAction = new UnityEvent();
        buttonAction.AddListener(listener);
    }
    #endregion
}
