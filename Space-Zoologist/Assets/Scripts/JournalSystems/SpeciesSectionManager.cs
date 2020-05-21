using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add new species and search for existing ones
/// </summary>
public class SpeciesSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] private GameObject SpeciesDisplayPrefab = default;
    [SerializeField] private GameObject SpeciesContent = default;
    [Expandable] public List<Species> ResearchedSpecies = default;
    [Header("For testing")]
    [SerializeField] List<SpeciesNeed> TestDiscoveredNeeds = default;
    [SerializeField] string TestDescription = default;
    private List<GameObject> JournalEntries = new List<GameObject>();
    [Header("Populate Needs Section")]
    public ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();

    public void Start()
    {
        foreach (Species species in this.ResearchedSpecies)
        {
            this.AddNewEntryFromSpecies(species);
        }
    }

    public void AddNewEntryFromGameObject(GameObject species)
    {
        SpeciesData speciesData = null;
        if (species.TryGetComponent(out speciesData))
        {
            this.AddNewEntryFromSpecies(speciesData.Data);
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
        newEntry.GetComponent<SpeciesData>().Data = species;
        this.TestingStuff(newEntry);
        this.SetupItemSelectedHandler(newEntry, this.OnItemSelectedEvent);
        // For searching by GameObject name
        newEntry.name = species.SpeciesName;
        this.JournalEntries.Add(newEntry);
        newEntry.SetActive(true);
    }

    public void TestingStuff(GameObject newEntry)
    {
        newEntry.GetComponent<SpeciesData>().DiscoveredNeeds = this.TestDiscoveredNeeds;
        newEntry.GetComponent<SpeciesData>().Description = this.TestDescription;
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
