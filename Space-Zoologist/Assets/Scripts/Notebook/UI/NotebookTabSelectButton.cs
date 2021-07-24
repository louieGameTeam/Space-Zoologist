using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class NotebookTabSelectButton : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Toggle that selects the notebook tab")]
    private Toggle myToggle;
    [SerializeField]
    [Tooltip("Text that displays the name of the notebook tab this button navigates to")]
    private TextMeshProUGUI tabName;
    [SerializeField]
    [Tooltip("Event invoked when this button is selected")]
    private NotebookTabEvent selectedEvent;

    // Notebook tab that this button navigates to
    private NotebookTab tab;

    public void Setup(NotebookTab tab, ToggleGroup parent, UnityAction<NotebookTab> callback, bool isOn)
    {
        this.tab = tab;

        // Setup the toggle 
        myToggle.group = parent;
        myToggle.onValueChanged.AddListener(OnToggleStateChanged);
        // Listen for the callback when selected event is raised
        selectedEvent.AddListener(callback);

        // Set the text on the GUI element
        tabName.text = tab.ToString();

        // Set state of the toggle.  NOTE: this invokes OnToggleStateChanged immediately
        myToggle.isOn = isOn;
    }

    private void OnToggleStateChanged(bool state)
    {
        if (state) selectedEvent.Invoke(tab);
    }
    // Select this button by setting toggle to be "on"
    // NOTE: Immediately invokes OnToggleStateChanged
    public void Select()
    {
        myToggle.isOn = true;
    }

    [System.Serializable]
    public class NotebookTabEvent : UnityEvent<NotebookTab> { }
}
