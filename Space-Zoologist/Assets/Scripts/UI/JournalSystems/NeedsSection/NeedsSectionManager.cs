using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Uses the SpeciesJournalData from the currently selected species entry to update and manage the species needs data
/// </summary>
public class NeedsSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsContainer = default;
    [SerializeField] GameObject NeedDisplayPrefab = default;
    [Header("For translating need strings to need SO")]
    [SerializeField] SpeciesNeedReferenceData SpeciesNeedReferenceData = default;
    [SerializeField] Text NeedNameText = default;
    private List<GameObject> AllNeedsDisplay = new List<GameObject>();
    // Update the JournalData for the SelectedSpecies whenever a change is made
    private JournalEntry SelectedSpeciesJournalData = default;
    public ItemSelectedEvent NeedSelected = new ItemSelectedEvent();
    public ItemSelectedEvent CheckResearch = new ItemSelectedEvent();
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

    // Add discovered needs and update's reference to saved descriptions
    public void SetupSelectedSpeciesNeeds(GameObject species)
    {
        this.ClearPreviousChanges();
        SpeciesJournalData speciesData = species.GetComponent<SpeciesJournalData>();
        this.SelectedSpeciesJournalData = speciesData.JournalEntry;
        int i = 0;
        foreach(var needToDisplay in speciesData.JournalEntry.DiscoveredNeeds)
        {
            // Use SpeciesNeedReferenceData to turn the string into the Need
            Need speciesNeed = this.SpeciesNeedReferenceData.FindNeed(needToDisplay.Key);
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
                Need speciesNeed = needPopup.GetComponent<NeedData>().Need;
                newNeed.GetComponent<NeedData>().Need = speciesNeed;
                newNeed.GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(speciesNeed);
                this.SelectedSpeciesJournalData.DiscoveredNeeds.Add(speciesNeed.NeedName, new JournalNeedResearch(speciesNeed.NeedName));
                this.InitializeNeedDescription(newNeed.GetComponent<NeedData>());
                break;
            }
        }
    }

    // Put in description if found in Journal Entry data
    public void InitializeNeedDescription(NeedData need)
    {
        need.Description = "";
        if (this.SelectedSpeciesJournalData.DiscoveredNeeds.ContainsKey(need.Need.NeedName))
        {
            need.Description = this.SelectedSpeciesJournalData.DiscoveredNeeds[need.Need.NeedName].NeedDescription;
        }
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }

    // Turn off display gameobjects and set all toggles off
    private void ClearPreviousChanges()
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
       this.NeedNameText.text = need.GetComponent<NeedData>().Need.NeedName;
    }

    // Find currently selected need, update it's current description data, and update journal entry description data
    public void UpdateNeedDescription(string description)
    {
        GameObject entry = this.FindSelectedNeed();
        if (entry != null)
        {
            NeedData need = entry.gameObject.GetComponent<NeedData>();
            need.Description = description;
            if (this.SelectedSpeciesJournalData.DiscoveredNeeds.ContainsKey(need.Need.NeedName))
            {
                this.SelectedSpeciesJournalData.DiscoveredNeeds[need.Need.NeedName].NeedDescription = description;
            }
            else
            {
                JournalNeedResearch newNeed = new JournalNeedResearch(need.Need.NeedName);
                newNeed.NeedDescription = description;
                this.SelectedSpeciesJournalData.DiscoveredNeeds.Add(need.Need.NeedName, newNeed);
            }
        }
    }

    // Updates DisplayedEntry data and turns off GameObject
    public void RemoveSelectedNeed()
    {
        GameObject entry = this.FindSelectedNeed();
        if (entry != null)
        {
            NeedData need = entry.gameObject.GetComponent<NeedData>();
            this.SelectedSpeciesJournalData.DiscoveredNeeds.Remove(need.Need.NeedName);
            entry.gameObject.SetActive(false);
        }
    }

    public void ResearchSelectedNeed()
    {
        GameObject entry = this.FindSelectedNeed();
        if (entry != null)
        {
            this.CheckResearch.Invoke(entry.gameObject);
        }
    }

    // If an object is not selected, but an icon is clicked, null should be checked and nothing should
    private GameObject FindSelectedNeed()
    {
        foreach(Toggle entry in this.m_Toggles)
        {
            if (entry.isOn && entry.gameObject.activeSelf)
            {
                return entry.gameObject;
            }
        }
        return null;
    }
}
