using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NotebookUI : MonoBehaviour
{
    #region Public Typedefs
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    #endregion

    #region Public Properties
    public NotebookModel Notebook => notebook;
    public NotebookTabPicker TabPicker => tabPicker;
    public UnityEvent OnContentChanged => onContentChanged;
    public BoolEvent OnNotebookToggle => onNotebookToggle;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the serialized object that holds all info about the notebook")]
    private NotebookModel notebook;
    [SerializeField]
    [Tooltip("Reference to the script that selects the tabs in the notebook")]
    private NotebookTabPicker tabPicker;
    [SerializeField]
    [Tooltip("Reference to the script that edits resource requests")]
    private ResourceRequestEditor resourceRequestEditor;
    [SerializeField]
    [Tooltip("Event invoked when the content on the notebook changes")]
    private UnityEvent onContentChanged;
    [SerializeField]
    [Tooltip("Event invoked each time the notebook is enabled/disabled")]
    private BoolEvent onNotebookToggle;
    #endregion

    #region Private Fields
    // Maps the names of the category pickers to the components for fast lookup
    // Used for navigating to a bookmark in the notebook
    private Dictionary<string, BookmarkTarget> nameTargetMap = new Dictionary<string, BookmarkTarget>();
    private bool isOpen = false;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Setup the notebook at the start
        notebook.Setup();

        // Update the enclosure IDs
        notebook.TryAddEnclosureID(LevelID.FromCurrentSceneName());

        // Try to get an instance of the game manager
        GameManager instance = GameManager.Instance;

        // If the instance exists then unlock all item id's that exist in the list of items
        if(instance)
        {
            foreach(LevelData.ItemData item in instance.LevelData.ItemQuantities)
            {
                notebook.UnlockItem(item.itemObject.ItemID);
            }
        }

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
    #endregion

    #region Public Methods
    // Directly referenced by the button
    public void Toggle()
    {
        SetIsOpen(!isOpen);
    }
    public void SetIsOpen(bool isOpen)
    {
        if (isOpen != this.isOpen)
        {
            if (isOpen)
                AudioManager.instance.PlayOneShot(SFXType.MenuOpen);
            else
                AudioManager.instance.PlayOneShot(SFXType.MenuClose);
        }

        this.isOpen = isOpen;
        gameObject.SetActive(isOpen);
        onNotebookToggle.Invoke(isOpen);
    }
    public void NavigateToBookmark(Bookmark bookmark)
    {
        if (!isOpen) SetIsOpen(true);
        bookmark.Navigate(nameTargetMap);
    }
    /// <summary>
    /// Fill in a resource request on the request list editor
    /// </summary>
    /// <param name="resourceRequest"></param>
    public void FillResourceRequest(ResourceRequest resourceRequest)
    {
        // This one property set invokes a cascade of events automatically so that the ui updates correctly
        resourceRequestEditor.Request = resourceRequest;
    }
    #endregion
}
