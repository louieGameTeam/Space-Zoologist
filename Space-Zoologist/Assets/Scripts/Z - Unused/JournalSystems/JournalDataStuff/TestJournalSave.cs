using UnityEngine;
using System.Collections.Generic;
using System;

public class TestJournalSave : MonoBehaviour
{

    public void CreateAndSaveJournal()
    {
        JournalEntry entrya = new JournalEntry("Madle");
        JournalEntry entryb = new JournalEntry("Prog");

        JournalNeedResearch a = new JournalNeedResearch("SpaceMaple");
        entrya.DiscoveredNeeds.Add("SpaceMaple", a);
        a = new JournalNeedResearch("GasX");
        entrya.DiscoveredNeeds.Add("GasX", a);

        a = new JournalNeedResearch("RedLiquid");
        entryb.DiscoveredNeeds.Add("RedLiquid", a);
        a = new JournalNeedResearch("Sand");
        entryb.DiscoveredNeeds.Add("Sand", a);

        entrya.DiscoveredSpeciesEntryText = "likes to have lots of water and SpaceMaples";
        entrya.DiscoveredNeeds["SpaceMaple"].NeedDescription = "needs x amount";
        entrya.DiscoveredNeeds["GasX"].NeedDescription = "needs y amount";

        entryb.DiscoveredSpeciesEntryText = "likes to have lots of water and SpaceMaples";
        entryb.DiscoveredNeeds["RedLiquid"].NeedDescription = "needs x amount";
        entryb.DiscoveredNeeds["Sand"].NeedDescription = "needs y temperature";

        List<JournalEntry> entries = new List<JournalEntry>();
        entries.Add(entrya);
        entries.Add(entryb);
        JournalData journal = new JournalData(entries);
        SaveSystem.SaveJournal(journal);
    }

    public void LoadAndTestJournal()
    {
        JournalData journal = SaveSystem.LoadJournal();
        Debug.Log("Madle");
        foreach (var item in journal.Entries["Madle"].DiscoveredNeeds)
        {
            Debug.Log(item.Value.NeedName);
            Debug.Log(item.Value.NeedDescription);
        }
        Debug.Log("Prog");
        foreach (var item in journal.Entries["Prog"].DiscoveredNeeds)
        {
            Debug.Log(item.Value.NeedName);
            Debug.Log(item.Value.NeedDescription);
        }

    }
}
