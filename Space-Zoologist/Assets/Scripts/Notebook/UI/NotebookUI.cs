using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NotebookUI : MonoBehaviour
{
    // Public accessors
    public NotebookModel Notebook => notebook;

    [SerializeField]
    [Expandable]
    [Tooltip("Reference to the serialized object that holds all info about the notebook")]
    private NotebookModel notebook;
    [SerializeField]
    [Tooltip("Reference to the script that selects the tabs in the notebook")]
    private NotebookTabPicker tabPicker;

    // Maps the names of the category pickers to the components for fast lookup
    // Used for navigating to a bookmark in the notebook
    private Dictionary<string, ResearchCategoryPicker> namePickerMap = new Dictionary<string, ResearchCategoryPicker>();
    private bool isOpen = false;

    // I thought that this was called when the game object is inactive but apparently it is not
    private void Awake()
    {
        // Setup the notebook at the start
        notebook.Setup();

        // Update the enclosure IDs
        notebook.TryAddEnclosureID(EnclosureID.FromCurrentSceneName());

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
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            string numberString = SceneManager.GetActiveScene().name;
            numberString = numberString.Substring("TestLevel".Length, 1);
            int number = int.Parse(numberString);
            SceneManager.LoadScene("TestLevel" + (number % 3 + 1));
        }
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
}
