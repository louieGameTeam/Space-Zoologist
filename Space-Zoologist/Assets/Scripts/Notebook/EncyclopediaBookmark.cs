using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaBookmark : NotebookBookmark
{
    public override Type ExpectedComponentType => typeof(ResearchEncyclopediaUI);

    [SerializeField]
    [Tooltip("Identification of the article bookmarked in the encyclopedia")]
    private ResearchEncyclopediaArticleID articleID;

    public EncyclopediaBookmark(string prefix, NotebookTab tab, ResearchCategory category, string pickerName, ResearchEncyclopediaArticleID articleID) : base(prefix, tab, category, pickerName)
    {
        this.articleID = articleID;
    }
    public static EncyclopediaBookmark Create(string prefix, NotebookTab tab, ResearchCategoryPicker picker, ResearchEncyclopediaArticleID articleID)
    {
        return new EncyclopediaBookmark(prefix, tab, picker.SelectedCategory, picker.name, articleID);
    }
    protected override void ProcessComponent(Component component)
    {
        ResearchEncyclopediaUI ui = (ResearchEncyclopediaUI)component;
        ui.CurrentArticleID = articleID;
    }
}
