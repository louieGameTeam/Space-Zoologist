using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntryData : NotebookDataModule
{
    #region Public Properties
    public List<ResearchEncyclopediaArticleData> Articles => articles;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of notes that the player has taken in this research entry")]
    private List<string> notes = new List<string>();
    [SerializeField]
    private List<ResearchEncyclopediaArticleData> articles = new List<ResearchEncyclopediaArticleData>();
    #endregion

    #region Private Fields
    private ResearchEntryConfig entryConfig;
    #endregion

    #region Constructors
    public ResearchEntryData(NotebookConfig config, ResearchEntryConfig entryConfig) : base(config)
    {
        this.entryConfig = entryConfig;

        // Add an empty string for each label in the note labels
        foreach(string _ in entryConfig.NoteLabels.Labels)
        {
            notes.Add("");
        }

        // Double check to make sure you have an encyclopedia
        if(entryConfig.Encyclopedia)
        {
            // Add an empty article data for each article config
            foreach (ResearchEncyclopediaArticleConfig articleConfig in entryConfig.Encyclopedia.Articles)
            {
                articles.Add(new ResearchEncyclopediaArticleData(config, articleConfig));
            }
        }
    }
    #endregion

    #region Public Methods
    public string ReadNote(string label)
    {
        int index = entryConfig.NoteLabels.Labels.IndexOf(label);

        if (index >= 0)
        {
            if (index < notes.Count)
            {
                return notes[index];
            }
            else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
                $"no corresponding note found for label '{label}'" +
                $"\n\tLabel index: {index}" +
                $"\n\tNote count: {notes.Count}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
            $"no label '{label}' was found in the list of research entry note labels" +
            $"\n\tLabels: [ {string.Join(", ", entryConfig.NoteLabels.Labels)} ]");
    }
    public void WriteNote(string label, string note)
    {
        int index = entryConfig.NoteLabels.Labels.IndexOf(label);

        if (index >= 0)
        {
            if (index < notes.Count)
            {
                notes[index] = note;
            }
            else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
                $"no corresponding note found for label '{label}'" +
                $"\n\tLabel index: {index}" +
                $"\n\tNote count: {notes.Count}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
            $"no label '{label}' was found in the list of research entry note labels" +
            $"\n\tLabels: [ {string.Join(", ", entryConfig.NoteLabels.Labels)} ]");
    }
    #endregion
}
