using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotebookBookmark
{
    // Public accessors
    public string Label => label;
    public NotebookTab Tab => tab;
    public ResearchCategory Category => category;
    public string PickerName => pickerName;
    public virtual Type ExpectedComponentType => null;

    protected string label;
    protected NotebookTab tab;
    protected ResearchCategory category;
    // NOTE: we can't just store the research category picker here in the object
    // This bookmark is stored in a serialized object, so it survives after the picker is destroyed
    // Instead, we ID the picker by the game object's name, and let the NotebookUI provide the
    // picker to the bookmark
    protected string pickerName;

    public NotebookBookmark(string label, NotebookTab tab, ResearchCategory category, string pickerName)
    {
        this.label = label;
        this.tab = tab;
        this.category = category;
        this.pickerName = pickerName;
    }
    public static NotebookBookmark Create(string label, NotebookTab tab, ResearchCategoryPicker picker)
    {
        return new NotebookBookmark(label, tab, picker.SelectedCategory, picker.name);
    }
    public void NavigateTo(NotebookTabPicker tabPicker, Dictionary<string, ResearchCategoryPicker> namePickerMap, Component component)
    {
        // First, pick the tab in the tab picker
        tabPicker.SelectTab(tab);
        // Then set the selected category on the category picker
        namePickerMap[PickerName].SelectedCategory = category;

        // Check if expected component is null
        // Null component type implies this bookmark does not need additional components for navigation
        if(ExpectedComponentType != null)
        {
            if (component != null)
            {
                // If the additional info has the correct type, then use it
                if (component.GetType() == ExpectedComponentType)
                {
                    ProcessComponent(component);
                }
                // Log warning if it has the wrong type
                else LogBookmarkComponentWarning(component);
            }
            else
            {
                LogBookmarkComponentWarning(component);
            }
        }
    }
    protected virtual void ProcessComponent(Component component)
    {
        // PASS - only used by sub-classes
        // But we also can't make this class abstract because it is possible to have a base bookmark
    }
    private void LogBookmarkComponentWarning(Component component)
    {
        Debug.LogWarning("Attempting to navigate to bookmark of type " + GetType() + " with component = " + component +
            "\n\tThe bookmark needs an component of type " + ExpectedComponentType.Name + " to properly navigate to the desired page");
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
