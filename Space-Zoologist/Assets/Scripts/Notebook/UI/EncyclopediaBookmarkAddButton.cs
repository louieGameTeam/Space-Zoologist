using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaBookmarkAddButton : NotebookUIChild
{
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

    protected override void Awake()
    {
        base.Awake();
        button.onClick.AddListener(OnClick);
    }

    // On click, try to add the bookmark
    // If adding the bookmark succeeds, then make the bookmark UI create a new bookmark
    protected virtual void OnClick()
    {
        EncyclopediaBookmark bookmark = EncyclopediaBookmark.Create("{0} Encyclopedia: {1}", NotebookTab.Research, categoryPicker, ui.CurrentArticleID);
        if (UIParent.NotebookModel.TryAddBookmark(bookmark))
        {
            bookmarkUI.CreateBookmarkButton(bookmark);
        }
    }
}
