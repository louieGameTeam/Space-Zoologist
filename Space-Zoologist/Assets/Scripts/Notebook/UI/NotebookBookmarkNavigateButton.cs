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
    private NotebookTabPicker tabPicker;
    // Category picker to setup when the bookmark is activated
    private ResearchCategoryPicker categoryPicker;
    // Bookmark represented by this button
    private NotebookBookmark bookmark;

    public void Setup(NotebookBookmark bookmark, NotebookTabPicker tabPicker, ResearchCategoryPicker categoryPicker)
    {
        this.bookmark = bookmark;
        this.tabPicker = tabPicker;
        this.categoryPicker = categoryPicker;

        text.text = bookmark.Label;
        myButton.onClick.AddListener(OnClick);
    }

    // On click select the correct tab, and setup the category picker
    private void OnClick()
    {
        tabPicker.SelectTab(bookmark.Tab);
        categoryPicker.SelectedCategory = bookmark.Category;
    }
}
