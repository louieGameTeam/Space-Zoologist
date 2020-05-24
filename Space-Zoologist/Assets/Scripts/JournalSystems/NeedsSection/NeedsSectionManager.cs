using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NeedsSectionManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsContainer = default;
    [SerializeField] GameObject NeedDisplayPrefab = default;
    [SerializeField] Text NeedNameText = default;
    [Header("For testing")]
    private List<GameObject> AllNeedsDisplay = new List<GameObject>();
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

    public void DisplayNewNeed(GameObject need)
    {
        SpeciesNeed speciesNeed = need.GetComponent<ItemData>().SpeciesNeedItemData;
        foreach(GameObject newNeed in this.AllNeedsDisplay)
        {
            if (!newNeed.activeSelf)
            {
                newNeed.SetActive(true);
                newNeed.GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(speciesNeed);
                break;
            }
        }
    }

    public void SetupDiscoveredNeeds(GameObject species)
    {
        SpeciesJournalData speciesData = species.GetComponent<SpeciesJournalData>();
        // int i = 0;
        // foreach(SpeciesNeed needToDisplay in speciesData.JournalEntry.DiscoveredNeeds)
        // {
        //     this.AllNeedsDisplay[i].GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(needToDisplay);
        //     i++;
        // }
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }

    // Could be refactored if more display properties need to be changed
    public void ChangeNeedNameText(GameObject need)
    {
        this.NeedNameText.text = need.GetComponent<ItemData>().SpeciesNeedItemData.Name.ToString();
    }
}
