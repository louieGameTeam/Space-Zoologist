using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchEncyclopediaUI : NotebookUIChild
{
    #region Public Properties
    public ResearchEncyclopedia CurrentEncyclopedia => UIParent.Notebook.Research.GetEntry(currentItem).Encyclopedia;
    public ResearchEncyclopediaArticle CurrentArticle => CurrentEncyclopedia != null ? CurrentEncyclopedia.GetArticle(currentArticleID) : null;
    public ResearchEncyclopediaArticleID CurrentArticleID
    {
        get => currentArticleID;
        set
        {
            List<ResearchEncyclopediaArticleID> ids = GetDropdownIDs();
            int index = ids.FindIndex(x => x == value);

            if(index >= 0 && index < dropdown.options.Count)
            {
                // NOTE: this invokes "OnDropdownValueChanged" immediately
                dropdown.value = index;
                dropdown.RefreshShownValue();
            }
            else
            {
                Debug.LogWarning("Encyclopedia article " + value.ToString() + " was not found in the dropdown, so the new value will be ignored");
            }
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the widget that selects the category for the encyclopedia")]
    private ItemPicker itemPicker;
    [SerializeField]
    [Tooltip("Dropdown used to select available encyclopedia articles")]
    private TMP_Dropdown dropdown;
    [SerializeField]
    [Tooltip("Input field used to display the encyclopedia article")]
    private ResearchEncyclopediaArticleInputField articleBody;
    [SerializeField]
    [Tooltip("Script that is targetted by the bookmarking system")]
    private BookmarkTarget bookmarkTarget;
    #endregion

    #region Private Fields
    // Maps the research category to the index of the article previously selected
    private Dictionary<ItemID, int> previousSelected = new Dictionary<ItemID, int>();
    // Current research category selected
    private ItemID currentItem = new ItemID(ItemRegistry.Category.Species, -1);
    // Current research article selected
    private ResearchEncyclopediaArticleID currentArticleID;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        base.Setup();

        // Add listener for change of dropdown value
        // (is "on value changed" invoked at the start?)
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // If the category picker is already initialized, we need to update our UI
        if(itemPicker.HasBeenInitialized)
        {
            OnItemIDChanged(itemPicker.SelectedItem);
        }

        // Add listener for changes in the research category selected
        itemPicker.OnItemPicked.AddListener(OnItemIDChanged);

        // Setup the bookmark target to get/set the article id
        bookmarkTarget.Setup(() => CurrentArticleID, x => CurrentArticleID = (ResearchEncyclopediaArticleID)x);
    }
    // Get a list of the research article IDs currently in the dropdown
    public List<ResearchEncyclopediaArticleID> GetDropdownIDs()
    {
        return dropdown.options
            .Select(o => DropdownLabelToArticleID(o.text))
            .ToList();
    }
    public static string ArticleIDToDropdownLabel(ResearchEncyclopediaArticleID id)
    {
        string label = id.Title;
        // Only include the author if it has an author
        if (id.Author != "") label += "\n" + id.Author;
        return label;
    }
    public static ResearchEncyclopediaArticleID DropdownLabelToArticleID(string label)
    {
        string[] titleAndAuthor = Regex.Split(label, "\n");

        // If there are two items in the split string, use them both
        if (titleAndAuthor.Length > 1)
        {
            return new ResearchEncyclopediaArticleID(titleAndAuthor[0], titleAndAuthor[1]);
        }
        // If there was only one item, we know that there was not author
        else return new ResearchEncyclopediaArticleID(titleAndAuthor[0], "");
    }
    #endregion

    #region Private Methods
    private void OnItemIDChanged(ItemID id)
    {
        // If a current item is selected that save the dropdown value that was previously selected
        if (currentItem.Index >= 0) previousSelected[currentItem] = dropdown.value;

        // Set currently selected category
        currentItem = id;
        // Clear the options of the dropdown
        dropdown.ClearOptions();

        // Check if encyclopedia is null before trying to use it
        if(CurrentEncyclopedia != null)
        {
            // Loop through all articles in the current encyclopedia and add their title-author pairs to the dropdown list
            foreach (KeyValuePair<ResearchEncyclopediaArticleID, ResearchEncyclopediaArticle> article in CurrentEncyclopedia.Articles)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(ArticleIDToDropdownLabel(article.Key)));
            }
            // If this item has been selected before, open to the article that was selected previously
            if (previousSelected.ContainsKey(id)) dropdown.value = previousSelected[id];
            // If this item has not been selected before, then select the first article
            else
            {
                previousSelected[id] = 0;
                dropdown.value = 0;
            }
        }
        // If the current encyclopedia is null then there are no articles to pick
        else
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData("<No articles>"));
            dropdown.value = 0;
        }

        // Refresh the shown value since we just changed it
        dropdown.RefreshShownValue();
        OnDropdownValueChanged(dropdown.value);
    }

    private void OnDropdownValueChanged(int value)
    {
        if (CurrentEncyclopedia != null)
        {
            // Create the id object
            currentArticleID = DropdownLabelToArticleID(dropdown.options[value].text);
            // Update the article on the script
            articleBody.UpdateArticle(CurrentArticle);
        }
        else articleBody.UpdateArticle(null);
    }
    #endregion
}
