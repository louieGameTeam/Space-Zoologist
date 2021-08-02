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
    public string Acronym => acronym;
    public Research NotebookResearch => notebookResearch;
    public List<NotebookBookmark> Bookmarks { get; private set; } = new List<NotebookBookmark>();
    public Dictionary<char, string> AcronymNotes
    {
        get
        {
            if (acronymNotes.Count <= 0) Setup();
            return acronymNotes;
        }
    }

    //[SerializeField]
    [Tooltip("General notes that the player has made about the game")]
    private string generalNotes;

    [SerializeField]
    [Tooltip("Acronym that the player gets to spell out on the home page")]
    private string acronym = "ROCTM";
    [SerializeField]
    [Expandable]
    [Tooltip("Reference to the model holding all the player's research and info" +
        "about the different species, foods, and tiles")]
    private Research notebookResearch;

    private Dictionary<char, string> acronymNotes = new Dictionary<char, string>();

    public void Setup()
    {
        // Foreach letter in the acronym, add an empty string to the dictionary
        foreach(char c in acronym)
        {
            acronymNotes.Add(c, "");
        }
    }

    public string ReadAcronymNote(char c) => AcronymNotes[c];
    public void WriteAcronymNote(char c, string note) => AcronymNotes[c] = note;

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
