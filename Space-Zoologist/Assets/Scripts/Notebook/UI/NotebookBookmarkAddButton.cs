using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookBookmarkAddButton : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the notebook object to add a bookmark to")]
    private Notebook notebook;
    [SerializeField]
    [Tooltip("Reference to the button that adds the bookmark when clicked")]
    private Button button;
    [SerializeField]
    [Tooltip("Reference to the script that manages the UI for the bookmarks")]
    private NotebookBookmarkNavigationUI bookmarkUI;

    [Header("Bookmark data")]

    [SerializeField]
    [Tooltip("Prefix to attach to the name of the bookmark")]
    protected string formatLabel;
    [SerializeField]
    [Tooltip("Tab to set for the bookmark button")]
    protected NotebookTab tab;
    [SerializeField]
    [Tooltip("Reference to the category picker to add a bookmark for")]
    protected ResearchCategoryPicker categoryPicker;

    protected virtual void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    // On click, try to add the bookmark
    // If adding the bookmark succeeds, then make the bookmark UI create a new bookmark
    protected virtual void OnClick()
    {
        NotebookBookmark bookmark = NotebookBookmark.Create(formatLabel, tab, categoryPicker);
        if(notebook.TryAddBookmark(bookmark))
        {
            bookmarkUI.CreateBookmarkButton(bookmark);
        }
    }
}
