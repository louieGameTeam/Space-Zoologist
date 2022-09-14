using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

using TMPro;

public class BookmarkAddButton : NotebookUIChild
{
    #region Private Properties
    private BookmarkTarget[] BookmarkTargets
    {
        get
        {
            BookmarkTarget[] targets = new BookmarkTarget[additionalBookmarkTargets.Length + 1];
            targets[0] = UIParent.TabPicker.GetComponent<BookmarkTarget>();
            System.Array.Copy(additionalBookmarkTargets, 0, targets, 1, additionalBookmarkTargets.Length);
            return targets;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Suggested title of the bookmark")]
    private string suggestedTitle = "New Bookmark";
    [SerializeField]
    [Tooltip("Reference to the script to assist with the dropdown functionality")]
    private GeneralDropdown dropdown = null;
    [SerializeField]
    [Tooltip("Game object that is enabled if the current page has a bookmark")]
    private GameObject hasBookmarkGraphic = null;
    [SerializeField]
    [Tooltip("Text used to input the name of the new bookmark")]
    private TMP_InputField bookmarkTitle = null;
    [SerializeField]
    [Tooltip("Reference to the button that adds the bookmark when clicked")]
    private Button confirmButton = null;
    [SerializeField]
    [FormerlySerializedAs("bookmarkTargets")]
    [Tooltip("List of components to target when navigating to the newly added bookmark")]
    protected BookmarkTarget[] additionalBookmarkTargets = null;
    #endregion

    #region Private Fields
    // Reference to the UI used to navigate to bookmarks
    private NotebookBookmarkNavigationUI bookmarkUI;
    #endregion

    #region Monobehaviour Messages
    private void OnEnable()
    {
        if (IsSetUp) UpdateInteractable();
    }
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Get the bookmark navigation ui manually
        bookmarkUI = UIParent.gameObject.GetComponentInChildren<NotebookBookmarkNavigationUI>(true);

        dropdown.OnDropdownEnabled.AddListener(OnDropdownActivated);
        confirmButton.onClick.AddListener(TryAddBookmark);
        bookmarkTitle.onSubmit.AddListener(s => TryAddBookmark());

        UIParent.OnContentChanged.AddListener(UpdateInteractable);
    }
    public void UpdateInteractable()
    {
        Bookmark bookmark = new Bookmark(suggestedTitle, BookmarkTargets.Select(x => BookmarkData.Create(x)).ToArray());
        dropdown.Interactable = !UIParent.Data.HasBookmark(bookmark);
        hasBookmarkGraphic.SetActive(!dropdown.Interactable);
    }
    #endregion

    #region Private / Protected Methods
    private void OnDropdownActivated()
    {
        // When the dropdown is activated then set the suggested bookmark title
        bookmarkTitle.text = suggestedTitle;
    }

    // On click, try to add the bookmark
    // If adding the bookmark succeeds, then make the bookmark UI create a new bookmark
    protected virtual void TryAddBookmark()
    {
        Bookmark bookmark = new Bookmark(bookmarkTitle.text, BookmarkTargets.Select(x => BookmarkData.Create(x)).ToArray());
        if (UIParent.Data.TryAddBookmark(bookmark))
        {
            bookmarkUI.CreateBookmarkButton(bookmark);
            dropdown.DisableDropdown();

            // Update interactable state of the button
            UpdateInteractable();
        }
    }
    #endregion
}
