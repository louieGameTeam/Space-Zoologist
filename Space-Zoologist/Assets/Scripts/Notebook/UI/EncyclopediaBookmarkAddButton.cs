using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaBookmarkAddButton : NotebookBookmarkAddButton
{
    protected override string SuggestedBookmarkTitle => categoryPicker.SelectedCategory.Name + " Encyclopedia: " + ui.CurrentArticleID;

    [SerializeField]
    [Tooltip("Reference to the encyclopedia ui to create a bookmark for")]
    protected ResearchEncyclopediaUI ui;

    protected override NotebookBookmark BookmarkToAdd(string inputText)
    {
        return EncyclopediaBookmark.Create(inputText, categoryPicker, ui.CurrentArticleID);
    }
}
