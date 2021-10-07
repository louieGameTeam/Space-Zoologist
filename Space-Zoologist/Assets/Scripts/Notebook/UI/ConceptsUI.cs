using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConceptsUI : NotebookUIChild
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Object used to pick the enclosure for this ui")]
    private LevelIDPicker enclosurePicker;
    [SerializeField]
    [Tooltip("Object used to edit a list of resource requests")]
    private ResourceRequestListEditor listEditor; 
    [SerializeField]
    [Tooltip("Button used to request resources")]
    private Button requestButton;
    [SerializeField]
    [Tooltip("Text that displays the money remaining")]
    private TextMeshProUGUI balanceText;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Add listener for enclosure id picked
        enclosurePicker.OnLevelIDPicked.AddListener(OnEnclosureIDPicked);
        OnEnclosureIDPicked(LevelID.FromCurrentSceneName());

        // When request button clicked then review resource requests
        requestButton.onClick.AddListener(ReviewResourceRequests);

        // Update text once at the beginning
        UpdateText();
    }
    #endregion

    #region Private Methods
    private void OnEnclosureIDPicked(LevelID id)
    {
        // Update list being edited by the list editor
        listEditor.UpdateListEdited(id);

        // Make request button interactable only if the id picked is the current id
        LevelID current = LevelID.FromCurrentSceneName();
        requestButton.interactable = id == current;
    }
    private void ReviewResourceRequests()
    {
        // Make the concept model review the requests
        UIParent.Notebook.Concepts.ReviewResourceRequests();

        // Update review ui for all editors
        listEditor.UpdateReviewUI();

        // Update the text displayed
        UpdateText();
    }
    private void UpdateText()
    {
        if(GameManager.Instance)
        {
            balanceText.text = GameManager.Instance.Balance.ToString();
        }
    }
    #endregion
}
