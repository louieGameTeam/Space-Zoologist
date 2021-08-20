using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBookmarkAddButton : NotebookBookmarkAddButton
{
    protected override string SuggestedBookmarkTitle => string.Format(formatLabel, categoryPicker.SelectedCategory.Name);

    [SerializeField]
    [Tooltip("Research tab that the bookmark will navigate to")]
    private NotebookTab tab;
    [SerializeField]
    [Tooltip("String format to use to get the suggested bookmark title. " +
        "Put {0} to substitute the name of the research category")]
    private string formatLabel;

    protected override NotebookBookmark BookmarkToAdd(string inputText)
    {
        return NotebookBookmark.Create(inputText, tab, categoryPicker);
    }
}
