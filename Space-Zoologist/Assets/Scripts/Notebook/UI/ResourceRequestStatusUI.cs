using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            statusReasonText.text = request.StatusReason;
        }
        else parentObject.SetActive(false);
    }
    #endregion
}
