using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookBookmarkNavigationUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the notebook object")]
    private Notebook notebook;
    [SerializeField]
    [Tooltip("Prefab of the buttons used to navigate to a bookmark")]
    private NotebookBookmarkNavigateButton buttonPrefab;
    [SerializeField]
    [Tooltip("Reference to the script that navigates between notebook tabs")]
    private NotebookTabPicker tabPicker;
    [SerializeField]
    [Tooltip("Parent object for all navigation buttons")]
    private Transform buttonParent;
    [SerializeField]
    [Tooltip("Reference to the parent of all notebook UI. " +
        "Used to find the category pickers in the notebook that the bookmarks will refer to")]
    private GameObject notebookRoot;

    // Maps the names of the category pickers to the components for fast lookup
    private Dictionary<string, ResearchCategoryPicker> namePickerMap = new Dictionary<string, ResearchCategoryPicker>();

    private void Awake()
    {
        // Map all pickers to their corresponding name
        ResearchCategoryPicker[] allPickers = notebookRoot.GetComponentsInChildren<ResearchCategoryPicker>(true);
        foreach (ResearchCategoryPicker picker in allPickers)
        {
            namePickerMap.Add(picker.name, picker);
        }
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
        clone.Setup(newBookmark, tabPicker, namePickerMap[newBookmark.PickerName]);
    }
}
