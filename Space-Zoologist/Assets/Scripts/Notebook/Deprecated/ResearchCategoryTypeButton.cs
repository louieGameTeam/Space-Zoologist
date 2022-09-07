using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class ResearchCategoryTypeButton : MonoBehaviour
{
    // Typedef so it will appear in the editor
    [System.Serializable]
    public class ItemCategoryEvent : UnityEvent<ItemRegistry.Category> { }

    // Public accessors
    public Toggle MyToggle => myToggle;

    [SerializeField]
    [Tooltip("The toggle that manages this button")]
    private Toggle myToggle = null;
    [SerializeField]
    [Tooltip("Text displayed in the button")]
    private TextMeshProUGUI text = null;
    [SerializeField]
    [Tooltip("Event invoked when this button is selected")]
    private ItemCategoryEvent onSelected = null;

    [Tooltip("This button's type")]
    private ItemRegistry.Category type;

    public void Setup(ToggleGroup group, ItemRegistry.Category type, UnityAction<ItemRegistry.Category> callback, bool isOn)
    {
        // Set the toggle group for this toggle
        myToggle.group = group;
        // Set the type for this button
        this.type = type;
        // Add the callback to the on selected listener
        onSelected.AddListener(callback);

        // Set the text on the button
        text.text = type.ToString();
        // Add function to callback when the toggle is switched to on
        myToggle.onValueChanged.AddListener(OnToggleStateChanged);

        // Set the desired initial toggle state
        myToggle.isOn = isOn;
    }

    // If the toggle is toggling on, invoke on selected event
    private void OnToggleStateChanged(bool state)
    {
        if (state)
        {
            onSelected.Invoke(type);
        }
    }
}
