using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class ResearchCategoryDropdown : NotebookUIChild
{
    [System.Serializable]
    public class ResearchCategoryEvent : UnityEvent<ResearchCategory> { }

    // Public accessors
    public ResearchCategoryType Type => type;
    public TMP_Dropdown Dropdown => dropdown;

    // Private editor data

    [SerializeField]
    [Tooltip("Reference to the dropdown used to select the research category")]
    private TMP_Dropdown dropdown;
    [SerializeField]
    [Tooltip("Event invoked when this dropdown selects a research category")]
    private ResearchCategoryEvent onResearchCategorySelected;

    // Maps a selected item in the dropdown to a research category
    private Dictionary<TMP_Dropdown.OptionData, ResearchCategory> optionCategoryMap = new Dictionary<TMP_Dropdown.OptionData, ResearchCategory>();
    private ResearchCategoryType type;

    public void Setup(ResearchCategoryType type, UnityAction<ResearchCategory> callback)
    {
        base.Awake();

        // Set this type
        this.type = type;

        // Select all categories with the given type
        ResearchCategory[] categories = UIParent.Notebook.Research.ResearchDictionary
            .Where(kvp => kvp.Key.Type == type)
            .Select(kvp => kvp.Key)
            .ToArray();

        // Clear any existing options
        dropdown.ClearOptions();

        // Setup the dropdown with all the correct text/image options
        foreach(ResearchCategory category in categories)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(category.Name, category.Image);
            dropdown.options.Add(option);
            optionCategoryMap.Add(option, category);
        }

        // Setup the value changed callback
        dropdown.onValueChanged.AddListener(SetDropdownValue);
        onResearchCategorySelected.AddListener(callback);

        // Set the dropdown value
        SetDropdownValueWithoutNotify(0);
    }

    // Set the value of the dropdown
    public void SetDropdownValue(int value) => SetDropdownValueHelper(value, true);
    public void SetDropdownValueWithoutNotify(int value) => SetDropdownValueHelper(value, false);

    public bool SetResearchCategory(ResearchCategory category) => SetResearchCategoryHelper(category, v => SetDropdownValue(v));
    public bool SetResearchCategoryWithoutNotify(ResearchCategory category) => SetResearchCategoryHelper(category, v => SetDropdownValueWithoutNotify(v));

    private bool SetResearchCategoryHelper(ResearchCategory category, UnityAction<int> valueSetter)
    {
        // Find the first value in the list that matches
        TMP_Dropdown.OptionData selection = optionCategoryMap.FirstOrDefault(kvp => kvp.Value == category).Key;

        if (selection != null)
        {
            int value = dropdown.options.FindIndex(option => option == selection);

            // If the option was found, then invoke the event
            if (value >= 0)
            {
                valueSetter.Invoke(value);
                return true;
            }
            else return false;
        }
        else return false;
    }
    private void SetDropdownValueHelper(int value, bool notify)
    {
        // Get the selected option
        TMP_Dropdown.OptionData selection = dropdown.options[value];

        // Display image if it is not null, otherwise display text
        dropdown.captionImage.enabled = selection.image != null;
        dropdown.captionText.enabled = selection.image == null;

        // Set the value and refresh the shown value
        dropdown.value = value;
        dropdown.RefreshShownValue();

        // If we are notifying then raise the event
        if (notify) onResearchCategorySelected.Invoke(optionCategoryMap[selection]);
    }
}
