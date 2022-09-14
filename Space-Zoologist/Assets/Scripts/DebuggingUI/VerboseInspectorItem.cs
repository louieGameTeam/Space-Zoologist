using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VerboseInspectorItem : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Text used to display the type name of the inspected item")]
    private TextMeshProUGUI titleText = null;
    [SerializeField]
    [Tooltip("Text used to display the information of the inspected item")]
    private TextMeshProUGUI informationText = null;
    #endregion

    #region Public Methods
    public void DisplayItem(object item, string itemName = null)
    {
        if (item != null)
        {
            // If item name is left out, it defaults to the typename of the object
            if (itemName == null) itemName = item.GetType().ToString();

            // Set the title text to the name of the item
            titleText.text = itemName;

            // Display the json of the object
            informationText.text = JsonUtility.ToJson(item, true);
        }
        // If the item is null then display the text saying it is null
        else
        {
            titleText.text = "<null>";
            informationText.text = "No object displayed";
        }
    }
    #endregion
}
