using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// TODO setup need selection and toggles
public class NeedsSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsContainer = default;
    [SerializeField] GameObject NeedDisplayPrefab = default;
    [SerializeField] Text NeedNameText = default;
    [SerializeField] SpeciesNeedReferenceData SpeciesNeedReferenceData = default;
    [Header("For testing")]
    private List<GameObject> AllNeedsDisplay = new List<GameObject>();
    private JournalEntry DisplayedEntry = default;
    public ItemSelectedEvent NeedSelected = new ItemSelectedEvent();

    public void Start()
    {
        this.NeedSelected.AddListener(this.ChangeNeedNameText);
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
            this.SetupNeedData(this.AllNeedsDisplay[i], needToDisplay, needData, speciesData);
            i++;
        }
    }

    private void SetupNeedData(GameObject needDisplay, string needToDisplay, SpeciesNeed needData, SpeciesJournalData speciesJournalData)
    {
        NeedData need = needDisplay.GetComponent<NeedData>();
        need.Need = needData;
        if (speciesJournalData.JournalEntry.DiscoveredNeedsEntryText.ContainsKey(needToDisplay))
        {
            need.Description = speciesJournalData.JournalEntry.DiscoveredNeedsEntryText[needToDisplay];
        }
        else
        {
            need.Description = "Write something here!";
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
                break;
            }
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
    }

    public void ChangeNeedNameText(GameObject need)
    {
       
    }
}
