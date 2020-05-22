using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add new species and search for existing ones
/// </summary>
/// TODO setup so species are first added from existing journal entries (and create connections between GameObject and JournalEntrySO)
/// and then added by creating a new journal entry SO
public class SpeciesSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] private GameObject SpeciesDisplayPrefab = default;
    [SerializeField] private GameObject SpeciesContent = default;
    [SerializeField] JournalData JournalEntriesData = default;
    private List<GameObject> JournalEntries = new List<GameObject>();
    [Header("Populate Needs Section")]
    public ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    public void Start()
    {
        foreach (JournalEntry entry in this.JournalEntriesData.Entries)
        {
            this.AddNewEntryFromSpecies(entry.DiscoveredSpecies);
        }
    }

    public void AddNewEntryFromGameObject(GameObject species)
    {
        SpeciesData speciesData = null;
        if (species.TryGetComponent(out speciesData))
        {
            this.AddNewEntryFromSpecies(speciesData.JournalData.DiscoveredSpecies);
        }
        else
        {
            Debug.Log("Attempted to add invalid gameobject to journal");
        }
    }

    public void AddNewEntryFromSpecies(Species species)
    {   
        GameObject newEntry = Instantiate(this.SpeciesDisplayPrefab, this.SpeciesContent.transform);
        newEntry.GetComponent<SpeciesEntryDisplayLogic>().Initialize(species);
        this.SetupItemSelectedHandler(newEntry, this.OnItemSelectedEvent);
        // For searching by GameObject name
        newEntry.name = species.SpeciesName;
        this.JournalEntries.Add(newEntry);
        newEntry.SetActive(true);
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
            foreach(GameObject entry in this.JournalEntries)
            {
                entry.SetActive(true);
            }
        }
        else 
        {
            foreach(GameObject entry in this.JournalEntries)
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
