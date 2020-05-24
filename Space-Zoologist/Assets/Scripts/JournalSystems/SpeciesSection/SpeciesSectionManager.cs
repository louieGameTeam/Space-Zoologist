using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add new species and search for existing ones
/// </summary>
/// 2 TODO Add methods to journal data so entries are indexible
/// 3 TODO setup so species are first added from existing journal entries (and create connections between GameObject and JournalEntry) and then added with a new journal entry added
/// 4 TODO refactor so everything works with new JournalData setup
public class SpeciesSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] private GameObject SpeciesDisplayPrefab = default;
    [SerializeField] private GameObject SpeciesContent = default;
    [SerializeField] private SpeciesReferenceData SpeciesData = default;
    private JournalData JournalEntries = default;
    private List<GameObject> JournalEntriesDisplay = new List<GameObject>();
    [Header("Populate Needs Section")]
    public ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    // Load Journal data and populate Journal with entries
    public void Start()
    {
        this.JournalEntries = SaveSystem.LoadJournal();
        // foreach (JournalEntry entry in this.JournalEntries.Entries)
        // {
        //     Species species = SpeciesData.FindSpecies(entry.DiscoveredSpecies);
        //     this.AddNewEntryFromSpecies(species, true);
        // }
    }

    public void AddNewEntryFromGameObject(GameObject species)
    {
        SpeciesJournalData speciesData = null;
        if (species.TryGetComponent(out speciesData))
        {
            Species s = SpeciesData.FindSpecies(speciesData.JournalEntry.DiscoveredSpecies);
            this.AddNewEntryFromSpecies(s, false);
        }
        else
        {
            Debug.Log("Attempted to add invalid gameobject to journal");
        }
    }

    public void AddNewEntryFromSpecies(Species species, bool alreadyExists)
    {   
        GameObject newEntry = Instantiate(this.SpeciesDisplayPrefab, this.SpeciesContent.transform);
        newEntry.GetComponent<SpeciesEntryDisplayLogic>().Initialize(species);
        this.SetupItemSelectedHandler(newEntry, this.OnItemSelectedEvent);
        // For searching by GameObject name
        newEntry.name = species.SpeciesName;
        newEntry.SetActive(true);
        // if (!alreadyExists)
        // {
        //     JournalEntry journalEntry = new JournalEntry(species.SpeciesName);
        //     this.JournalEntries.Entries.Add(journalEntry);
        // }
        this.JournalEntriesDisplay.Add(newEntry);
    }

    public void SetupItemSelectedHandler(GameObject entry, ItemSelectedEvent itemSelected)
    {
        entry.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(itemSelected);
    }

    // Searching by actual GameObject name
    public void SearchJournal(string speciesSearch)
    {
        if (speciesSearch.Equals(""))
        {
            foreach(GameObject entry in this.JournalEntriesDisplay)
            {
                entry.SetActive(true);
            }
        }
        else 
        {
            foreach(GameObject entry in this.JournalEntriesDisplay)
            {
                if (speciesSearch.Length > entry.name.Length)
                {
                    continue;
                }
                if (!(entry.name.ToLower().Substring(0, speciesSearch.Length).Equals(speciesSearch.ToLower().Substring(0, speciesSearch.Length))))
                {
                    entry.SetActive(false);
                }
                else
                {
                    entry.SetActive(true);
                }
            }
        }
    }


}
