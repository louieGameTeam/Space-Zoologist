using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class NotebookBookmarkNavigateButton : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Button that navigates to the bookmark when clicked")]
    private Button navigateButton;
    [SerializeField]
    [Tooltip("Reference to the toggle that will delete the bookmark when navigating to another page")]
    private Toggle deleteToggle;
    [SerializeField]
    [Tooltip("Reference to the object used to display the text of the button")]
    private TextMeshProUGUI text;

    // Bookmark represented by this button
    private NotebookBookmark bookmark;

    public void Setup(NotebookBookmark bookmark)
    {
        base.Setup();
        this.bookmark = bookmark;

        text.text = bookmark.Label;
        navigateButton.onClick.AddListener(OnNavigateButtonClicked);
        deleteToggle.onValueChanged.AddListener(OnDeleteToggleChanged);
    }

    // On click select the correct tab, and setup the category picker
    private void OnNavigateButtonClicked()
    {
        UIParent.NavigateToBookmark(bookmark);
    }

    private void OnDeleteToggleChanged(bool isOn)
    {
        navigateButton.interactable = isOn;

        if (isOn) text.text = bookmark.Label;
        else text.text = "<marked for deletion>";
    }

    private void OnDisable()
    {
        if (!deleteToggle.isOn)
        {
            UIParent.Notebook.RemoveBookmark(bookmark);
            Destroy(gameObject);
        }
    }
}
