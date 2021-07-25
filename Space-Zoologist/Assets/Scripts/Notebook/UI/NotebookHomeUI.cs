using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class NotebookHomeUI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the scriptable object to get/set general notes from")]
    private Notebook notebookObject;
    [SerializeField]
    [Tooltip("Reference to the input field used to write general notes")]
    private TMP_InputField inputField;

    private void Awake()
    {
        inputField.onEndEdit.AddListener(SetGeneralNotes);
    }
    private void SetGeneralNotes(string notes)
    {
        notebookObject.GeneralNotes = notes;
    }
}
