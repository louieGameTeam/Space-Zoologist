using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookBookmarkNavigationUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the notebook object")]
    private Notebook notebook;
    [SerializeField]
    [Tooltip("Reference to the main notebook UI script")]
    private NotebookUI notebookUI;
    [SerializeField]
    [Tooltip("Prefab of the buttons used to navigate to a bookmark")]
    private NotebookBookmarkNavigateButton buttonPrefab;
    [SerializeField]
    [Tooltip("Reference to the script that navigates between notebook tabs")]
    private NotebookTabPicker tabPicker;
    [SerializeField]
    [Tooltip("Parent object for all navigation buttons")]
    private Transform buttonParent;

    private void Awake()
    {
        // Create a bookmark for each bookmark currently in the notebook
        foreach (NotebookBookmark bookmark in notebook.Bookmarks)
        {
            CreateBookmarkButton(bookmark);
        }
    }

    // When a new bookmark is created, then instantiate a new button for it
    public void CreateBookmarkButton(NotebookBookmark newBookmark)
    {
        NotebookBookmarkNavigateButton clone = Instantiate(buttonPrefab, buttonParent);
        clone.Setup(notebookUI, newBookmark);
    }
}
