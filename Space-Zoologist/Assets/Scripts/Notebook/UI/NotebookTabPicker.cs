using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NotebookTabPicker : NotebookUIChild
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Root object where all of the pages will be found")]
    private Transform pagesRoot;
    [SerializeField]
    [Tooltip("Prefab of the button used to select notebook tabs")]
    private NotebookTabSelectButton buttonPrefab;
    [SerializeField]
    [Tooltip("Toggle group used to make only one button selected")]
    private ToggleGroup parent;
    [SerializeField]
    [Tooltip("Reference to the bookmark target to use")]
    private BookmarkTarget bookmarkTarget;
    #endregion

    #region Private Fields
    // Current tab of the picker
    private NotebookTab currentTab;
    // List of the buttons used to select a tab
    private List<NotebookTabSelectButton> buttons = new List<NotebookTabSelectButton>();
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Setup the bookmark target
        bookmarkTarget.Setup(() => currentTab, t => SelectTab((NotebookTab)t));

        // Get all notebook tabs
        NotebookTab[] tabs = (NotebookTab[])System.Enum.GetValues(typeof(NotebookTab));
        
        // Set all pages false
        for(int i = 0; i < tabs.Length; i++)
        {
            pagesRoot.GetChild(i).gameObject.SetActive(false);
        }
        
        for(int i = 0; i < tabs.Length; i++)
        {
            NotebookTabSelectButton button = Instantiate(buttonPrefab, parent.transform);
            // Only the first selector will be on. NOTE: this invokes "OnTabSelected" immediately
            button.Setup(tabs[i], parent, OnTagSelected);
            // Add this button to the list
            buttons.Add(button);
        }
    }
    // Select a specific notebook tab by selecting one of the buttons
    public void SelectTab(NotebookTab tab)
    {
        buttons[(int)tab].Select();
    }
    /// <summary>
    /// Get the root transform for the given notebook tab
    /// </summary>
    /// <param name="tab"></param>
    /// <returns></returns>
    public Transform GetTabRoot(NotebookTab tab)
    {
        return pagesRoot.GetChild((int)tab);
    }
    #endregion

    #region Private Methods
    private void OnTagSelected(NotebookTab tab)
    {
        // Disable the current page and enable the new page
        GetTabRoot(currentTab).gameObject.SetActive(false);
        GetTabRoot(tab).gameObject.SetActive(true);
        currentTab = tab;
    }
    #endregion
}
