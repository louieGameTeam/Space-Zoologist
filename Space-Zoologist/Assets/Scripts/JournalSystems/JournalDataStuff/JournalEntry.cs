using System.Collections.Generic;

[System.Serializable]
public class JournalEntry
{
    public string DiscoveredSpecies;
    public string DiscoveredSpeciesEntryText;
    public Dictionary<string, JournalNeedResearch> DiscoveredNeeds;

    public JournalEntry(string speciesName)
    {
        this.DiscoveredSpecies = speciesName;
        this.DiscoveredNeeds = new Dictionary<string, JournalNeedResearch>();
    }
}
