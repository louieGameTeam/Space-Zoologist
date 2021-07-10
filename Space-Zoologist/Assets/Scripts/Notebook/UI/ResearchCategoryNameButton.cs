using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using TMPro;

public class ResearchCategoryNameButton : MonoBehaviour
{
    // Typedef so it will appear in the editor
    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    // Public accessors
    public Toggle MyToggle => myToggle;

    [SerializeField]
    [Tooltip("The toggle that manages this button")]
    private Toggle myToggle;
    [SerializeField]
    [Tooltip("Text displayed in the button")]
    private TextMeshProUGUI text;
    [SerializeField]
    [Tooltip("Event invoked when this button is selected")]
    private StringEvent onSelected;

    [Tooltip("This button's name")]
    private string researchCategoryName;

    public void Setup(ToggleGroup group, string researchCategoryName, UnityAction<string> callback)
    {
        // Set the toggle group for this toggle
        myToggle.group = group;
        // Set the name for this button
        this.researchCategoryName = researchCategoryName;
        // Add the callback to the on selected listener
        onSelected.AddListener(callback);

        // Set the text on the button
        text.text = researchCategoryName;
        // Add function to callback when the toggle is switched to on
        myToggle.onValueChanged.AddListener(OnToggleStateChanged);

        // Set initial toggle state
        //myToggle.isOn = isOn;
    }

    // If the toggle is toggling on, invoke on selected event
    private void OnToggleStateChanged(bool state)
    {
        if (state) onSelected.Invoke(researchCategoryName);
    }
}
