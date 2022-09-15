using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConceptsUI : NotebookUIChild
{
    #region Public Properties
    public Button RequestButton => requestButton;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Object used to edit the current resource request")]
    private ResourceRequestEditor requestEditor = null;
    [SerializeField]
    [Tooltip("Object that displays when the player attempts to request the resource")]
    private ReviewedResourceRequestDisplay reviewDisplay = null;
    [SerializeField]
    [Tooltip("Button used to request resources")]
    private Button requestButton = null;
    [SerializeField]
    [Tooltip("Button that opens the build ui when clicked")]
    private Button buildButton = null;
    [SerializeField]
    [Tooltip("Text that displays the money remaining")]
    private TextMeshProUGUI balanceText = null;
    [SerializeField]
    [Tooltip("Object used to display the store item cells")]
    private DisplayItemCellsByCategory itemCellDisplay = null;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // When request button clicked then review the current request
        requestButton.onClick.AddListener(() => reviewDisplay.DisplayReview(requestEditor.Request));
        // When build button is clicked then open up the store
        buildButton.onClick.AddListener(() => GameManager.Instance.m_menuManager.SetStoreIsOn(true));
        // Update the text whenever the review is confirmed
        reviewDisplay.OnReviewConfirmed.AddListener(OnReviewConfirmed);
        // Set the requested item when the item cell display has an item clicked on it
        itemCellDisplay.ItemClickedEvent.AddListener(SetRequestedItem);
        // Update the text whenever an object is moved
        FindObjectOfType<MoveObject>().OnObjectMoved.AddListener(UpdateText);

        // Update text once at the beginning
        UpdateText();
    }
    #endregion

    #region Private Methods
    private void UpdateText()
    {
        print ("updated text");
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
    private void SetRequestedItem(Item item)
    {
        requestEditor.Request.ItemRequested = item.ID;
        requestEditor.UpdateUI();
    }
    #endregion
}
