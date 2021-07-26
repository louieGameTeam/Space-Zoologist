using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notebook/Notebook")]
public class Notebook : ScriptableObject
{
    // General notes for this notebook
    public string GeneralNotes
    {
        get => generalNotes;
        set => generalNotes = value;
    }
    public List<NotebookBookmark> Bookmarks => bookmarks;

    [SerializeField]
    [Tooltip("General notes that the player has made about the game")]
    private string generalNotes;
    [SerializeField]
    [Tooltip("Reference to the model holding all the player's research and info" +
        "about the different species, foods, and tiles")]
    private Research research;

    [SerializeField]
    [Tooltip("List of bookmarks currently in the notebook")]
    private List<NotebookBookmark> bookmarks = new List<NotebookBookmark>();

    // Add the bookmark if the notebook doesn't already have it in the list
    public bool TryAddBookmark(NotebookBookmark bookmark)
    {
        if (!Bookmarks.Contains(bookmark))
        {
            AddBookmark(bookmark);
            return true;
        }
        else return false;
    }
    private void AddBookmark(NotebookBookmark bookmark)
    {
        Bookmarks.Add(bookmark);
    }
}
