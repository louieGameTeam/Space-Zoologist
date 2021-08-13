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
    [Tooltip("Reference to the dropdown used to select the enclosure ID")]
    private TMP_Dropdown dropdown;
    [SerializeField]
    [Tooltip("Event invoked when an enclosure ID is picked")]
    private EnclosureIDEvent onEnclosureIDPicked;

    private const string levelPrefix = "Level ";
    private const string enclosurePrefix = ", Enclosure ";

    public override void Setup()
    {
        base.Setup();

        // Setup the options in the dropdown 
        dropdown.ClearOptions();
        foreach(EnclosureID id in UIParent.Notebook.EnclosureIDs)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(levelPrefix + id.LevelNumber + enclosurePrefix + id.EnclosureNumber);
            dropdown.options.Add(option);
        }

        // Set the dropdown value and invoke the event
        dropdown.value = 0;
        dropdown.RefreshShownValue();
        OnDropdownValueChanged(0);
    }

    private void OnDropdownValueChanged(int value)
    {
        TMP_Dropdown.OptionData currentOption = dropdown.options[value];
        EnclosureID currentID = OptionDataToEnclosureID(currentOption);
        onEnclosureIDPicked.Invoke(currentID);
    }

    private EnclosureID OptionDataToEnclosureID(TMP_Dropdown.OptionData option)
    {
        // Get the indices in the string where the numbers should be found
        int indexOfLevelNumber = levelPrefix.Length;
        int indexOfEnclosureNumber = option.text.IndexOf(enclosurePrefix);
        // Compute the substrings where the numbers
        string levelNumberSubstring = option.text.Substring(indexOfLevelNumber, indexOfEnclosureNumber - indexOfLevelNumber);
        string enclosureNumberSubstring = option.text.Substring(indexOfEnclosureNumber + enclosurePrefix.Length);
        // Parse the substrings
        int levelNumber = int.Parse(levelNumberSubstring);
        int enclosureNumber = int.Parse(enclosureNumberSubstring);
        return new EnclosureID(levelNumber, enclosureNumber);
    }
}
