using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class GenericButton : MonoBehaviour
{
    #region Public Properties
    public Button Button => button;
    public TextMeshProUGUI ButtonText => buttonText;
    public Image ButtonImage => buttonImage;
    public UnityEvent ButtonAction => buttonAction;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the button in the generic window")]
    private Button button = null;
    [SerializeField]
    [Tooltip("Reference to the text displayed in the button")]
    private TextMeshProUGUI buttonText = null;
    [SerializeField]
    [Tooltip("Image that displays the background of the button")]
    private Image buttonImage = null;
    [SerializeField]
    [Tooltip("Event invoked after this button is pressed and the window has finished closing")]
    private UnityEvent buttonAction = null;
    #endregion
}
