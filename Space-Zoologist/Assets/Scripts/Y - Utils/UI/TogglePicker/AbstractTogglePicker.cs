using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AbstractTogglePicker : MonoBehaviour
{
    #region Public Properties
    public Toggle Toggle => toggle;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the toggle that picks the object")]
    private Toggle toggle;
    #endregion
}
