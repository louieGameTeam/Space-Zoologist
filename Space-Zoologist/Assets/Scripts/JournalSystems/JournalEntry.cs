using System.Collections.Generic;

// ScriptableObject for testing
[System.Serializable]
public class JournalEntry
{
    public string DiscoveredSpecies = default;
    public string DiscoveredSpeciesEntryText = default;
    public List<string> DiscoveredNeeds = default;
    public Dictionary<string, string> DiscoveredNeedsEntryText = new Dictionary<string, string>();

    public JournalEntry(string speciesName)
    {
        this.DiscoveredSpecies = speciesName;
    }
}
