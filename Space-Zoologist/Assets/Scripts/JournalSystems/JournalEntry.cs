using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JournalEntry", menuName = "Journal/JournalEntry")]
public class JournalEntry : ScriptableObject
{
    [SerializeField] public Species DiscoveredSpecies = default;
    [SerializeField] public string DiscoveredSpeciesEntryText = default;
    [SerializeField] public List<SpeciesNeed> DiscoveredNeeds = default;
    public Dictionary<SpeciesNeed, string> DiscoveredNeedsEntryText = new Dictionary<SpeciesNeed, string>();
    [Header("For testing")]
    [SerializeField] public List<string> TestNeedDescriptions = default;

    public void OnValidate()
    {
        int i = 0;
        foreach(SpeciesNeed need in this.DiscoveredNeeds)
        {
            this.DiscoveredNeedsEntryText.Add(need, this.TestNeedDescriptions[i]);
            i++;
        }
    }
}
