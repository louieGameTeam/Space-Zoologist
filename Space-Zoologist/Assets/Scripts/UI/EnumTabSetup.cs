using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnumTabSetup<TEnum> : MonoBehaviour
    where TEnum : System.Enum
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the toggle group picker used to select a toggle")]
    private AbstractToggleGroupPicker toggleGroupPicker;
    [SerializeField]
    [Tooltip("Prefab of the toggle picker to instantiate for each enum value")]
    private AbstractTogglePicker togglePickerPrefab;
    [SerializeField]
    [Tooltip("Toggle group used to make only one toggle active at a time")]
    private ToggleGroup toggleGroup;
    [SerializeField]
    [Tooltip("Transform that all of the toggle are instantiated under")]
    private Transform toggleParent;
    [SerializeField]
    [Tooltip("List of tabs to enable/disable for each enum")]
    private GameObject[] enumTabs;
    [SerializeField]
    [Tooltip("List of enums to exclude from the tab setup")]
    private List<TEnum> exclusions;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Get enums with the applied filter
        IEnumerable<TEnum> enums = System.Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(Filter);
        // A list of all the toggles we created
        List<AbstractTogglePicker> toggles = new List<AbstractTogglePicker>();

        foreach (TEnum e in enums)
        {
            // Create a toggle and cast it to a generic toggle
            AbstractTogglePicker toggle = Instantiate(togglePickerPrefab, toggleParent);
            toggle.ObjectPicked = e;
            toggle.Toggle.group = toggleGroup;
            toggles.Add(toggle);
        }

        // Set the toggle pickers on the toggle group
        toggleGroupPicker.SetTogglePickers(toggles);
        // When the toggle state of any toggle changes then update the tabs displayed
        toggleGroupPicker.OnToggleStateChanged.AddListener(UpdateTabs);
    }
    private void OnValidate()
    {
        // Get a list of the enum names
        string[] enumNames = System.Enum.GetNames(typeof(TEnum));

        if(enumNames.Length != enumTabs.Length)
        {
            // Create a new set of tabs with the correct length
            GameObject[] newTabs = new GameObject[enumNames.Length];
            // Set the smaller length
            int smallerLength = Mathf.Min(newTabs.Length, enumTabs.Length);
            // Copy current tabs to new tabs
            System.Array.Copy(enumTabs, newTabs, smallerLength);
            // Set current tabs to new tabs
            enumTabs = newTabs;
        }
    }
    #endregion

    #region Protected Methods
    // Include enums that are not excluded
    protected virtual bool Filter(TEnum enumValue) => !exclusions.Contains(enumValue);
    protected virtual void UpdateTabs()
    {
        // Cache the objects that have been picked
        List<object> pickedEnums = toggleGroupPicker.ObjectsPicked;
        // List all enum names
        TEnum[] enums = (TEnum[])System.Enum.GetValues(typeof(TEnum));

        // Set the tabs on or off depending on if their enum index is picked
        for(int i = 0; i < enums.Length; i++)
        {
            if(enumTabs[i])
            {
                bool isPicked = pickedEnums.Contains(enums[i]);
                enumTabs[i].SetActive(isPicked);
            }
        }
    }
    #endregion
}
