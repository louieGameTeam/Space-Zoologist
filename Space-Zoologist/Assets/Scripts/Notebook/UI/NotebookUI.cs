using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using DialogueEditor;

public class NotebookUI : MonoBehaviour
{
    #region Public Typedefs
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    #endregion

    #region Public Properties
    public NotebookConfig Config => config;
    public NotebookData Data => data;
    public NotebookTabPicker TabPicker => tabPicker;
    public UnityEvent OnContentChanged => onContentChanged;
    public BoolEvent OnNotebookToggle => onNotebookToggle;
    public BoolEvent OnEnableInspectorToggle => onEnableInspectorToggle;
    public bool IsOpen => isOpen;
    public ResourceRequestEditor ResourceRequestEditor => resourceRequestEditor;
    public List<NotebookUIChild> NotebookUIChildren => notebookUIChildren;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Rect transform at the root of the notebook UI")]
    private RectTransform root;
    [SerializeField]
    [FormerlySerializedAs("notebook")]
    [Tooltip("Reference to the serialized object that holds all info about the notebook")]
    private NotebookConfig config;
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
    [SerializeField]
    [Tooltip("Event invoked each time the inspector needs to be enabled/disabled by the notebook")]
    private BoolEvent onEnableInspectorToggle;
    #endregion

    #region Private Fields
    // Data that the player edits as they play with the notebook
    private NotebookData data;
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

    //Band-aid fix for dialogue offset being bugged due to event listener ordering
    private bool frameDialogueOffsetLock = false;

    //Used to determine if the inspector should be showing or not
    private HashSet<NotebookUIChild> inspectorBlockingSet = new HashSet<NotebookUIChild>();

    // tracks notebookuichildren
    private  List<NotebookUIChild> notebookUIChildren = new List<NotebookUIChild>();
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Load the notebook from save, or create a new one if save data doesn't exist
        data = GameManager.Instance.LoadNotebook() ?? new NotebookData(config);

        // Set the configuration of the notebook data
        data.SetConfig(config);

        // Add the current level
        data.OnLevelEncountered(LevelID.Current());

        // Try to get an instance of the game manager
        GameManager instance = GameManager.Instance;

        // If the instance exists then unlock all item id's that exist in the list of items
        if (instance)
        {
            foreach (LevelData.ItemData item in instance.LevelData.ItemQuantities)
            {
                data.UnlockItem(item.itemObject.ID);
            }
        }

        // Setup the tab picker first of all children
        tabPicker.Setup();
        tabPicker.OnTabSelect += UpdateInspectorHiddenByChild;

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
            if (!conversationCallbackSet)
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
            {
                GameManager.Instance.TryToPause("Notebook");
                AudioManager.instance.PlayOneShot(SFXType.NotebookOpen);                
            }               
            else
            {
                GameManager.Instance.TryToUnpause("Notebook");
                AudioManager.instance.PlayOneShot(SFXType.NotebookClose);               
            }
        }

        this.isOpen = isOpen;
        gameObject.SetActive(isOpen);
        onNotebookToggle.Invoke(isOpen);
        if(isOpen) 
            UpdateInspectorHiddenByChild();
        else
            onEnableInspectorToggle.Invoke(true);
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

    /// <summary>
    /// Add or remove NotebookUIChild from the inspector blocker set
    /// </summary>
    /// <param name="child"></param>
    /// <param name="adding"></param>
    public void UpdateHideInspectorSet(NotebookUIChild child, bool adding = true)
    {
        if(adding)
        {
            inspectorBlockingSet.Add(child);
        }
        else
        {
            inspectorBlockingSet.Remove(child);
        }
        UpdateInspectorHiddenByChild();
    }
    
    /// <summary>
    /// Hide or show the inspector based on NotebookUIChildren in the blocker set
    /// </summary>
    private void UpdateInspectorHiddenByChild()
    {
        bool shouldHide = false;
        foreach(var child in inspectorBlockingSet)
        {
            if (child.gameObject.activeInHierarchy)
                shouldHide = true;
        }
        onEnableInspectorToggle.Invoke(!shouldHide);
    }

    #endregion

    #region Private Methods
    private void SetRectTransformOffsets(bool dialogueActive)
    {
        if (dialogueActive) SetDialogueOffsets();
        else SetDefaultOffsets();
    }

    private void SetDefaultOffsets()
    {
        if (!frameDialogueOffsetLock)
            root.SetOffsets(defaultSize);
    }
    private void SetDialogueOffsets()
    {
        root.SetOffsets(dialogueSize);

        // Make sure not to start a coroutine
        // while the behaviour is disabled
        if (gameObject.activeInHierarchy)
        {
            frameDialogueOffsetLock = true;
            StartCoroutine(Corout_FrameDialogueOffsetUnlocking());
        }
    }
    private IEnumerator Corout_FrameDialogueOffsetUnlocking()
    {
        yield return new WaitForEndOfFrame();
        frameDialogueOffsetLock = false;
    }

    #endregion
}
