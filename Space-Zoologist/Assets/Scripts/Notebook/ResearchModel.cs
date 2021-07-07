using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notebook/ResearchModel")]
public class ResearchModel : ScriptableObject
{
    // Edit each research entry category seperately
    // This makes it easier to edit than simply 
    [SerializeField]
    private List<ResearchEntry> speciesResearch;
    [SerializeField]
    private List<ResearchEntry> foodResearch;
    [SerializeField]
    private List<ResearchEntry> tileResearch;

    private Dictionary<ResearchCategory, ResearchEntry> researchDictionary = new Dictionary<ResearchCategory, ResearchEntry>();

    private void Awake()
    {
        InitResearchEntries(speciesResearch, ResearchCategoryType.Species);
        InitResearchEntries(foodResearch, ResearchCategoryType.Food);
        InitResearchEntries(tileResearch, ResearchCategoryType.Tile);
    }

    private void InitResearchEntries(List<ResearchEntry> entries, ResearchCategoryType type)
    {
        foreach (ResearchEntry entry in entries)
        {
            entry.Awake(type);
            researchDictionary.Add(entry.Category, entry);
        }
    }

    public ResearchEntry GetEntry(ResearchCategory category) => researchDictionary[category];
}
