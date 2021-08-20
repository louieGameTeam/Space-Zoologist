using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class EnumerablePicker
{
    // Public typedef - so that event will appear in the editor
    [System.Serializable]
    public class EnumerableItemEvent : UnityEvent<EnumerableItem> { }

    // Get the item that is currently selected
    // We may need a setter?
    public EnumerableItem SelectedItem => selectedItem;

    // Private editor fields
    [SerializeField]
    [Tooltip("Groups together all of the enumerable buttons")]
    private ToggleGroup buttonGroup;
    [SerializeField]
    [Tooltip("Reference to the prefab of the button to instantiate")]
    private EnumerableButton buttonPrefab;
    [SerializeField]
    [Tooltip("Event invoked when a button is clicked")]
    private EnumerableItemEvent onItemSelected;

    // Maps the item of the button to the button for fast lookup
    private Dictionary<EnumerableItem, EnumerableButton> itemButtonMap = new Dictionary<EnumerableItem, EnumerableButton>();
    // Reference to the item that is currently selected
    private EnumerableItem selectedItem = null;

    private EnumerablePicker(ToggleGroup buttonGroup, EnumerableButton buttonPrefab, EnumerableItemEvent onItemSelected)
    {
        this.buttonGroup = buttonGroup;
        this.buttonPrefab = buttonPrefab;
        this.onItemSelected = onItemSelected;
    }

    public EnumerablePicker(EnumerablePicker other) : this(other.buttonGroup, other.buttonPrefab, other.onItemSelected) { }

    public void Setup(UnityAction<EnumerableItem> callback, params EnumerableItem[] items)
    {
        // Create a button for each enumerable item
        foreach(EnumerableItem item in items)
        {
            // Create the button and set it up
            EnumerableButton instance = Object.Instantiate(buttonPrefab, buttonGroup.transform);
            instance.Setup(buttonGroup, item, OnItemSelectedMethod);
            // Add the button to the map of buttons
            itemButtonMap.Add(item, instance);
        }
        // Add listener for the given callback
        onItemSelected.AddListener(callback);
    }
    private void OnItemSelectedMethod(EnumerableItem item)
    {
        selectedItem = item;
        onItemSelected.Invoke(item);
    }
    public void SetActive(bool active)
    {
        foreach(KeyValuePair<EnumerableItem, EnumerableButton> kvp in itemButtonMap)
        {
            kvp.Value.gameObject.SetActive(active);
        }
        // The button previously selected will be on when activated and disabled when deactivated
        if (selectedItem != null) itemButtonMap[selectedItem].SetToggleState(active);
    }
    // NOTE: invokes "OnItemSelectedMethod" immediately
    public void SelectButton(EnumerableItem item)
    {
        itemButtonMap[item].SetToggleState(true);
    }
}
