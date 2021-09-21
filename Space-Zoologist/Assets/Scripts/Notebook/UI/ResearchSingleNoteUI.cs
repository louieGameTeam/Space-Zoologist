using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class ResearchSingleNoteUI : NotebookUIChild
{
    #region Public Typedefs
    // So that it appears in the editor
    [System.Serializable] public class StringStringEvent : UnityEvent<string, string> { }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Text used to display the label for this note")]
    private TextMeshProUGUI labelText;
    [SerializeField]
    [Tooltip("Input field used to write the note")]
    private TMP_InputField myInputField;
    #endregion

    #region Public Methods
    public void Setup(ItemID id, string label, ScrollRect scrollTarget)
    {
        // Setup the UI child base
        base.Setup();

        // Read the initial note value
        string initialNote = UIParent.Notebook.Research.GetEntry(id).Notes.ReadNote(label);

        // Setup the initial note and label
        labelText.text = label + ":";
        myInputField.text = initialNote;

        // When input finishes editing then write the note to the notebook model
        myInputField.onEndEdit.AddListener(s =>
        {
            UIParent.Notebook.Research.GetEntry(id).Notes.WriteNote(label, s);
        });

        // Make the scroll event of the input field target the scroll rect
        OnScrollEventInterceptor interceptor = myInputField.gameObject.AddComponent<OnScrollEventInterceptor>();
        interceptor.InterceptTarget = scrollTarget;
    }
    #endregion
}
