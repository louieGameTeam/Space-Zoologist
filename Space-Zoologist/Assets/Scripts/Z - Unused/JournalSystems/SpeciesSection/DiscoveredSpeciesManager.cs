//using System.Collections.Generic;
//using UnityEngine;

//public class DiscoveredSpeciesManager : MonoBehaviour, ISetupSelectable
//{
//    [SerializeField] GameObject SpeciesDiscoveredContent = default;
//    [SerializeField] GameObject SpeciesPopupPrefab = default;
//    [SerializeField] SpeciesSectionManager speciesSectionManager = default;
//    [Header("For testing")]
//    [SerializeField] List<AnimalSpecies> LevelSpeciesPlaceholder = default;
//    [Header("RemoveSelfFromList and whatever else should happen")]
//    public ItemSelectedEvent SpeciesSelected = new ItemSelectedEvent();

//    public void Start()
//    {
//        this.AddDiscoveredSpecies();
//    }

//    // Filters out species that have already been discovered
//    public void AddDiscoveredSpecies()
//    {
//        foreach (AnimalSpecies species in this.LevelSpeciesPlaceholder)
//        {
//            // Journal data exists, need to filter
//            if (this.speciesSectionManager.JournalEntries != null)
//            {
//                if (!this.speciesSectionManager.JournalEntries.Entries.ContainsKey(species.SpeciesName))
//                {
//                    this.CreateNewDisplayObject(species);
//                }
//                else
//                {
//                    Debug.Log("Species \"" + species.SpeciesName + "\" already exists in journal");
//                }
//            }
//            // No journal data yet, just add
//            else
//            {
//                this.CreateNewDisplayObject(species);
//            }
//        }
//    }

//    private void CreateNewDisplayObject(AnimalSpecies species)
//    {
//        GameObject discoveredSpecies = Instantiate(this.SpeciesPopupPrefab, this.SpeciesDiscoveredContent.transform);
//        discoveredSpecies.GetComponent<SpeciesEntryDisplayLogic>().Initialize(species);
//        discoveredSpecies.GetComponent<SpeciesJournalData>().JournalEntry = new JournalEntry(species.SpeciesName);
//        this.SetupItemSelectedHandler(discoveredSpecies, this.SpeciesSelected);
//    }

//    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
//    {
//        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
//    }
//}
