using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NotebookBookmark
{
    // Public accessors
    public string Label => prefix + " -> " + category.Name;
    public NotebookTab Tab => tab;
    public ResearchCategory Category => category;
    public string PickerName => pickerName;

    [SerializeField]
    [Tooltip("Label used to identify the bookmark")]
    private string prefix;
    [SerializeField]
    [Tooltip("Tab in the notebook for this bookmark")]
    private NotebookTab tab;
    [SerializeField]
    [Tooltip("Research category for the bookmark's picker")]
    private ResearchCategory category;
    [SerializeField]
    [Tooltip("Reference to the object that picks the category for this page")]
    private string pickerName;

    public NotebookBookmark(string prefix, NotebookTab tab, ResearchCategory category, string pickerName)
    {
        this.prefix = prefix;
        this.tab = tab;
        this.category = category;
        this.pickerName = pickerName;
    }
    public static NotebookBookmark BuildFromState(string prefix, NotebookTab tab, ResearchCategoryPicker picker)
    {
        return new NotebookBookmark(prefix, tab, picker.SelectedCategory, picker.name);
    }
    public static bool operator==(NotebookBookmark a, NotebookBookmark b)
    {
        return a.tab == b.tab && a.category == b.category && a.pickerName == b.pickerName;
    }
    public static bool operator!=(NotebookBookmark a, NotebookBookmark b)
    {
        return !(a == b);
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        else if (obj.GetType() == typeof(NotebookBookmark)) return this == (NotebookBookmark)obj;
        else return false;
    }
    public override int GetHashCode()
    {
        return tab.GetHashCode() + category.GetHashCode() + pickerName.GetHashCode();
    }
}
