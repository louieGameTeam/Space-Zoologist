using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class NotebookBookmarkNavigateButton : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Button that navigates to the bookmark when clicked")]
    private Button myButton;
    [SerializeField]
    [Tooltip("Reference to the object used to display the text of the button")]
    private TextMeshProUGUI text;

    // Reference to the script that manages navigation between notebook tabs
    private NotebookUI notebookUI;
    // Bookmark represented by this button
    private NotebookBookmark bookmark;

    public void Setup(NotebookUI notebookUI, NotebookBookmark bookmark)
    {
        this.notebookUI = notebookUI;
        this.bookmark = bookmark;

        text.text = bookmark.Label;
        myButton.onClick.AddListener(OnClick);
    }

    // On click select the correct tab, and setup the category picker
    private void OnClick()
    {
        notebookUI.NavigateToBookmark(bookmark);
    }
}
