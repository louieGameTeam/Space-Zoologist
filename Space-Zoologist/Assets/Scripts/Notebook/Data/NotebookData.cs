using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotebookData : NotebookDataModule
{
    #region Public Properties
    public List<string> AcronymNotes => acronymNotes;
    public List<ItemID> ItemsUnlocked => itemsUnlocked;
    public ResearchData Research => research;
    public ObservationsData Observations => observations;
    public ConceptsData Concepts => concepts;
    public TestAndMetricsData TestAndMetrics => testAndMetrics;
    public List<Bookmark> Bookmarks => bookmarks;
    public List<LevelID> Levels => observations.LevelsIDs;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Notes that the player has taken on each character in the acronym")]
    private List<string> acronymNotes = new List<string>();
    [SerializeField]
    [Tooltip("List of items that the player has unlocked")]
    private List<ItemID> itemsUnlocked = new List<ItemID>();

    [SerializeField]
    [Tooltip("List of research data that the player has added to")]
    private ResearchData research;
    [SerializeField]
    [Tooltip("Observations that the player has made")]
    private ObservationsData observations;
    [SerializeField]
    [Tooltip("Concepts data containing reviewed resource requests")]
    private ConceptsData concepts;
    [SerializeField]
    [Tooltip("Test and metric data that the player has measured")]
    private TestAndMetricsData testAndMetrics;

    [SerializeField]
    [Tooltip("List of bookmarks in the notebook")]
    private List<Bookmark> bookmarks = new List<Bookmark>();
    #endregion

    #region Constructors
    public NotebookData(NotebookConfig config) : base(config)
    {
        // Add an empty string for every character
        foreach (char _ in config.Acronym)
        {
            acronymNotes.Add("");
        }
        research = new ResearchData(config);
        observations = new ObservationsData(config);
        concepts = new ConceptsData(config);
        testAndMetrics = new TestAndMetricsData(config);
    }
    #endregion

    #region Public Methods
    public override void SetConfig(NotebookConfig config)
    {
        base.SetConfig(config);

        // If the acronym is not the same length as the notes, rebuild the notes
        if(config.Acronym.Length != acronymNotes.Count)
        {
            acronymNotes.Clear();

            // Add an empty string for every character
            foreach (char _ in config.Acronym)
            {
                acronymNotes.Add("");
            }
        }

        // Set config for each sub-module
        research.SetConfig(config);
        observations.SetConfig(config);
        concepts.SetConfig(config);
        testAndMetrics.SetConfig(config);
    }
    public string ReadAcronymNote(char c)
    {
        int index = Config.Acronym.IndexOf(c);

        // Check to make sure the character was found
        if (index >= 0)
        {
            // Make sure it is not out of bounds of the array
            if (index < acronymNotes.Count)
            {
                return acronymNotes[index];
            }
            else throw new System.IndexOutOfRangeException($"{nameof(NotebookData)}: " +
                $"character '{c}' is out of range of the current list of acronym notes." +
                $"\n\tAcronym: {Config.Acronym}" +
                $"\n\tNotes count: {acronymNotes.Count}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(NotebookData)}: " +
            $"no character '{c}' found in the notebook acronym '{Config.Acronym}'");
    }
    public void WriteAcronymNote(char c, string note)
    {
        int index = Config.Acronym.IndexOf(c);

        // Check to make sure the character was found
        if (index >= 0)
        {
            // Make sure it is not out of bounds of the array
            if (index < acronymNotes.Count)
            {
                acronymNotes[index] = note;
            }
            else throw new System.IndexOutOfRangeException($"{nameof(NotebookData)}: " +
                $"character '{c}' is out of range of the current list of acronym notes." +
                $"\n\tAcronym: {Config.Acronym}" +
                $"\n\tNotes count: {acronymNotes.Count}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(NotebookData)}: " +
            $"no character '{c}' found in the notebook acronym '{Config.Acronym}'");
    }
    // Return true/false if the notebook already has this bookmark
    public bool HasBookmark(Bookmark bookmark) => Bookmarks.Contains(bookmark);
    public bool TryAddBookmark(Bookmark bookmark)
    {
        if (!Bookmarks.Contains(bookmark))
        {
            AddBookmark(bookmark);
            return true;
        }
        else return false;
    }
    private void AddBookmark(Bookmark bookmark)
    {
        Bookmarks.Add(bookmark);
    }
    public void RemoveBookmark(Bookmark bookmark)
    {
        Bookmarks.Remove(bookmark);
    }

    // Level IDs
    public void TryAddLevelID(LevelID id)
    {
        observations.TryAddEnclosureID(id);
        concepts.TryAddEnclosureId(id);
        testAndMetrics.TryAddEnclosureID(id);
    }

    // Unlocked items
    public void UnlockItem(ItemID id)
    {
        if (!itemsUnlocked.Contains(id)) itemsUnlocked.Add(id);
    }
    public bool ItemIsUnlocked(ItemID id) => itemsUnlocked.Contains(id);
    #endregion

    #region Private Methods
    private int AcronymCharacterIndex(char c) => Config.Acronym.IndexOf(c);
    #endregion
}
