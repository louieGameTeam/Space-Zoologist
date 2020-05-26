using System.Collections.Generic;

// ScriptableObject for testing
[System.Serializable]
public class JournalEntry
{
    public string DiscoveredSpecies;
    public string DiscoveredSpeciesEntryText;
    public List<string> DiscoveredNeeds;
    public Dictionary<string, string> DiscoveredNeedsEntryText;

    public JournalEntry(string speciesName)
    {
        this.DiscoveredSpecies = speciesName;
        this.DiscoveredNeedsEntryText = new Dictionary<string, string>();
        this.DiscoveredNeeds = new List<string>();
    }
}
