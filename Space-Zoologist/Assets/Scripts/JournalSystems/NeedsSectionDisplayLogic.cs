using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NeedsSectionDisplayLogic : MonoBehaviour, ISetupSelectable
{
    [SerializeField] GameObject NeedsContainer = default;
    [SerializeField] GameObject NeedDisplayPrefab = default;
    [SerializeField] Text NeedNameText = default;
    [Header("For testing")]
    [SerializeField] string NeedDescription = default;
    private List<GameObject> AllNeedsDisplay = new List<GameObject>();
    public ItemSelectedEvent NeedSelected = new ItemSelectedEvent();

    public void Start()
    {
        this.NeedSelected.AddListener(this.ChangeNeedNameText);
        for (int i = 0; i < 10; i++)
        {
            GameObject newNeedDisplay = Instantiate(this.NeedDisplayPrefab, this.NeedsContainer.transform);
            newNeedDisplay.SetActive(false);
            this.AllNeedsDisplay.Add(newNeedDisplay);
        }        
    }

    public void PopulateNeedDisplay(GameObject species)
    {
        SpeciesData speciesData = species.GetComponent<SpeciesData>();
        int i = 0;
        foreach(SpeciesNeed needToDisplay in speciesData.DiscoveredNeeds)
        {
            this.AllNeedsDisplay[i].SetActive(true);
            this.AllNeedsDisplay[i].GetComponent<Image>().sprite = needToDisplay.Sprite;
            this.AllNeedsDisplay[i].GetComponent<ItemData>().JournalItemData = needToDisplay;
            this.SetupItemSelectedHandler(this.AllNeedsDisplay[i], this.NeedSelected);
            i++;
        }
    }

    public void SetupItemSelectedHandler(GameObject item, ItemSelectedEvent action)
    {
        item.GetComponent<SelectableCanvasImage>().SetupItemSelectedHandler(action);
    }

    public void ChangeNeedNameText(GameObject need)
    {
        this.NeedNameText.text = need.GetComponent<ItemData>().JournalItemData.Name.ToString();
    }
}
