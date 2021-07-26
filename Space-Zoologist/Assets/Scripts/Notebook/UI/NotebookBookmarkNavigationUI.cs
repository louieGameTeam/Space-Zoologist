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
    [Tooltip("Parent object for all navigation buttons")]
    private Transform buttonParent;

    private void Awake()
    {
        // NOTE: for now, on each play, remove all bookmarks
        // Unity's serialization system seems to serialize the bookmarks to their base class,
        // breaking their heirarchy. We'll have to solve this in a different way later
        notebook.Bookmarks.Clear();

        // Create a bookmark for each bookmark currently in the notebook
        //for (int i = 0; i < notebook.Bookmarks.Count; i++)
        //{
        //    CreateBookmarkButton(notebook.Bookmarks[i]);
        //}
    }

    // When a new bookmark is created, then instantiate a new button for it
    public void CreateBookmarkButton(NotebookBookmark newBookmark)
    {
        NotebookBookmarkNavigateButton clone = Instantiate(buttonPrefab, buttonParent);
        clone.Setup(notebookUI, newBookmark);
    }
}
