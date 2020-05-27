using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Uses the SpeciesJournalData from the currently selected species to update and manage the species needs data
/// </summary>
/// TODO 1: refactor and clearly comment logic and update documentation
/// TODO 2: Setup research and need removal
public class NeedsSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsContainer = default;
    [SerializeField] GameObject NeedDisplayPrefab = default;
    [SerializeField] Text NeedNameText = default;
    [SerializeField] SpeciesNeedReferenceData SpeciesNeedReferenceData = default;
    [SerializeField] InputField DescriptionText = default;
    [Header("For testing")]
    private List<GameObject> AllNeedsDisplay = new List<GameObject>();
    private JournalEntry DisplayedEntry = default;
    public ItemSelectedEvent NeedSelected = new ItemSelectedEvent();
    // Toggle group wasn't working, making our own for now
    // Works well for determining which item is currently selected
    private List<Toggle> m_Toggles = new List<Toggle>();

    public void Start()
    {
        this.InstantiateNeedObjects();     
    }

    private void InstantiateNeedObjects()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newNeedDisplay = Instantiate(this.NeedDisplayPrefab, this.NeedsContainer.transform);
            newNeedDisplay.SetActive(false);
            this.SetupItemSelectedHandler(newNeedDisplay, this.NeedSelected);
            this.AllNeedsDisplay.Add(newNeedDisplay);
            this.m_Toggles.Add(newNeedDisplay.GetComponent<Toggle>());
        }  
    }

    // Add discovered needs and update NeedData's reference to saved descriptions
    public void SetupDiscoveredNeeds(GameObject species)
    {
        this.ClearPreviousNeeds();
        SpeciesJournalData speciesData = species.GetComponent<SpeciesJournalData>();
        this.DisplayedEntry = speciesData.JournalEntry;
        int i = 0;
        foreach(string needToDisplay in speciesData.JournalEntry.DiscoveredNeeds)
        {
            SpeciesNeed needData = this.SpeciesNeedReferenceData.AllNeeds[needToDisplay];
            this.AllNeedsDisplay[i].GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(needData);
            NeedData need = this.AllNeedsDisplay[i].GetComponent<NeedData>();
            need.Need = needData;
            this.InitializeNeedDescription(need);
            i++;
        }
    }
    // update DisplayedEntry needs data. Assume need has been filtered to be unique
    public void SetupNewNeedDisplay(GameObject needPopup)
    {
        foreach(GameObject newNeed in this.AllNeedsDisplay)
        {
            if (!newNeed.activeSelf)
            {
                newNeed.SetActive(true);
                SpeciesNeed speciesNeed = needPopup.GetComponent<NeedData>().Need;
                newNeed.GetComponent<NeedData>().Need = speciesNeed;
                newNeed.GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(speciesNeed);
                this.DisplayedEntry.DiscoveredNeeds.Add(speciesNeed.Name.ToString());
                this.InitializeNeedDescription(newNeed.GetComponent<NeedData>());
                break;
            }
        }
    }

    private void InitializeNeedDescription(NeedData need)
    {
        need.Description = "";
        if (this.DisplayedEntry.DiscoveredNeedsEntryText.ContainsKey(need.Need.Name.ToString()))
        {
            need.Description = this.DisplayedEntry.DiscoveredNeedsEntryText[need.Need.Name.ToString()];
        }
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }

    private void ClearPreviousNeeds()
    {
        foreach(GameObject need in this.AllNeedsDisplay)
        {
            need.SetActive(false);
        }
        foreach(Toggle toggle in this.m_Toggles)
        {
            toggle.isOn = false;
        }
        this.NeedNameText.text = "";
    }

    public void ChangeNeedNameText(GameObject need)
    {
       this.NeedNameText.text = need.GetComponent<NeedData>().Need.Name.ToString();
    }

    // Find currently selected need, update it's current description data and update journal entry description data
    public void UpdateNeedDescription(string description)
    {
        foreach(Toggle entry in this.m_Toggles)
        {
            if (entry.isOn)
            {
                NeedData need = entry.gameObject.GetComponent<NeedData>();
                need.Description = description;
                if (this.DisplayedEntry.DiscoveredNeedsEntryText.ContainsKey(need.Need.Name.ToString()))
                {
                    this.DisplayedEntry.DiscoveredNeedsEntryText[need.Need.Name.ToString()] = description;
                }
                else
                {
                    this.DisplayedEntry.DiscoveredNeedsEntryText.Add(need.Need.Name.ToString(), description);
                }
                break;
            }
        }
    }

    public void PrintEntryData(GameObject needSelected)
    {
        // Debug.Log("Current entry: " + this.DisplayedEntry.DiscoveredSpecies);
    }
}
