using System.Collections.Generic;

[System.Serializable]
public class JournalData 
{
    public Dictionary<string, JournalEntry> Entries;

    // For loading previous journal data
    public JournalData(JournalData data)
    {
        this.Entries = new Dictionary<string, JournalEntry>();
        foreach(KeyValuePair<string, JournalEntry> entry in data.Entries)
        {
            this.Entries.Add(entry.Key, entry.Value);
        }
    }

    // For testing
    public JournalData(List<JournalEntry> entries)
    {
        this.Entries = new Dictionary<string, JournalEntry>();
        foreach(JournalEntry entry in entries)
        {
            this.Entries.Add(entry.DiscoveredSpecies, entry);
        }
    }

    public JournalData()
    {
        this.Entries = new Dictionary<string, JournalEntry>();
    }
}
