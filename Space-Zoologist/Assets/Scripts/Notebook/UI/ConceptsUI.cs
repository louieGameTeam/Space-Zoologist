using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConceptsUI : NotebookUIChild
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Object used to edit the current resource request")]
    private ResourceRequestEditor requestEditor;
    [SerializeField]
    [Tooltip("Object that displays when the player attempts to request the resource")]
    private ReviewedResourceRequestDisplay reviewDisplay;
    [SerializeField]
    [Tooltip("Button used to request resources")]
    private Button requestButton;
    [SerializeField]
    [Tooltip("Text that displays the money remaining")]
    private TextMeshProUGUI balanceText;
    [SerializeField]
    [Tooltip("Object used to display the store item cells")]
    private DisplayItemCellsByCategory itemCellDisplay;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // When request button clicked then review the current request
        requestButton.onClick.AddListener(() => reviewDisplay.DisplayReview(requestEditor.Request));
        // Update the text whenever the review is confirmed
        reviewDisplay.OnReviewConfirmed.AddListener(OnReviewConfirmed);

        // Update text once at the beginning
        UpdateText();
    }
    #endregion

    #region Private Methods
    private void UpdateText()
    {
        if(GameManager.Instance)
        {
            balanceText.text = "$" + GameManager.Instance.Balance;
        }
    }
    private void OnReviewConfirmed()
    {
        UpdateText();
        itemCellDisplay.SetupCells();
        requestEditor.ResetRequest();
    }
    #endregion
}
