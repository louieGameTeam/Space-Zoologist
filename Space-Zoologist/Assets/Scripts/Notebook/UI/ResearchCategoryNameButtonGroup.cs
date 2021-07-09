using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchCategoryNameButtonGroup
{
    private List<ResearchCategoryNameButton> buttons = new List<ResearchCategoryNameButton>();

    // Index of the button previously selected before disabling
    private int previousSelection = 0;

    public void AddButton(ResearchCategoryNameButton button)
    {
        buttons.Add(button);
    }

    public void SetActive(bool active)
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(active);
            // If this button is on, set previous selection
            if (buttons[i].MyToggle.isOn) previousSelection = i;
        }
        // The button that was previously selected will be on when active, and off when deactivating
        buttons[previousSelection].MyToggle.isOn = active;
    }
}
