using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class NotebookHomeUI : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Reference to the input field used to write general notes")]
    private TMP_InputField inputField;

    protected override void Awake()
    {
        base.Awake();
        inputField.onEndEdit.AddListener(SetGeneralNotes);
    }
    private void SetGeneralNotes(string notes)
    {
        UIParent.NotebookModel.GeneralNotes = notes;
    }
}
