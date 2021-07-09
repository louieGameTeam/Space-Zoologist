using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ResearchCategoryPicker : MonoBehaviour
{
    [SerializeField]
    [Expandable]
    [Tooltip("The research model used to pick the categories for")]
    private ResearchModel researchModel;

    [Header("Research Category Type Selection")]

    [SerializeField]
    [Tooltip("This toggle group will be the parent of all toggles used to select the research category's type")]
    private ToggleGroup typeGroup;
    [SerializeField]
    [Tooltip("Prefab of the button used to select the research category type")]
    private ResearchCategoryTypeButton typeButton;

    [Header("Research Category Name Selection")]

    [SerializeField]
    [Tooltip("This toggle group will be the parent of all toggles used to select the research category's name")]
    private ToggleGroup nameGroup;
    [SerializeField]
    [Tooltip("Prefab of the button used to select the research name")]
    private ResearchCategoryNameButton nameButton;

    // The category currently selected
    private ResearchCategory selectedCategory;
    // List of name button groups
    // NOTE: access the buttons for category type using buttonGroups[(int)ResearchCategoryType.<enum_value>]
    private List<ResearchCategoryNameButtonGroup> nameButtonGroups = new List<ResearchCategoryNameButtonGroup>();

    private void Start()
    {
        // Go through all research categories in the research model, adding buttons to each group in the list
        foreach (KeyValuePair<ResearchCategory, ResearchEntry> entry in researchModel.ResearchDictionary)
        {
            // Add groups until the count exceeds the current index we need to insert into
            while(nameButtonGroups.Count <= (int)entry.Key.Type)
            {
                nameButtonGroups.Add(new ResearchCategoryNameButtonGroup());
            }            

            // Create a clone and set it up
            ResearchCategoryNameButton clone = Instantiate(nameButton, nameGroup.transform);
            clone.Setup(nameGroup, entry.Key.Name, OnResearchCategoryNameChanged, false);
            // Add it to the current category name button group
            nameButtonGroups[(int)entry.Key.Type].AddButton(clone);
        }

        // Disable all button groups initially
        foreach (ResearchCategoryNameButtonGroup group in nameButtonGroups)
        {
            group.SetActive(false);
        }

        // Get all research category types
        ResearchCategoryType[] types = (ResearchCategoryType[])System.Enum.GetValues(typeof(ResearchCategoryType));
        // Loop through all types.  Instantiate a button for each type
        for(int i = 0; i < types.Length; i++)
        {
            ResearchCategoryTypeButton clone = Instantiate(typeButton, typeGroup.transform);
            // Setup clone. The first one is selected.
            // NOTE: this invokes OnResearchCategoryTypeChanged immediately
            clone.Setup(typeGroup, types[i], OnResearchCategoryTypeChanged, i == 0);
        }
    }

    private void OnResearchCategoryTypeChanged(ResearchCategoryType type)
    {
        // If a previous category has been selected, then disable it
        if(selectedCategory != null)
        {
            nameButtonGroups[(int)selectedCategory.Type].SetActive(false);   
        }

        // Create the currently selected category
        selectedCategory = new ResearchCategory(type, "");

        // Enable the new name button group
        // NOTE: this invokes OnResearchCategoryNameChaned immediately
        nameButtonGroups[(int)type].SetActive(true);

        // Destroy all existing name buttons and remove them from the list
        //while(currentNameButtons.Count > 0)
        //{
        //    Destroy(currentNameButtons[0].gameObject);
        //    currentNameButtons.RemoveAt(0);
        //}

        //// Get the list of the names to instantiate
        //List<string> names = typeNameMapping[type];
        //bool isSelected = true;

        //// Instantiate a button for each category name
        //foreach(string n in names)
        //{
        //    ResearchCategoryNameButton clone = Instantiate(nameButton, nameGroup.transform);
        //    // Make just the first clone set to "on" initially
        //    // NOTE: this invokes the "OnResearchCategoryNameChanged" event immediately
        //    clone.Setup(nameGroup, n, OnResearchCategoryNameChanged, isSelected);
        //    isSelected = false;
        //    // Add the clone to the list
        //    currentNameButtons.Add(clone);
        //}
    }

    private void OnResearchCategoryNameChanged(string name)
    {
        selectedCategory = new ResearchCategory(selectedCategory.Type, name);

        // Invoke the event
        Debug.Log("Selected research category: " + selectedCategory.Type + ", " + name);
    }
}
