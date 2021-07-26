using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaBookmarkAddButton : MonoBehaviour
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
    [Tooltip("Reference to the category picker to add a bookmark for")]
    protected ResearchCategoryPicker categoryPicker;
    [SerializeField]
    [Tooltip("Reference to the encyclopedia ui to create a bookmark for")]
    protected ResearchEncyclopediaUI ui;

    protected virtual void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    // On click, try to add the bookmark
    // If adding the bookmark succeeds, then make the bookmark UI create a new bookmark
    protected virtual void OnClick()
    {
        EncyclopediaBookmark bookmark = EncyclopediaBookmark.Create("Encyclopedia", NotebookTab.Research, categoryPicker, ui.CurrentArticleID);
        if (notebook.TryAddBookmark(bookmark))
        {
            bookmarkUI.CreateBookmarkButton(bookmark);
        }
    }
}
