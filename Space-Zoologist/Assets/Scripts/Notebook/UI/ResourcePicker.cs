using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ResourcePicker : NotebookUIChild
{
    #region Typedefs
    [System.Serializable]
    public class ItemEvent : UnityEvent<Item> { }
    #endregion

    #region Public Properties
    public TMP_Dropdown Dropdown => dropdown;
    public ItemEvent OnItemSelected => onItemSelected;
    public Item ItemSelected
    {
        get => OptionDataToItem(dropdown.options[dropdown.value]);
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
    private ItemEvent onItemSelected;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Add all options to the list of options
        dropdown.ClearOptions();

        // Get the instance of the game manager
        GameManager instance = GameManager.Instance;

        if(instance)
        {
            // Only add options that are not pods so we cannot request animals as resources
            foreach (LevelData.ItemData item in instance.LevelData.ItemQuantities)
            {
                if (item.itemObject.Type != ItemType.Pod && item.itemObject.Type != ItemType.Machine)
                {
                    dropdown.options.Add(ItemDataToOptionData(item.itemObject));
                }
            }
        }
        else
        {
            Debug.Log("ResourcePicker: the resource picker could not be set up " +
                "because no instance of the GameManager could be found");

            // Add some placeholder options
            dropdown.AddOptions(new List<string>()
            {
                "Item 1", "Item 2", "Item 3"
            });
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
    private Item OptionDataToItem(TMP_Dropdown.OptionData option)
    {
        GameManager instance = GameManager.Instance;

        // Double check to make sure there is a game manager
        if(instance)
        {
            List<LevelData.ItemData> itemDatas = instance.LevelData.ItemQuantities;
            itemDatas = itemDatas.FindAll(x => x.itemObject.ItemName == option.text && x.itemObject.Icon == option.image);

            if (itemDatas.Count <= 0) return null;
            else
            {
                // This should never happen, but we log a warning just in case
                if (itemDatas.Count > 1)
                {
                    Debug.LogWarning("ResourcePicker: found multiple item datas with the same item name and icon, " +
                        "so we will have to pick the first one");
                }

                return itemDatas[0].itemObject;
            }
        }
        // If instance is null, return null
        else
        {
            Debug.Log("ResourcePicker: cannot convert an option data to item if no instance " +
                "of the GameManager exists");
            return null;
        }
    }
    #endregion
}
