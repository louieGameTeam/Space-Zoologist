using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Uses the SpeciesJournalData from the currently selected species entry to update and manage the species needs data
/// </summary>
public class NeedsSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsContainer = default;
    [SerializeField] GameObject NeedDisplayPrefab = default;
    [SerializeField] SpeciesNeedReferenceData SpeciesNeedReferenceData = default;
    [SerializeField] Text NeedNameText = default;
    [SerializeField] ResearchSectionManager researchSectionManager = default;
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
            // Use SpeciesNeedReferenceData to turn the string into the Need 
            SpeciesNeed speciesNeed = this.SpeciesNeedReferenceData.AllNeeds[needToDisplay];
            this.AllNeedsDisplay[i].GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(speciesNeed);
            // Update the current NeedData 
            NeedData need = this.AllNeedsDisplay[i].GetComponent<NeedData>();
            need.Need = speciesNeed;
            this.InitializeNeedDescription(need);
            i++;
        }
    }

    // update display's NeedData and JournalEntry's list of discovered needs.
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

    // Put in description if found in Journal Entry data
    public void InitializeNeedDescription(NeedData need)
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

    // Turn off display gameobjects and set all toggles off
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

    // Find currently selected need, update it's current description data, and update journal entry description data
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

    // Updates DisplayedEntry data and turns off GameObject
    public void RemoveSelectedNeed()
    {
        foreach(Toggle entry in this.m_Toggles)
        {
            if (entry.isOn)
            {
                NeedData need = entry.gameObject.GetComponent<NeedData>();
                this.DisplayedEntry.RemoveNeed(need.Need.Name.ToString());
                entry.gameObject.SetActive(false);
            }
        }
    }

    public void ResearchSelectedNeed()
    {
        foreach(Toggle entry in this.m_Toggles)
        {
            if (entry.isOn)
            {
                this.researchSectionManager.CanResearch(entry.gameObject);
            }
        }
    }
}
