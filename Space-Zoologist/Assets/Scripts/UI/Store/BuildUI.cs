using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class BuildUI : MonoBehaviour
{
    #region Public Properties
    public IntToggleGroupPicker StoreSectionIndexPicker => storeSectionIndexPicker;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Toggle group that picks the store menu to show")]
    [FormerlySerializedAs("indexPicker")]
    private IntToggleGroupPicker storeSectionIndexPicker = null;
    [SerializeField]
    [Tooltip("Button that closes the build UI when clicked")]
    private Button closeButton = null;
    #endregion

    #region Private Fields
    private MenuManager menuManager;
    #endregion

    #region Monobehaviour Messages
    // Start is called before the first frame update
    void Start()
    {
        // Get the menu manager
        menuManager = FindObjectOfType<MenuManager>();
        // Set toggle picked to the first one
        storeSectionIndexPicker.SetTogglePicked(0);

        // Open the menu on the menu manager
        storeSectionIndexPicker.OnToggleStateChanged.AddListener(() =>
        {
            menuManager.OpenMenu(storeSectionIndexPicker.FirstValuePicked);
        });

        // Call the close function when the close button is clicked
        closeButton.onClick.AddListener(Close);
    }
    #endregion

    #region Public Methods
    public void Close()
    {
        // Set the store to off
        menuManager.SetStoreIsOn(false);
    }
    #endregion
}
