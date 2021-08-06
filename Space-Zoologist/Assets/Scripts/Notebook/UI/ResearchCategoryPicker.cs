using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ResearchCategoryPicker : NotebookUIChild
{
    // So that the event appears in the editor
    [System.Serializable] public class ResearchCategoryEvent : UnityEvent<ResearchCategory> { }

    // Public accessors

    public ResearchCategory SelectedCategory
    {
        get => selectedCategory;
        set
        {
            // Activate the type button with this type (this invokes OnResearchCategoryTypeChanged)
            typeButtons[(int)value.Type].MyToggle.isOn = true;
            // Activate the name button with this name (this invokes OnResearchCategoryNameChanged)
            nameButtonGroups[(int)value.Type].SelectButton(value.Name);
        }
    }
    public ResearchCategoryEvent OnResearchCategoryChanged => onResearchCategoryChanged;
    public bool HasBeenInitialized => SelectedCategory.Name != null && SelectedCategory.Name != "";

    // Private editor fields

    [Header("Research Category Type Selection")]

    [SerializeField]
    [Tooltip("This toggle group will be the parent of all toggles used to select the research category's type")]
    private ToggleGroup typeGroup;
    [SerializeField]
    [Tooltip("Prefab of the button used to select the research category type")]
    private ResearchCategoryTypeButton typeButton;

    [Header("Research Category Name Selection")]

    [SerializeField]
    [Tooltip("This toggle group will be the parent of all toggles used to select the research category's name")]
    private ToggleGroup nameGroup;
    [SerializeField]
    [Tooltip("Prefab of the button used to select the research name")]
    private ResearchCategoryNameButton nameButton;

    [Header("Events")]

    [SerializeField]
    [Tooltip("Event invoked when the research category picker changes category picked")]
    private ResearchCategoryEvent onResearchCategoryChanged;

    // The category currently selected
    private ResearchCategory selectedCategory;
    // List of name button groups
    // NOTE: access the buttons for category type using nameButtonGroups[(int)ResearchCategoryType]
    private List<ResearchCategoryNameButtonGroup> nameButtonGroups = new List<ResearchCategoryNameButtonGroup>();
    // List of buttons used to select the type
    // NOTE: access the buttons by type using typeButtons[(int)ResearchCategoryType]
    private List<ResearchCategoryTypeButton> typeButtons = new List<ResearchCategoryTypeButton>();

    protected override void Awake()
    {
        base.Awake();

        // Go through all research categories in the research model, adding buttons to each group in the list
        foreach (KeyValuePair<ResearchCategory, ResearchEntry> entry in UIParent.NotebookModel.NotebookResearch.ResearchDictionary)
        {
            // Add groups until the count exceeds the current index we need to insert into
            while(nameButtonGroups.Count <= (int)entry.Key.Type)
            {
                nameButtonGroups.Add(new ResearchCategoryNameButtonGroup());
            }            

            // Create a clone and set it up
            ResearchCategoryNameButton clone = Instantiate(nameButton, nameGroup.transform);
            clone.Setup(nameGroup, entry.Key.Name, entry.Key.Image, OnResearchCategoryNameChanged);
            // Add it to the current category name button group
            nameButtonGroups[(int)entry.Key.Type].AddButton(clone);
        }

        // Disable all button groups initially
        foreach (ResearchCategoryNameButtonGroup group in nameButtonGroups)
        {
            group.SetActive(false);
        }

        // Get all research category types
        ResearchCategoryType[] types = (ResearchCategoryType[])System.Enum.GetValues(typeof(ResearchCategoryType));
        // Loop through all types.  Instantiate a button for each type
        for (int i = 0; i < types.Length; i++)
        {
            ResearchCategoryTypeButton clone = Instantiate(typeButton, typeGroup.transform);
            // Setup clone. The first one is selected.
            // NOTE: this invokes OnResearchCategoryTypeChanged immediately
            clone.Setup(typeGroup, types[i], OnResearchCategoryTypeChanged, i == 0);
            typeButtons.Add(clone);
        }
    }

    private void OnResearchCategoryTypeChanged(ResearchCategoryType type)
    {
        // If a previous category has been selected, then disable it
        if(selectedCategory.Name != null)
        {
            nameButtonGroups[(int)selectedCategory.Type].SetActive(false);   
        }

        // Create the currently selected category
        selectedCategory = new ResearchCategory(type, "", null);

        // Enable the new name button group
        // NOTE: this invokes OnResearchCategoryNameChanged immediately
        nameButtonGroups[(int)type].SetActive(true);
    }

    private void OnResearchCategoryNameChanged(string name, Sprite image)
    {
        selectedCategory = new ResearchCategory(selectedCategory.Type, name, image);

        // Invoke the event
        onResearchCategoryChanged.Invoke(selectedCategory);
    }
}
