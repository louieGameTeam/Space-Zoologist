using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notebook/Notebook")]
public class NotebookModel : ScriptableObject
{
    #region Public Properties
    public string Acronym => acronym;
    public ResearchModel Research => research;
    public ObservationsModel Observations => observations;
    public ConceptsModel Concepts => concepts;
    public TestAndMetricsModel TestAndMetrics => testAndMetrics;
    public NotebookTabScaffold TabScaffold => tabScaffold;
    public List<Bookmark> Bookmarks { get; private set; } = new List<Bookmark>();
    // This should be the same for concepts and testAndMetrics also
    public List<EnclosureID> EnclosureIDs => observations.EnclosureIDs;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Acronym that the player gets to spell out on the home page")]
    private string acronym = "ROCTM";
    
    [Space]
    
    [SerializeField]
    [WrappedProperty("researchEntryData")]
    [Tooltip("Reference to the model holding all the player's research and info" +
        "about the different species, foods, and tiles")]
    private ResearchModel research;
    [SerializeField]
    [Tooltip("Player observation notes")]
    private ObservationsModel observations;
    [SerializeField]
    [Tooltip("Player's proposal of concepts and requests for resources")]
    private ConceptsModel concepts;
    [SerializeField]
    [Tooltip("Test and metrics that the player has taken")]
    private TestAndMetricsModel testAndMetrics;

    [Space]

    [SerializeField]
    [Tooltip("Controls which tabs are available in what levels")]
    private NotebookTabScaffold tabScaffold;
    [SerializeField]
    [Tooltip("List of items that should be unlocked at the beginning of the game")]
    private List<ItemID> initiallyUnlockedItems;
    #endregion

    #region Private Fields
    // Notes on each character in the acronym
    private Dictionary<char, string> acronymNotes = new Dictionary<char, string>();
    // A set with all items that have been unlocked
    private HashSet<ItemID> itemsUnlocked = new HashSet<ItemID>();
    #endregion

    #region Public Methods
    public void Setup()
    {
        // Foreach letter in the acronym, add an empty string to the dictionary
        foreach(char c in acronym)
        {
            if (!acronymNotes.ContainsKey(c)) acronymNotes.Add(c, "");
        }
        // Foreach letter in the initially unlocked list, add it to the unlocked items
        foreach(ItemID item in initiallyUnlockedItems)
        {
            if (!itemsUnlocked.Contains(item)) itemsUnlocked.Add(item);
        }
        research.Setup();
    }

    public string ReadAcronymNote(char c) => acronymNotes[c];
    public void WriteAcronymNote(char c, string note) => acronymNotes[c] = note;

    // Return true/false if the notebook already has this bookmark
    public bool HasBookmark(Bookmark bookmark) => Bookmarks.Contains(bookmark);
    // Add the bookmark if the notebook doesn't already have it in the list
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
    public void TryAddEnclosureID(EnclosureID id)
    {
        observations.TryAddEnclosureID(id);
        concepts.TryAddEnclosureId(id);
        testAndMetrics.TryAddEnclosureID(id);
    }

    public void UnlockItem(ItemID id)
    {
        if (!itemsUnlocked.Contains(id)) itemsUnlocked.Add(id);
    }
    public bool ItemIsUnlocked(ItemID id) => itemsUnlocked.Contains(id);
    #endregion
}
