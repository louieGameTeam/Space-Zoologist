using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchEncyclopediaUI : NotebookUIChild
{
    public ResearchEncyclopedia CurrentEncyclopedia => UIParent.NotebookModel.NotebookResearch.GetEntry(currentCategory).Encyclopedia;
    public ResearchEncyclopediaArticle CurrentArticle => CurrentEncyclopedia.GetArticle(currentArticleID);

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

    [SerializeField]
    [Tooltip("Reference to the widget that selects the category for the encyclopedia")]
    private ResearchCategoryPicker categoryPicker;
    [SerializeField]
    [Tooltip("Dropdown used to select available encyclopedia articles")]
    private TMP_Dropdown dropdown;
    [SerializeField]
    [Tooltip("Input field used to display the encyclopedia article")]
    private TMP_InputField articleBody;
    [SerializeField]
    [Tooltip("Button used to add a highlight to the article")]
    private Button highlightButton;

    [Header("Rich Text")]

    [SerializeField]
    [Tooltip("List of tags used to render highlighted encyclopedia article text")]
    private List<RichTextTag> highlightTags;

    // Maps the research category to the index of the article previously selected
    private Dictionary<ResearchCategory, int> previousSelected = new Dictionary<ResearchCategory, int>();
    // Current research category selected
    private ResearchCategory currentCategory;
    // Current research article selected
    private ResearchEncyclopediaArticleID currentArticleID;

    protected override void Awake()
    {
        base.Awake();

        // Add listener for change of dropdown value
        // (is "on value changed" invoked at the start?)
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // If the category picker is already initialized, we need to update our UI
        if(categoryPicker.HasBeenInitialized)
        {
            OnResearchCategoryChanged(categoryPicker.SelectedCategory);
        }

        // Add listener for changes in the research category selected
        categoryPicker.OnResearchCategoryChanged.AddListener(OnResearchCategoryChanged);
        // Add listener for highlight button
        highlightButton.onClick.AddListener(RequestHighlight);
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
        foreach(KeyValuePair<ResearchEncyclopediaArticleID, ResearchEncyclopediaArticle> article in CurrentEncyclopedia.Articles)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(ArticleIDToDropdownLabel(article.Key)));
        }
        // Select the first article in the list
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
            // Create the id object
            currentArticleID = DropdownLabelToArticleID(dropdown.options[value].text);
            // Set the text of the article GUI element
            articleBody.text = RichEncyclopediaArticleText(CurrentArticle, highlightTags);
        }
        else articleBody.text = "<color=#aaa>This encyclopedia has no entries</color>";
    }

    private void RequestHighlight()
    {
        // Use selection position on the input field to determine position of highlights
        int start = articleBody.selectionAnchorPosition;
        int end = articleBody.selectionFocusPosition;

        // If selection has no length, exit the function
        if (start == end) return;

        // If start is bigger than end, swap them
        if(start > end)
        {
            int temp = start;
            start = end;
            end = temp;
        }

        // Request a highlight on the current article
        CurrentArticle.RequestHighlight(start, end);
        // Update the text on the article
        articleBody.text = RichEncyclopediaArticleText(CurrentArticle, highlightTags);
    }

    public static string RichEncyclopediaArticleText(ResearchEncyclopediaArticle article, List<RichTextTag> tags)
    {
        string richText = article.Text;
        int globalIndexAdjuster = 0;    // Adjust the index for each highlight
        int globalIndexIncrementer = 0; // Length of all the tags used in each highlight
        int localIndexAdjuster; // Used to adjust the index as each tag is applied

        // Compute the index incrementer by incrementing tag lengths
        foreach(RichTextTag tag in tags)
        {
            globalIndexIncrementer += tag.Length;
        }
        // Go through all highlights
        foreach(ResearchEncyclopediaArticleHighlight highlight in article.Highlights)
        {
            // Reset local adjuster to 0
            localIndexAdjuster = 0;

            // Apply each of the tags used to highlight
            foreach(RichTextTag tag in tags)
            {
                richText = tag.Apply(richText, highlight.Start + globalIndexAdjuster + localIndexAdjuster, highlight.Length);
                localIndexAdjuster += tag.OpeningTag.Length;
            }

            // Increase the global index adjuster
            globalIndexAdjuster += globalIndexIncrementer;
        }

        return richText;
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
        if (id.Author != "") label += " by " + id.Author;
        return label;
    }
    public static ResearchEncyclopediaArticleID DropdownLabelToArticleID(string label)
    {
        string[] titleAndAuthor = Regex.Split(label, " by ");

        // If there are two items in the split string, use them both
        if(titleAndAuthor.Length > 1)
        {
            return new ResearchEncyclopediaArticleID(titleAndAuthor[0], titleAndAuthor[1]);
        }
        // If there was only one item, we know that there was not author
        else return new ResearchEncyclopediaArticleID(titleAndAuthor[0], "");
    }
}
