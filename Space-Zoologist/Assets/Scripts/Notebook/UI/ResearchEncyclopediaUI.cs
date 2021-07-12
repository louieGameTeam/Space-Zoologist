using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class ResearchEncyclopediaUI : MonoBehaviour
{
    [SerializeField]
    [Expandable]
    [Tooltip("Object that holds all the research data")]
    private ResearchModel researchModel;

    [SerializeField]
    [Tooltip("Reference to the widget that selects the category for the encyclopedia")]
    private ResearchCategoryPicker categoryPicker;
    [SerializeField]
    [Tooltip("Dropdown used to select available encyclopedia articles")]
    private TMP_Dropdown dropdown;
    [SerializeField]
    [Tooltip("Input field used to display the encyclopedia article")]
    private TMP_InputField articleBody;

    // Maps the research category to the index of the article previously selected
    private Dictionary<ResearchCategory, int> previousSelected = new Dictionary<ResearchCategory, int>();
    // Current research category selected
    private ResearchCategory currentCategory;

    private void Start()
    {
        // Add listener for change of dropdown value
        // (is "on value changed" invoked at the start?)
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // If the category picker is already initialized, we need to update our UI
        if(categoryPicker.SelectedCategory.Name != null)
        {
            OnResearchCategoryChanged(categoryPicker.SelectedCategory);
        }

        // Add listener for changes in the research category selected
        categoryPicker.OnResearchCategoryChanged.AddListener(OnResearchCategoryChanged);
    }

    private void OnResearchCategoryChanged(ResearchCategory category)
    {
        // If the current category exists, save the value previously selected in the dictionary
        if(previousSelected.ContainsKey(currentCategory)) previousSelected[currentCategory] = dropdown.value;

        // Set currently selected category
        currentCategory = category;
        // Clear the options of the dropdown
        dropdown.ClearOptions();

        // Loop through all articles in the current encyclopedia and add their title-author pairs to the dropdown list
        foreach(KeyValuePair<ResearchEncyclopediaArticleID, ResearchEncyclopediaArticle> article in researchModel.GetEntry(category).Encyclopedia.Articles)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(article.Key.Title + " -> " + article.Key.Author));
        }
        // Select the first article in the list
        // NOTE: this code immediately invokes "OnDropdownValueChanged"
        if (previousSelected.ContainsKey(category)) dropdown.value = previousSelected[category];
        else
        {
            previousSelected[category] = 0;
            dropdown.value = 0;
        }

        // Refresh the shown value since we just changed it
        dropdown.RefreshShownValue();
        OnDropdownValueChanged(dropdown.value);
    }

    private void OnDropdownValueChanged(int value)
    {
        if (dropdown.options.Count > 0)
        {
            // Get the title and author from the dropdown string
            string[] titleAndAuthor = Regex.Split(dropdown.options[dropdown.value].text, " -> ");
            // Create the id object
            ResearchEncyclopediaArticleID id = new ResearchEncyclopediaArticleID(titleAndAuthor[0], titleAndAuthor[1]);
            // Get the article with the given id from the research encyclopedia
            ResearchEncyclopediaArticle article = researchModel.GetEntry(currentCategory).Encyclopedia.GetArticle(id);
            // Set the text of the article GUI element
            articleBody.text = article.Text;
        }
        else articleBody.text = "<color=#aaa>This encyclopedia has no entries</color>";
    }
}
