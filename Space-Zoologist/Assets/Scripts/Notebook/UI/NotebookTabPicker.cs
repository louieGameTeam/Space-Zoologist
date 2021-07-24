using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NotebookTabPicker : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Prefab of the button used to select notebook tabs")]
    private NotebookTabSelectButton buttonPrefab;
    [SerializeField]
    [Tooltip("Toggle group used to make only one button selected")]
    private ToggleGroup parent;
    [SerializeField]
    [Tooltip("List of the parents of the game objects for each notebook page.  " +
        "NOTE: these must match up with the same order as the NotebookTab enum")]
    private List<GameObject> pages;

    // Current tab of the picker
    private NotebookTab currentTab;
    // List of the buttons used to select a tab
    private List<NotebookTabSelectButton> buttons = new List<NotebookTabSelectButton>();

    private void Awake()
    {
        // By default, each page is inactive
        foreach(GameObject page in pages)
        {
            page.SetActive(false);
        }

        // Instantiate a selection button for each value in the notebook tab enum
        NotebookTab[] tabs = (NotebookTab[])System.Enum.GetValues(typeof(NotebookTab));
        for(int i = 0; i < tabs.Length; i++)
        {
            NotebookTabSelectButton button = Instantiate(buttonPrefab, parent.transform);
            // Only the first selector will be on. NOTE: this invokes "OnTabSelected" immediately
            button.Setup(tabs[i], parent, OnTabSelected, i == 0);
            // Add this button to the list
            buttons.Add(button);
        }
    }

    private void OnTabSelected(NotebookTab tab)
    {
        int i = (int)tab;
        // Make sure that the page exists
        if(i < pages.Count)
        {
            // Disable the current page and enable the new page
            pages[(int)currentTab].SetActive(false);
            pages[i].SetActive(true);
            currentTab = tab;
        }
    }
    // Select a specific notebook tab by selecting one of the buttons
    public void SelectTab(NotebookTab tab)
    {
        buttons[(int)tab].Select();
    }
}
