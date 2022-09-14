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
    private AbstractToggleGroupPicker toggleGroupPicker = null;
    [SerializeField]
    [Tooltip("Prefab of the toggle picker to instantiate for each enum value")]
    private AbstractTogglePicker togglePickerPrefab = null;
    [SerializeField]
    [Tooltip("Toggle group used to make only one toggle active at a time")]
    private ToggleGroup toggleGroup = null;
    [SerializeField]
    [Tooltip("Transform that all of the toggle are instantiated under")]
    private Transform toggleParent = null;
    [SerializeField]
    [Tooltip("If true, the enum list marks enums to INCLUDE, if false, it marks enums to EXCLUDE")]
    private bool filterMarksInclusions = false;
    [SerializeField]
    [Tooltip("List of enums to exclude/include from the tab setup")]
    private List<TEnum> filter = null;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Get enums with the applied filter
        IEnumerable<TEnum> enums = GetFilteredEnums();
        // A list of all the toggles we created
        List<AbstractTogglePicker> toggles = new List<AbstractTogglePicker>();

        foreach (TEnum e in enums)
        {
            // Create a toggle and cast it to a generic toggle
            AbstractTogglePicker toggle = Instantiate(togglePickerPrefab, toggleParent);
            toggle.SetObjectPicked(e);
            toggle.Toggle.group = toggleGroup;
            toggles.Add(toggle);
        }

        // Set the toggle pickers on the toggle group
        toggleGroupPicker.SetTogglePickers(toggles);
        // Set object picked to the first enum in the filtered list
        toggleGroupPicker.SetObjectPicked(enums.First());
    }
    #endregion

    #region Protected Methods
    // If filter marks inclusions, choose this value if the filter contains it
    // If the filter marks exclusions, choose this value if the filter doesn't contain it
    protected virtual bool Filter(TEnum enumValue) => filterMarksInclusions && filter.Contains(enumValue) || !filterMarksInclusions && !filter.Contains(enumValue);
    protected IEnumerable<TEnum> GetFilteredEnums() => System.Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(Filter);
    #endregion
}
