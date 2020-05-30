using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

// TODO 1 figure out how researching data should be tracked and stored
// TODO 2 figure out how research should work if need removed
// TODO 3 figure out how research works behind the scenes
public class ResearchSectionManager : MonoBehaviour
{
    [SerializeField] GameObject NeedDisplayPrefab = default;
    [SerializeField] GameObject ResearchContainer = default;
    [SerializeField] GameObject ResearchPopup = default;
    public ItemSelectedEvent MouseEnterEvent = new ItemSelectedEvent();
    public UnityEvent MouseExitEvent = new UnityEvent(); 

    private List<GameObject> ResearchNeeds = new List<GameObject>();

    public void Start()
    {
        this.InstantiateResearchObjects();    
    }

    private void InstantiateResearchObjects()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject newNeedDisplay = Instantiate(this.NeedDisplayPrefab, this.ResearchContainer.transform);
            newNeedDisplay.AddComponent(typeof(OnMouseHover));
            OnMouseHover m = newNeedDisplay.GetComponent<OnMouseHover>();
            m.MouseEnterEvent = this.MouseEnterEvent;
            m.MouseExitEvent = this.MouseExitEvent;
            newNeedDisplay.SetActive(false);
            this.ResearchNeeds.Add(newNeedDisplay);
        }  
    }

    // Adds selected need to research list
    private void AddNewResearchObject(GameObject need)
    {
        NeedData needData = need.GetComponent<NeedData>();
        foreach (GameObject researchNeed in this.ResearchNeeds)
        {
            if (!researchNeed.activeSelf)
            {
                researchNeed.GetComponent<NeedData>().Need = needData.Need;
                researchNeed.GetComponent<NeedsEntryDisplayLogic>().SetupDisplay(needData.Need);
                researchNeed.SetActive(true);
                researchNeed.name = needData.Need.Name.ToString();
                break;
            }
        }
    }

    // TODO figure out how to determine if a need can be researched
    public void CanResearch(GameObject need)
    {
        NeedData needData = need.GetComponent<NeedData>();
        foreach (GameObject researchNeed in this.ResearchNeeds)
        {
            if (researchNeed.activeSelf)
            {
                if (researchNeed.name.Equals(needData.Need.Name.ToString()))
                {
                    return;
                }
            }
        }
        this.AddNewResearchObject(need);
    }
}
