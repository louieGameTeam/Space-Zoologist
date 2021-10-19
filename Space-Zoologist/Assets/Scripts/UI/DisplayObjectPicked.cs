using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayObjectPicked : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the toggle with the object to display")]
    private AbstractTogglePicker togglePicker;
    [SerializeField]
    [Tooltip("Reference to the text used to display the object picked on the toggle")]
    private TextMeshProUGUI text;
    #endregion

    #region Monobehaviour Messages

    #endregion
}
