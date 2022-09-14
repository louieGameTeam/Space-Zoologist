using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCategoryTabSetup : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip ("Prefab of the toggle picker to instantiate for each enum value")]
    private GameObject togglePickerPrefab;
    [SerializeField]
    [Tooltip ("Toggle group used to make only one toggle active at a time")]
    private ToggleGroup toggleGroup;
    [SerializeField]
    [Tooltip ("Transform that all of the toggle are instantiated under")]
    private Transform toggleParent;
    [SerializeField]
    [Tooltip ("If true, the enum list marks enums to INCLUDE, if false, it marks enums to EXCLUDE")]
    private bool filterMarksInclusions = false;
    [SerializeField]
    [Tooltip ("List of enums to exclude/include from the tab setup")]
    private List<ItemRegistry.Category> filter;
    #endregion

    private AbstractToggleGroupPicker toggleGroupPicker;

    #region Monobehaviour Messages
    private void Start () {
        // Get enums with the applied filter
        IEnumerable<ItemRegistry.Category> enums = GetFilteredEnums ();
        // A list of all the toggles we created
        List<AbstractTogglePicker> toggles = new List<AbstractTogglePicker> ();

        foreach (ItemRegistry.Category e in enums) {
            // Create a toggle and cast it to a generic toggle
            AbstractTogglePicker toggle = Instantiate (togglePickerPrefab, toggleParent).GetComponent<AbstractTogglePicker>();
            toggle.SetObjectPicked (e);
            toggle.Toggle.group = toggleGroup;
            toggles.Add (toggle);
        }

        toggleGroupPicker = GetComponent<AbstractToggleGroupPicker>();

        // Set the toggle pickers on the toggle group
        toggleGroupPicker.SetTogglePickers (toggles);
        // Set object picked to the first enum in the filtered list
        toggleGroupPicker.SetObjectPicked (enums.First ());
    }
    #endregion

    #region Protected Methods
    // If filter marks inclusions, choose this value if the filter contains it
    // If the filter marks exclusions, choose this value if the filter doesn't contain it
    protected virtual bool Filter (ItemRegistry.Category enumValue) => filterMarksInclusions && filter.Contains (enumValue) || !filterMarksInclusions && !filter.Contains (enumValue);
    protected IEnumerable<ItemRegistry.Category> GetFilteredEnums () => System.Enum.GetValues (typeof (ItemRegistry.Category))
            .Cast<ItemRegistry.Category> ()
            .Where (Filter);
    #endregion*/
}
