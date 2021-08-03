using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaBookmark : NotebookBookmark
{
    public override Type ExpectedComponentType => typeof(ResearchEncyclopediaUI);

    private ResearchEncyclopediaArticleID articleID;

    public EncyclopediaBookmark(string label, ResearchCategory category, string pickerName, ResearchEncyclopediaArticleID articleID) : base(label, NotebookTab.Research, category, pickerName)
    {
        this.articleID = articleID;
    }
    public static EncyclopediaBookmark Create(string prefix, ResearchCategoryPicker picker, ResearchEncyclopediaArticleID articleID)
    {
        return new EncyclopediaBookmark(prefix, picker.SelectedCategory, picker.name, articleID);
    }
    protected override void ProcessComponent(Component component)
    {
        ResearchEncyclopediaUI ui = (ResearchEncyclopediaUI)component;
        ui.CurrentArticleID = articleID;
    }

    public static bool operator ==(EncyclopediaBookmark a, EncyclopediaBookmark b)
    {
        return a.tab == b.tab && a.category == b.category && a.pickerName == b.pickerName && a.articleID == b.articleID;
    }
    public static bool operator !=(EncyclopediaBookmark a, EncyclopediaBookmark b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        else if (obj.GetType() == typeof(EncyclopediaBookmark)) return this == (EncyclopediaBookmark)obj;
        else return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode() + articleID.GetHashCode();
    }

}
