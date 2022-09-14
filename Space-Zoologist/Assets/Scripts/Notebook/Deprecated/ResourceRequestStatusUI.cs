using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[System.Obsolete("No system exists to display the status of a resource request anymore. " +
    "Use the 'ReviewedResourceRequestDisplay' instead.")]
public class ResourceRequestStatusUI : NotebookUIChild
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Main object that the text is a parent under")]
    private GameObject parentObject;
    [SerializeField]
    [Tooltip("Text that displays the request status")]
    private TextMeshProUGUI statusText;
    [SerializeField]
    [Tooltip("Event trigger attached to the status bar object")]
    private UnityEngine.EventSystems.EventTrigger statusTrigger;
    [SerializeField]
    [Tooltip("Parent game object for the status reason display")]
    private GameObject statusReasonObject;
    [SerializeField]
    [Tooltip("Text that displays the reason for the request status")]
    private TextMeshProUGUI statusReasonText;
    #endregion
}
