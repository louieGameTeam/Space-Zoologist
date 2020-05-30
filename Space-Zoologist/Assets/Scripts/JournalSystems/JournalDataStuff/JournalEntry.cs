using System.Collections.Generic;

[System.Serializable]
public class JournalEntry
{
    public string DiscoveredSpecies;
    public string DiscoveredSpeciesEntryText;
    public List<string> DiscoveredNeeds;
    public Dictionary<string, string> DiscoveredNeedsEntryText;
    public Dictionary<string, bool> ResearchedNeeds;

    public JournalEntry(string speciesName)
    {
        this.DiscoveredSpecies = speciesName;
        this.DiscoveredNeedsEntryText = new Dictionary<string, string>();
        this.DiscoveredNeeds = new List<string>();
        this.ResearchedNeeds = new Dictionary<string, bool>();
    }

    public void RemoveNeed(string need)
    {
        this.DiscoveredNeedsEntryText.Remove(need);
        this.DiscoveredNeeds.Remove(need);
    }
}
