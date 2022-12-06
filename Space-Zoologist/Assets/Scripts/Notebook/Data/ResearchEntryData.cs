using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntryData : NotebookDataModule
{
    public List<ResearchEncyclopediaArticleData> Articles => articles;

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
            else
                return string.Empty;
            /*else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
                $"no corresponding note found for label '{label}'" +
                $"\n\tLabel index: {index}" +
                $"\n\tNote count: {notes.Count}");*/
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
            while (notes.Count <= index)
                notes.Add(string.Empty);
            notes[index] = note;
            /*else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
                $"no corresponding note found for label '{label}'" +
                $"\n\tLabel index: {index}" +
                $"\n\tNote count: {notes.Count}");*/
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
            $"no label '{label}' was found in the list of research entry note labels" +
            $"\n\tLabels: [ {string.Join(", ", entryConfig.NoteLabels.Labels)} ]");
    }
    public ResearchEncyclopediaArticleData GetArticleData(ResearchEncyclopediaArticleID id)
    {
        ResearchEncyclopediaArticleConfig articleConfig = entryConfig.Encyclopedia.Articles.Find(article => article.ID == id);

        if (articleConfig != null)
        {
            int index = entryConfig.Encyclopedia.Articles.IndexOf(articleConfig);
            return GetArticleData(index);
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
            $"no article found with id '{id}'");
    }
    public ResearchEncyclopediaArticleData GetArticleData(int index)
    {
        if (index >= 0 && index < articles.Count)
        {
            return articles[index];
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchEntryData)}: " +
            $"no article data exists for index '{index}'");
    }
    #endregion
}
