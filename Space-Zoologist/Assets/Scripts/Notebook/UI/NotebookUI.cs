using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NotebookUI : MonoBehaviour
{
    // Public accessors
    public NotebookModel Notebook => notebook;
    public UnityEvent OnEnclosureIDsUpdated => onEnclosureIDsUpdated;

    [SerializeField]
    [Expandable]
    [Tooltip("Reference to the serialized object that holds all info about the notebook")]
    private NotebookModel notebook;
    [SerializeField]
    [Tooltip("Reference to the script that selects the tabs in the notebook")]
    private NotebookTabPicker tabPicker;
    [SerializeField]
    [Tooltip("Event invoked whenever the test and metrics model updates")]
    private UnityEvent onEnclosureIDsUpdated;

    // Maps the names of the category pickers to the components for fast lookup
    // Used for navigating to a bookmark in the notebook
    private Dictionary<string, ResearchCategoryPicker> namePickerMap = new Dictionary<string, ResearchCategoryPicker>();
    private bool isOpen = false;
    private EnclosureSystem enclosureSystem;

    // I thought that this was called when the game object is inactive but apparently it is not
    private void Awake()
    {
        // Setup the notebook at the start
        notebook.Setup();

        // Setup the test and metric data with the current level name and enclosure set
        enclosureSystem = FindObjectOfType<EnclosureSystem>();
        UpdateEnclosureIDs();
        // Whenever a new enclosed area appears, update the test and metrics model
        EventManager.Instance.SubscribeToEvent(EventType.NewEnclosedArea, UpdateEnclosureIDs);

        // Map all pickers to their corresponding name
        ResearchCategoryPicker[] allPickers = GetComponentsInChildren<ResearchCategoryPicker>(true);
        foreach (ResearchCategoryPicker picker in allPickers)
        {
            namePickerMap.Add(picker.name, picker);
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
    }

    public void NavigateToBookmark(NotebookBookmark bookmark)
    {
        // Get the expected component in the children of the notebook somewhere
        Component component;

        // Set component based on if type is null
        if (bookmark.ExpectedComponentType != null) component = GetComponentInChildren(bookmark.ExpectedComponentType, true);
        else component = null;

        bookmark.NavigateTo(tabPicker, namePickerMap, component);
    }

    /// <summary>
    /// Get a list of the current ids in the level
    /// </summary>
    /// <returns>A list of enclosure IDs</returns>
    public List<EnclosureID> CurrentIDs()
    {
        List<EnclosureID> ids = new List<EnclosureID>();
        string sceneName = SceneManager.GetActiveScene().name;

        foreach(EnclosedArea area in enclosureSystem.EnclosedAreas)
        {
            ids.Add(new EnclosureID(sceneName, area.id));
        }

        return ids;
    }

    /// <summary>
    /// Update the test and metrics model by adding any ids that did not exist before
    /// </summary>
    private void UpdateEnclosureIDs()
    {
        foreach(EnclosureID id in CurrentIDs())
        {
            Notebook.TryAddEnclosureID(id);
        }
        onEnclosureIDsUpdated.Invoke();
    }
}
