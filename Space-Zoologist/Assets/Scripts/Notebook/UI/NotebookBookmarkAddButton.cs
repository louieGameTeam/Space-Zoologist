using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public abstract class NotebookBookmarkAddButton : NotebookUIChild
{
    // Get the suggested title for the bookmark
    protected abstract string SuggestedBookmarkTitle { get; }

    [SerializeField]
    [Tooltip("Reference to the script to assist with the dropdown functionality")]
    private GeneralDropdown dropdown;
    [SerializeField]
    [Tooltip("Game object that is enabled if the current page has a bookmark")]
    private GameObject hasBookmarkGraphic;
    [SerializeField]
    [Tooltip("Text used to input the name of the new bookmark")]
    private TMP_InputField bookmarkTitle;
    [SerializeField]
    [Tooltip("Reference to the button that adds the bookmark when clicked")]
    private Button confirmButton;
    [SerializeField]
    [Tooltip("Reference to the script that manages the UI for the bookmarks")]
    private NotebookBookmarkNavigationUI bookmarkUI;

    [Header("Bookmark data")]

    [SerializeField]
    [Tooltip("Reference to the category picker to add a bookmark for")]
    protected ResearchCategoryPicker categoryPicker;

    public override void Setup()
    {
        base.Setup();

        dropdown.OnDropdownEnabled.AddListener(OnDropdownActivated);
        confirmButton.onClick.AddListener(TryAddBookmark);
        bookmarkTitle.onSubmit.AddListener(s => TryAddBookmark());

        UpdateInteractable();
    }

    private void OnDropdownActivated()
    {
        // When the dropdown is activated then set the suggested bookmark title
        bookmarkTitle.text = SuggestedBookmarkTitle;
    }

    // On click, try to add the bookmark
    // If adding the bookmark succeeds, then make the bookmark UI create a new bookmark
    protected virtual void TryAddBookmark()
    {
        NotebookBookmark bookmark = BookmarkToAdd(bookmarkTitle.text);
        if(UIParent.Notebook.TryAddBookmark(bookmark))
        {
            bookmarkUI.CreateBookmarkButton(bookmark);

            // Update interactable state of the button
            UpdateInteractable();
        }
    }

    private void OnEnable()
    {
        if (IsSetUp) UpdateInteractable();
    }

    public void UpdateInteractable()
    {
        NotebookBookmark bookmark = BookmarkToAdd(bookmarkTitle.text);
        dropdown.Interactable = !UIParent.Notebook.HasBookmark(bookmark);
        hasBookmarkGraphic.SetActive(!dropdown.Interactable);
    }

    protected abstract NotebookBookmark BookmarkToAdd(string inputText);
}
