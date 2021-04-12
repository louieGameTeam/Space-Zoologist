using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Very similar to Species Popup Manager
public class DiscoveredNeedsManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsDiscoveredContent = default;
    [SerializeField] GameObject NeedsPopupPrefab = default;
    [Header("For testing")]
    [SerializeField] List<Need> LevelNeedsPlaceholder = default;
    [Header("RemoveSelfFromList and whatever else should happen")]
    public ItemSelectedEvent NeedSelected = new ItemSelectedEvent();
    private List<GameObject> DiscoveredNeeds = new List<GameObject>();
    private GameObject EntrySelected = default;

    // Disabled on startup
    public void Start()
    {
        this.AddDiscoveredNeeds();
        this.gameObject.SetActive(false);
    }

    public void AddDiscoveredNeeds()
    {
        foreach (var need in this.LevelNeedsPlaceholder)
        {
            GameObject discoveredNeed = Instantiate(this.NeedsPopupPrefab, this.NeedsDiscoveredContent.transform);
            discoveredNeed.GetComponent<NeedData>().Need = need;
            discoveredNeed.GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(need);
            this.SetupItemSelectedHandler(discoveredNeed, this.NeedSelected);
            // For filtering
            discoveredNeed.name = need.NeedName;
            DiscoveredNeeds.Add(discoveredNeed);
        }
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }

    public void FilterPotentialNeeds()
    {
        this.FilterPotentialNeeds(this.EntrySelected);
    }
    // If need already added as need in species journal data, don't show it as a need that can be added
    public void FilterPotentialNeeds(GameObject speciesSelected)
    {
        this.EntrySelected = speciesSelected;
        JournalEntry speciesJournalData = speciesSelected.GetComponent<SpeciesJournalData>().JournalEntry;
        foreach(GameObject need in this.DiscoveredNeeds)
        {
            if (speciesJournalData.DiscoveredNeeds.ContainsKey(need.name))
            {
                need.SetActive(false);
            }
            else
            {
                need.SetActive(true);
            }
        }
    }
}
