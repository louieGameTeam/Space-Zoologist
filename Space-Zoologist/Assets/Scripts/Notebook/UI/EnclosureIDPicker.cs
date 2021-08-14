using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// UI element used to pick an enclosure ID
/// </summary>
public class EnclosureIDPicker : NotebookUIChild
{
    [System.Serializable] public class EnclosureIDEvent : UnityEvent<EnclosureID> { }

    // Public accessors
    public EnclosureIDEvent OnEnclosureIDPicked => onEnclosureIDPicked;

    [SerializeField]
    [Tooltip("Reference to the dropdown used to select the level number")]
    private TMP_Dropdown levelDropdown;
    [SerializeField]
    [Tooltip("Reference to the dropdown used to select the enclosure number")]
    private TMP_Dropdown enclosureDropdown;
    [SerializeField]
    [Tooltip("Event invoked when an enclosure ID is picked")]
    private EnclosureIDEvent onEnclosureIDPicked;

    private const string levelPrefix = "Level ";
    private const string enclosurePrefix = "Enclosure ";

    public override void Setup()
    {
        base.Setup();

        // Clear out any existing options
        levelDropdown.ClearOptions();
        enclosureDropdown.ClearOptions();

        // Loop through all enclosure id's and add them to the list
        foreach(EnclosureID id in UIParent.Notebook.EnclosureIDs)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(levelPrefix + id.LevelNumber);
            // If no option with the same text yet exists, then add it to the dropdown
            if(levelDropdown.options.FindIndex(x => x.text == option.text) < 0)
            {
                levelDropdown.options.Add(option);
            }
        }

        // Update the level dropdown to reflect the current level
        EnclosureID currentEnclosure = EnclosureID.FromCurrentSceneName();
        levelDropdown.value = currentEnclosure.LevelNumber;
        levelDropdown.RefreshShownValue();
        OnLevelDropdownValueChanged(levelDropdown.value);

        // Add listeners for the value changed events
        levelDropdown.onValueChanged.AddListener(OnLevelDropdownValueChanged);
        enclosureDropdown.onValueChanged.AddListener(OnEnclosureDropdownValueChanged);
    }

    private void OnLevelDropdownValueChanged(int value)
    {
        int selectedLevel = OptionDataToLevelNumber(levelDropdown.options[value]);

        // Clear out the options in the enclosure dropdown
        enclosureDropdown.ClearOptions();
        foreach(EnclosureID id in UIParent.Notebook.EnclosureIDs)
        {
            if (id.LevelNumber == selectedLevel) enclosureDropdown.options.Add(new TMP_Dropdown.OptionData(enclosurePrefix + id.EnclosureNumber));
        }

        // Get the enclosure represented in the current scene
        EnclosureID currentEnclosure = EnclosureID.FromCurrentSceneName();
        // If we selected the current level, then select the current enclosure number
        if (currentEnclosure.LevelNumber == selectedLevel)
        {
            enclosureDropdown.value = currentEnclosure.EnclosureNumber - 1;
        }
        else enclosureDropdown.value = 0;

        // Set the enclosure to the first one selected
        enclosureDropdown.RefreshShownValue();
        OnEnclosureDropdownValueChanged(enclosureDropdown.value);
    }
    private void OnEnclosureDropdownValueChanged(int value)
    {
        TMP_Dropdown.OptionData levelOptionSelected = levelDropdown.options[levelDropdown.value];
        TMP_Dropdown.OptionData enclosureOptionSelected = enclosureDropdown.options[value];
        EnclosureID enclosureSelected = new EnclosureID(OptionDataToLevelNumber(levelOptionSelected), OptionDataToEnclosureNumber(enclosureOptionSelected));
        onEnclosureIDPicked.Invoke(enclosureSelected);
    }

    // Private conversions from option data to numbers and back
    private int OptionDataToLevelNumber(TMP_Dropdown.OptionData option)
    {
        return int.Parse(option.text.Substring(levelPrefix.Length));
    }
    private int OptionDataToEnclosureNumber(TMP_Dropdown.OptionData option)
    {
        return int.Parse(option.text.Substring(enclosurePrefix.Length));
    }
}
