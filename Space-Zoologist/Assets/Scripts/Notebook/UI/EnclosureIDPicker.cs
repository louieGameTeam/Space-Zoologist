using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnclosureIDPicker : NotebookUIChild
{
    [SerializeField]
    [Tooltip("Reference to the dropdown used to select the enclosure ID")]
    private TMP_Dropdown dropdown;

    protected override void Awake()
    {
        base.Awake();

        
    }
}
