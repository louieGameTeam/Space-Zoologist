using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class ItemDropdown : NotebookUIChild
{
    #region Public Typedefs
    [System.Serializable]
    public class ItemIDEvent : UnityEvent<ItemID> { }
    #endregion

    #region Public Properties
    public TMP_Dropdown Dropdown => dropdown;
    public ItemIDEvent OnItemSelected => onItemSelected;
    public ItemID SelectedItem => optionCategoryMap[dropdown.options[dropdown.value]];
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the dropdown used to select the research category")]
    protected TMP_Dropdown dropdown = null;
    [SerializeField]
    [Tooltip("Name to display for the item in the dropdown")]
    private ItemName.Type itemDisplayName = ItemName.Type.Colloquial;
    [SerializeField]
    [Tooltip("True if text and image should display simultaneously")]
    protected bool textAndImage = false;
    [SerializeField]
    [Tooltip("Event invoked when this dropdown selects a research category")]
    protected ItemIDEvent onItemSelected = null;
    #endregion

    #region Private Fields
    // Maps a selected item in the dropdown to a research category
    // NOTE: why don't we just change this to two conversion functions to change betweeen types?
    // NOTE: CAN'T do that because cannot find an item on the registry from its name, you need the index
    protected Dictionary<TMP_Dropdown.OptionData, ItemID> optionCategoryMap = new Dictionary<TMP_Dropdown.OptionData, ItemID>();
    // source
    private ItemID[] currentSource;
    #endregion

    #region Public Methods
    /// <summary>
    /// Set ItemID source to custom source (Entire ItemRegistry by default)
    /// </summary>
    /// <param name=""></param>
    public void SetSource(ItemID[] source)
    {
        currentSource = source;
    }
    public override void Setup()
    {
        base.Setup();
        // Clear any existing data
        dropdown.ClearOptions();
        optionCategoryMap.Clear(); 
        foreach(ItemID id in GetItemIDs(currentSource))
        {
            // Get the current option
            ItemData data = ItemRegistry.Get(id);
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(data.Name.GetName(ItemName.Type.Colloquial), data.ShopItem.Icon);
            
            // Add the option to the dropdown and the dictionary
            dropdown.options.Add(option);
            optionCategoryMap.Add(option, id);
        }

        // Setup the value changed callback
        dropdown.onValueChanged.AddListener(SetDropdownValue);
        // Setup the value to the first one
        if (dropdown.options.Count > 0) SetDropdownValueWithoutNotify(0);
    }

    /// <summary>
    /// Given the item id, get its index in the dropdown list
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int DropdownIndex(ItemID id) => dropdown.options.FindIndex(option => option.text.Contains (id.Data.Name.Get(itemDisplayName)));

    // Set the value of the dropdown
    public void SetDropdownValue(int value) => SetDropdownValueHelper(value, true);
    public void SetDropdownValueWithoutNotify(int value) => SetDropdownValueHelper(value, false);

    public bool SetSelectedItem(ItemID id) => SetResearchCategoryHelper(id, v => SetDropdownValue(v));
    public bool SetSelectedItemWithoutNotify(ItemID id) => SetResearchCategoryHelper(id, v => SetDropdownValueWithoutNotify(v));
    #endregion

    #region Private Methods
    private bool SetResearchCategoryHelper(ItemID id, UnityAction<int> valueSetter)
    {
        // Find the first value in the list that matches
        TMP_Dropdown.OptionData selection = optionCategoryMap.FirstOrDefault(kvp => kvp.Value == id).Key;

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
        dropdown.captionImage.enabled = textAndImage || selection.image != null;
        dropdown.captionText.enabled = textAndImage || selection.image == null;

        // Set the value and refresh the shown value
        dropdown.value = value;
        dropdown.RefreshShownValue();

        // If we are notifying then raise the event
        if (notify) onItemSelected.Invoke(optionCategoryMap[selection]);
    }
    protected virtual ItemID[] GetItemIDs(ItemID[] source)
    {
        // use all items as source if no source specified
        return (source ?? ItemRegistry.GetAllItemIDs()).Where(i => UIParent.Data.ItemIsUnlocked(i)).ToArray();
    }
    #endregion
}
