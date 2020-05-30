using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconsDisplayLogic : MonoBehaviour
{
    [SerializeField] GameObject AddNeedIcon = default;
    [SerializeField] GameObject RemoveNeedIcon = default;
    [SerializeField] GameObject ResearchNeedIcon = default;

    public void TurnOnIcons()
    {
        this.AddNeedIcon.SetActive(true);
        this.RemoveNeedIcon.SetActive(true);
        this.ResearchNeedIcon.SetActive(true);
    }
}
