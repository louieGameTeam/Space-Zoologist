using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class NeedTypeDropdown : NotebookUIChild
{
    [System.Serializable]
    public class NeedTypeEvent : UnityEvent<NeedType> { }

    public NeedTypeEvent OnNeedTypeSelected => onNeedTypeSelected;

    [SerializeField]
    [Tooltip("Reference to the dropdown used to select the need type")]
    private TMP_Dropdown dropdown;
    [SerializeField]
    [Tooltip("List of selectable needs in this dropdown")]
    private List<NeedType> needs;
    [SerializeField]
    [Tooltip("Event raised when the dropdown value changes")]
    private NeedTypeEvent onNeedTypeSelected;

    protected override void Awake()
    {
        base.Awake();

        // Clear any existing options
        dropdown.ClearOptions();

        // Create a dropdown option for each need
        foreach(NeedType need in needs)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(need.ToString() + " Need");
            dropdown.options.Add(option);
        }

        // Add a listener for the dropdown value change
        dropdown.onValueChanged.AddListener(SetDropdownValue);
    }

    public void SetDropdownValue(int value)
    {
        onNeedTypeSelected.Invoke(needs[value]);
    }
}
