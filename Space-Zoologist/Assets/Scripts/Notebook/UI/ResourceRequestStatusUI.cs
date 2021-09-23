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
            // By default status reason is inactive
            statusReasonObject.SetActive(false);

            // If the request was denied or only partially granted then we need to set up
            // hover events to display the status reason
            if(request.CurrentStatus == ResourceRequest.Status.Denied || request.PartiallyGranted)
            {
                // Clear any existing triggers
                statusTrigger.triggers.Clear();

                // Create the pointer enter event trigger
                UnityEngine.EventSystems.EventTrigger.Entry pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry()
                {
                    eventID = EventTriggerType.PointerEnter,
                    callback = new UnityEngine.EventSystems.EventTrigger.TriggerEvent()
                };
                pointerEnter.callback.AddListener(x => OnStatusPointerEnter());

                // Create the pointer enter event trigger
                UnityEngine.EventSystems.EventTrigger.Entry pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry()
                {
                    eventID = EventTriggerType.PointerExit,
                    callback = new UnityEngine.EventSystems.EventTrigger.TriggerEvent()
                };
                pointerExit.callback.AddListener(x => OnStatusPointerExit());

                // Add the entries to the list of triggers
                statusTrigger.triggers.Add(pointerEnter);
                statusTrigger.triggers.Add(pointerExit);
            }
        }
        else parentObject.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void OnStatusPointerEnter()
    {
        statusReasonObject.SetActive(true);
    }
    private void OnStatusPointerExit()
    {
        statusReasonObject.SetActive(false);
    }
    #endregion
}
