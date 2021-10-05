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
    private EnclosureIDPicker enclosurePicker;
    [SerializeField]
    [Tooltip("Object used to edit a list of resource requests")]
    private ResourceRequestListEditor listEditor; 
    [SerializeField]
    [Tooltip("Button used to request resources")]
    private Button requestButton;
    [SerializeField]
    [Tooltip("Text that displays the requests remaining")]
    private TextMeshProUGUI requestsText;
    [SerializeField]
    [Tooltip("Text that displays the resources remaining")]
    private TextMeshProUGUI resourcesText;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Add listener for enclosure id picked
        enclosurePicker.OnEnclosureIDPicked.AddListener(OnEnclosureIDPicked);
        OnEnclosureIDPicked(LevelID.FromCurrentSceneName());

        // When request button clicked then review resource requests
        requestButton.onClick.AddListener(ReviewResourceRequests);

        // Try to get an instance of the event manager
        EventManager instance = EventManager.Instance;

        if(instance)
        {
            instance.SubscribeToEvent(EventType.NextDay, () => UpdateText(enclosurePicker.CurrentEnclosureID));
        }
    }
    #endregion

    #region Private Methods
    private void OnEnclosureIDPicked(LevelID id)
    {
        // Update list being edited by the list editor
        listEditor.UpdateListEdited(id, UIParent.Notebook.Concepts.GetResourceRequestList(id));

        // Make request button interactable only if the id picked is the current id
        LevelID current = LevelID.FromCurrentSceneName();
        requestButton.interactable = id == current;

        // Update the text displayed
        UpdateText(id);
    }
    private void ReviewResourceRequests()
    {
        // Make the concept model review the requests
        UIParent.Notebook.Concepts.ReviewResourceRequests();

        // Update review ui for all editors
        listEditor.UpdateReviewUI();

        // Update the text displayed
        UpdateText(LevelID.FromCurrentSceneName());
    }
    private void UpdateText(LevelID id)
    {
        int requestsLeft = UIParent.Notebook.Concepts.RemainingRequests(id);
        int resourcesLeft = UIParent.Notebook.Concepts.RemainingRequestableResources(id);

        requestsText.text = requestsLeft.ToString();
        resourcesText.text = resourcesLeft.ToString();
    }
    #endregion
}
