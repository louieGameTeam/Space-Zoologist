using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

public class NotebookDebugging : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public GameObject itemRoot;
    public Toggle toggle;
    
    private int index = 0;
    
    private void Start()
    {
        string indexString = itemRoot.name;
        string prefix = "Index ";
        int endSubstringIndex = indexString.IndexOf(':');

        indexString = indexString.Substring(prefix.Length - 1, endSubstringIndex - prefix.Length + 1);
        index = int.Parse(indexString);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            toggle.isOn = true;
            toggle.onValueChanged.Invoke(true);
            dropdown.onValueChanged.Invoke(index);
        });
    }
}
