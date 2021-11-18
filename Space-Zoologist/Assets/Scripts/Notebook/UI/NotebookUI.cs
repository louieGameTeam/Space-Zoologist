using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor;

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
    public bool IsOpen => isOpen;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform at the root of the notebook UI")]
    private RectTransform root;
    [SerializeField]
    [Tooltip("Reference to the serialized object that holds all info about the notebook")]
    private NotebookModel notebook;
    [SerializeField]
    [Tooltip("Reference to the script that selects the tabs in the notebook")]
    private NotebookTabPicker tabPicker;
    [SerializeField]
    [Tooltip("Reference to the audio manager for the notebook")]
    private NotebookSoundManager soundManager;
    [SerializeField]
    [Tooltip("Offsets from the sceen edges for the notebook")]
    private RectOffset defaultSize;
    [SerializeField]
    [Tooltip("Size of the notebook while dialogue is present")]
    private RectOffset dialogueSize;
    [SerializeField]
    [Tooltip("Event invoked when the content on the notebook changes")]
    private UnityEvent onContentChanged;
    [SerializeField]
    [Tooltip("Event invoked each time the notebook is enabled/disabled")]
    private BoolEvent onNotebookToggle;
    #endregion

    #region Private Fields
    // Reference to the resource request editor
    private ResourceRequestEditor resourceRequestEditor;
    // Maps the names of the category pickers to the components for fast lookup
    // Used for navigating to a bookmark in the notebook
    private Dictionary<string, BookmarkTarget> nameTargetMap = new Dictionary<string, BookmarkTarget>();
    private bool isOpen = false;
    // True if a callback is set up on the conversation manager yet
    // This has to be used because we don't know for sure if conversation manager instance is null or not
    // at the beginning of the scene
    private bool conversationCallbackSet = false;
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

        // Setup the tab picker first of all children
        tabPicker.Setup();

        // Get the resource request editor manually
        resourceRequestEditor = GetComponentInChildren<ResourceRequestEditor>(true);

        // Setup all children, ensuring correct initialization order
        NotebookUIChild[] children = GetComponentsInChildren<NotebookUIChild>(true);
        foreach (NotebookUIChild child in children) child.Setup();

        // Map all bookmark targets to their corresponding game object names
        BookmarkTarget[] allBookmarkTargets = GetComponentsInChildren<BookmarkTarget>(true);
        foreach (BookmarkTarget bookmarkTarget in allBookmarkTargets)
        {
            nameTargetMap.Add(bookmarkTarget.name, bookmarkTarget);
        }

        // Setup sound events after all children are set up
        soundManager.SetupSoundEvents();

        // This line of code prevents the notebook from turning off the first time that it is turned on,
        // while also making sure it is turned off at the start
        if (!isOpen) SetIsOpen(false);
    }
    private void OnEnable()
    {
        if (ConversationManager.Instance)
        {
            // If the callbacks have not been set yet then set them now
            if(!conversationCallbackSet)
            {
                ConversationManager.OnConversationStarted += SetDialogueOffsets;
                ConversationManager.OnConversationEnded += SetDefaultOffsets;
                conversationCallbackSet = true;
            }
            SetRectTransformOffsets(ConversationManager.Instance.IsConversationActive);
        }
    }
    // Unset the callbacks when this object is destroyed
    private void OnDestroy()
    {
        ConversationManager.OnConversationStarted -= SetDialogueOffsets;
        ConversationManager.OnConversationEnded -= SetDefaultOffsets;
    }
    #endregion

    #region Public Methods
    // Directly referenced by the button in the scene
    public void Toggle()
    {
        SetIsOpen(!isOpen);
    }
    public void SetIsOpen(bool isOpen)
    {
        if (isOpen != this.isOpen)
        {
            if (isOpen)
                AudioManager.instance.PlayOneShot(SFXType.NotebookOpen);
            else
                AudioManager.instance.PlayOneShot(SFXType.NotebookClose);
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

    #region Private Methods
    private void SetRectTransformOffsets(bool dialogueActive)
    {
        if (dialogueActive) SetDialogueOffsets();
        else SetDefaultOffsets();
    }
    private void SetDefaultOffsets() => root.SetOffsets(defaultSize);
    private void SetDialogueOffsets() => root.SetOffsets(dialogueSize);
    #endregion
}
