using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class EnumerableButton : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the toggle that toggles this button on/off")]
    private Toggle toggle;
    [SerializeField]
    [Tooltip("Text that the button uses to display the enumerable item")]
    private TextMeshProUGUI text;
    [SerializeField]
    [Tooltip("Image that the button uses to display the enumerable item")]
    private Image image;
    [SerializeField]
    [Tooltip("Event invoked when this button is selected")]
    private EnumerablePicker.EnumerableItemEvent onSelected;

    // Reference to the item that this button picks
    private EnumerableItem item;

    public void Setup(ToggleGroup group, EnumerableItem item, UnityAction<EnumerableItem> callback)
    {
        toggle.group = group;
        this.item = item;
        onSelected.AddListener(callback);

        // Use the item to change the display of this button
        item.SetDisplay(text, image);

        // Add listener to callback when the button is selected
        onSelected.AddListener(callback);
    }

    // If the toggle is toggling on, invoke the selected event
    private void OnToggleStateChanged(bool state)
    {
        if (state) onSelected.Invoke(item);
    }

    // Select this button
    // NOTE: immediately invokes "OnToggleStateChanged"
    public void SetToggleState(bool state)
    {
        toggle.isOn = state;
    }
}
