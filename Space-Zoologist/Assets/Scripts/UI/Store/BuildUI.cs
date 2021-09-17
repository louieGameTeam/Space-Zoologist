using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Toggle group that picks the store menu to show")]
    private IntToggleGroupPicker indexPicker;
    [SerializeField]
    [Tooltip("Button that closes the build UI when clicked")]
    private Button closeButton;
    public ItemID id;
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
        indexPicker.SetTogglePicked(0);

        // Open the menu on the menu manager
        indexPicker.OnToggleStateChanged.AddListener(() =>
        {
            menuManager.OpenMenu(indexPicker.FirstObjectPicked);
        });

        // Call the close function when the close button is clicked
        closeButton.onClick.AddListener(Close);
    }
    #endregion

    #region Public Methods
    public void Close()
    {
        // Toggle the store to off
        menuManager.ToggleStore();

        // Toggle drafting and grid overlay to off
        GameManager.Instance.m_gridSystem.ToggleDrafting();
        GameManager.Instance.m_gridSystem.ToggleGridOverlay();
    }
    #endregion
}
