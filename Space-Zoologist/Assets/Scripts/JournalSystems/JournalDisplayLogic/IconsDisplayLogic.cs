using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconsDisplayLogic : MonoBehaviour
{
    [SerializeField] GameObject AddNeedIcon = default;
    [SerializeField] GameObject RemoveNeedIcon = default;
    [SerializeField] GameObject ResearchNeedIcon = default;
    [SerializeField] GameObject ResearchedNeedIcon = default;

    public void TurnOnIcons()
    {
        this.AddNeedIcon.SetActive(true);
        this.RemoveNeedIcon.SetActive(true);
        this.ResearchNeedIcon.SetActive(true);
    }

    public void UpdateNeedIcons(GameObject needSelected)
    {
        NeedData needData = needSelected.GetComponent<NeedData>();
        // if (needData.isResearched)
        // {
        //     this.ResearchNeedIcon.SetActive(false);
        //     this.ResearchedNeedIcon.SetActive(true);
        // }
        // else
        // {
        //     this.ResearchNeedIcon.SetActive(true);
        //     this.ResearchedNeedIcon.SetActive(false);
        // }
    }
}
