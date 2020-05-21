using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Very similar to Species Popup Manager
// TODO setup add need popup so it can only come up when a species is selected and it adds the need to the list of discovered needs
// TODO fix visual display issues
public class NeedsPopupManager : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsDiscoveredContent = default;
    [SerializeField] GameObject NeedsPopupPrefab = default;
    [Header("For testing")]
    [SerializeField] List<SpeciesNeed> NeedsPlaceholder = default;
    [Header("RemoveSelfFromList and whatever else should happen")]
    public ItemSelectedEvent NeedSelected = new ItemSelectedEvent();

    public void Start()
    {
        this.AddDiscoveredNeeds();
    }

    public void AddDiscoveredNeeds()
    {
        foreach (var need in this.NeedsPlaceholder)
        {
            GameObject discoveredNeed = Instantiate(this.NeedsPopupPrefab, this.NeedsDiscoveredContent.transform);
            discoveredNeed.GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(need);
            discoveredNeed.GetComponent<ItemData>().JournalItemData = need;
            this.SetupItemSelectedHandler(discoveredNeed, this.NeedSelected);
        }
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }
}
