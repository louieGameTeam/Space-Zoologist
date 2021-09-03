using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

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

    #region Public Methods
    public void UpdateDisplay(ResourceRequest request)
    {
        if (request != null)
        {
            parentObject.SetActive(request.CurrentStatus != ResourceRequest.Status.NotReviewed);
            statusText.text = request.CurrentStatus.ToString();

            // If the request was only partially granted then add "Partially" to the front of the string
            if (request.PartiallyGranted) statusText.text = statusText.text.Insert(0, "Partially ");

            // Set the text on the status reason
            statusReasonText.text = request.StatusReason;

            if(request.CurrentStatus == ResourceRequest.Status.Denied || 
                (request.CurrentStatus == ResourceRequest.Status.Granted && request.PartiallyGranted))
            {
                statusTrigger.triggers.Clear();


            }
        }
        else parentObject.SetActive(false);
    }
    #endregion
}
