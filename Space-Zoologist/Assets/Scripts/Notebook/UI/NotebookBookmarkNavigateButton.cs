using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class NotebookBookmarkNavigateButton : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Button that navigates to the bookmark when clicked")]
    private Button myButton;
    [SerializeField]
    [Tooltip("Reference to the toggle that will delete the bookmark when navigating to another page")]
    private Toggle myToggle;
    [SerializeField]
    [Tooltip("Reference to the object used to display the text of the button")]
    private TextMeshProUGUI text;

    // Bookmark represented by this button
    private NotebookBookmark bookmark;

    public void Setup(NotebookBookmark bookmark)
    {
        this.bookmark = bookmark;

        text.text = bookmark.Label;
        myButton.onClick.AddListener(OnClick);
    }

    // On click select the correct tab, and setup the category picker
    private void OnClick()
    {
        UIParent.NavigateToBookmark(bookmark);
    }

    private void OnDisable()
    {
        if (!myToggle.isOn) Destroy(gameObject);
    }
}
