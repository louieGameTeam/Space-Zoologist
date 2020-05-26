using UnityEngine;
using System.Collections.Generic;

public class TestJournalSave : MonoBehaviour
{
    public void CreateAndSaveJournal()
    {
        JournalEntry entrya = new JournalEntry("strot");
        JournalEntry entryb = new JournalEntry("prog");
        List<string> needs = new List<string>();
        needs.Add("gasx");
        needs.Add("apple");

        entrya.DiscoveredNeeds = needs;
        entrya.DiscoveredSpeciesEntryText = "likes to have lots of water and apples";
        entrya.DiscoveredNeedsEntryText.Add("gasx", "needs x amount");
        entrya.DiscoveredNeedsEntryText.Add("apple", "needs y amount");

        entryb.DiscoveredNeeds = needs;
        entryb.DiscoveredSpeciesEntryText = "likes to have lots of water and apples";
        entryb.DiscoveredNeedsEntryText.Add("gasx", "needs x amount");
        entryb.DiscoveredNeedsEntryText.Add("apple", "needs y amount");

        List<JournalEntry> entries = new List<JournalEntry>();
        entries.Add(entrya);
        entries.Add(entryb);
        JournalData journal = new JournalData(entries);
        SaveSystem.SaveJournal(journal);
    }

    public void LoadAndTestJournal()
    {
        JournalData journal = SaveSystem.LoadJournal();
        Debug.Log(journal.Entries["strot"].DiscoveredNeedsEntryText["gasx"]);
        Debug.Log(journal.Entries["strot"].DiscoveredNeedsEntryText);
        Debug.Log(journal.Entries["prog"].DiscoveredSpeciesEntryText);
        Debug.Log(journal.Entries["prog"].DiscoveredNeedsEntryText["apple"]);

    }
}
