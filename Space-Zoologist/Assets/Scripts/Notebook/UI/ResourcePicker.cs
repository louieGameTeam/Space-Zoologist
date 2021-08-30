using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ResourcePicker : NotebookUIChild
{
    #region Typedefs
    [System.Serializable]
    public class ItemDataEvent : UnityEvent<Item> { }
    #endregion

    #region Public Properties
    public TMP_Dropdown Dropdown => dropdown;
    public ItemDataEvent OnItemSelected => onItemSelected;
    public Item ItemSelected
    {
        get => OptionDataToItemData(dropdown.options[dropdown.value]);
        set
        {
            TMP_Dropdown.OptionData option = ItemDataToOptionData(value);
            int index = dropdown.options.FindIndex(x => x.text == option.text && x.image == option.image);

            // If the item was found then set the dropdown value.  NOTE: invokes "on changed" event immediately
            if (index >= 0) dropdown.value = index;
            else
            {
                Debug.Log("ResourcePicker: tried to set item selected to an item that does not exist in the dropdown," +
                    " so the value will be ignored");
            }
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the dropdown used to pick the resource")]
    private TMP_Dropdown dropdown;
    [SerializeField]
    [Tooltip("Event invoked when a resource is selected from the list")]
    private ItemDataEvent onItemSelected;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Add all options to the list of options
        dropdown.ClearOptions();
        // Only add options that are not pods so we cannot request animals as resources
        foreach(LevelData.ItemData item in UIParent.LevelDataReference.LevelData.ItemQuantities)
        {
            if(item.itemObject.Type != ItemType.Pod)
            {
                dropdown.options.Add(ItemDataToOptionData(item.itemObject));
            }
        }

        // Add listener to the value changed event
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        // Set the value now
        dropdown.value = 0;
    }
    #endregion

    #region Private Methods
    private void OnDropdownValueChanged(int value)
    {
        onItemSelected.Invoke(ItemSelected);
    }

    private TMP_Dropdown.OptionData ItemDataToOptionData(Item item)
    {
        return new TMP_Dropdown.OptionData(item.ItemName, item.Icon);
    }
    private Item OptionDataToItemData(TMP_Dropdown.OptionData option)
    {
        List<LevelData.ItemData> itemDatas = UIParent.LevelDataReference.LevelData.ItemQuantities;
        itemDatas = itemDatas.FindAll(x => x.itemObject.ItemName == option.text && x.itemObject.Icon == option.image);

        if (itemDatas.Count <= 0) return null;
        else
        {
            // This should never happen, but we log a warning just in case
            if(itemDatas.Count > 1)
            {
                Debug.LogWarning("ResourcePicker: found multiple item datas with the same item name and icon, " +
                    "so we will have to pick the first one");
            }

            return itemDatas[0].itemObject;
        }
    }
    #endregion
}
