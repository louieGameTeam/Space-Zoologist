using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookBookmarkNavigationUI : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Prefab of the buttons used to navigate to a bookmark")]
    private NotebookBookmarkNavigateButton buttonPrefab;
    [SerializeField]
    [Tooltip("Parent object for all navigation buttons")]
    private Transform buttonParent;

    public override void Setup()
    {
        base.Setup();

        // Create a bookmark for each bookmark currently in the notebook
        for (int i = 0; i < UIParent.Notebook.Bookmarks.Count; i++)
        {
            CreateBookmarkButton(UIParent.Notebook.Bookmarks[i]);
        }
    }

    // When a new bookmark is created, then instantiate a new button for it
    public void CreateBookmarkButton(Bookmark newBookmark)
    {
        NotebookBookmarkNavigateButton clone = Instantiate(buttonPrefab, buttonParent);
        clone.Setup(newBookmark);
    }
}
