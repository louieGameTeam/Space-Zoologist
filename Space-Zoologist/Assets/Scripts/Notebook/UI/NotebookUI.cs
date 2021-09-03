using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NotebookUI : MonoBehaviour
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    // Public accessors
    public NotebookModel Notebook => notebook;
    public UnityEvent OnContentChanged => onContentChanged;
    public BoolEvent OnNotebookToggle => onNotebookToggle;

    [SerializeField]
    [Expandable]
    [Tooltip("Reference to the serialized object that holds all info about the notebook")]
    private NotebookModel notebook;
    [SerializeField]
    [Tooltip("Reference to the script that selects the tabs in the notebook")]
    private NotebookTabPicker tabPicker;
    [SerializeField]
    [Tooltip("Event invoked when the content on the notebook changes")]
    private UnityEvent onContentChanged;
    [SerializeField]
    [Tooltip("Event invoked each time the notebook is enabled/disabled")]
    private BoolEvent onNotebookToggle;

    // Maps the names of the category pickers to the components for fast lookup
    // Used for navigating to a bookmark in the notebook
    private Dictionary<string, BookmarkTarget> nameTargetMap = new Dictionary<string, BookmarkTarget>();
    private bool isOpen = false;

    private void Start()
    {
        // Setup the notebook at the start
        notebook.Setup();

        // Update the enclosure IDs
        notebook.TryAddEnclosureID(EnclosureID.FromCurrentSceneName());

        // Map all bookmark targets to their corresponding game object names
        BookmarkTarget[] allBookmarkTargets = GetComponentsInChildren<BookmarkTarget>(true);
        foreach (BookmarkTarget bookmarkTarget in allBookmarkTargets)
        {
            nameTargetMap.Add(bookmarkTarget.name, bookmarkTarget);
        }

        // Setup all children, ensuring correct initialization order
        NotebookUIChild[] children = GetComponentsInChildren<NotebookUIChild>(true);
        foreach (NotebookUIChild child in children) child.Setup();

        // This line of code prevents the notebook from turning off the first time that it is turned on,
        // while also making sure it is turned off at the start
        if (!isOpen) SetIsOpen(false);
    }
    public void Toggle()
    {
        SetIsOpen(!isOpen);
    }
    public void SetIsOpen(bool isOpen)
    {
        this.isOpen = isOpen;
        gameObject.SetActive(isOpen);
        onNotebookToggle.Invoke(isOpen);
    }
    public void NavigateToBookmark(Bookmark bookmark)
    {
        bookmark.Navigate(nameTargetMap);
    }
}
