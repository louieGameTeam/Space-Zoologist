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
    [Tooltip("Toggle that displays the dropdown that lets the user name the bookmark")]
    private Toggle dropdownToggle;
    [SerializeField]
    [Tooltip("Game object enabled over the toggle when it is enabled")]
    private GameObject isOnObject;
    [SerializeField]
    [Tooltip("Dropdown menu that appears when the button is clicked")]
    private GameObject dropdown;
    [SerializeField]
    [Tooltip("Text used to input the name of the new bookmark")]
    private TMP_InputField bookmarkTitle;
    [SerializeField]
    [Tooltip("Reference to the button that adds the bookmark when clicked")]
    private Button confirmButton;
    [SerializeField]
    [Tooltip("Reference to the button that cancels addinga bookmark")]
    private Button cancelButton;
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

        dropdownToggle.onValueChanged.AddListener(OnDropdownToggleStateChange);
        confirmButton.onClick.AddListener(TryAddBookmark);
        cancelButton.onClick.AddListener(CancelBookmarkAdd);
        bookmarkTitle.onSubmit.AddListener(s => TryAddBookmark());

        // Update dropdown based on initial state of the toggle
        OnDropdownToggleStateChange(dropdownToggle.isOn);
    }

    private void OnDropdownToggleStateChange(bool active)
    {
        // If activating, we need to check if the notebook already exists
        // If it does, we should not bother opening the dropdown
        if (active)
        {
            // Set title to suggested title
            bookmarkTitle.text = SuggestedBookmarkTitle;
            NotebookBookmark intendedBookmark = BookmarkToAdd(bookmarkTitle.text);

            // If the bookmark already exists, disable the toggle
            if (UIParent.Notebook.HasBookmark(intendedBookmark))
            {
                dropdownToggle.SetIsOnWithoutNotify(false);
            }
        }

        // Set is on and dropdown based on toggle state
        isOnObject.SetActive(dropdownToggle.isOn);
        dropdown.SetActive(dropdownToggle.isOn);
    }

    // On click, try to add the bookmark
    // If adding the bookmark succeeds, then make the bookmark UI create a new bookmark
    protected virtual void TryAddBookmark()
    {
        NotebookBookmark bookmark = BookmarkToAdd(bookmarkTitle.text);
        if(UIParent.Notebook.TryAddBookmark(bookmark))
        {
            bookmarkUI.CreateBookmarkButton(bookmark);

            // Disable the toggle now, immediately invokes OnDropdownToggleStateChanged
            dropdownToggle.isOn = false;
        }
    }

    private void CancelBookmarkAdd()
    {
        // Immediately invokes OnDropdownToggleStateChanged
        dropdownToggle.isOn = false;
    }
    // On disabled disable the dropdown, immediately invokes OnDropdownToggleStateChanged
    private void OnDisable()
    {
        dropdownToggle.isOn = false;
    }

    protected abstract NotebookBookmark BookmarkToAdd(string inputText);
}
