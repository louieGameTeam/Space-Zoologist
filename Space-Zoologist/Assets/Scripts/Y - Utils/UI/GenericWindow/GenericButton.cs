using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

[System.Serializable]
public class GenericButton
{
    #region Public Properties
    public Button Button => button;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the button in the generic window")]
    private Button button;
    [SerializeField]
    [Tooltip("Reference to the text displayed in the button")]
    private TextMeshProUGUI buttonText;
    #endregion

    #region Public Methods
    public void Setup(GenericButtonData data)
    {
        button.onClick.AddListener(data.ButtonAction.Invoke);
        buttonText.text = data.ButtonText;
    }
    #endregion
}
