using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResearchEncyclopediaUI : MonoBehaviour
{
    [SerializeField]
    [Expandable]
    [Tooltip("Object that holds all the research data")]
    private ResearchModel researchModel;

    [SerializeField]
    [Tooltip("Dropdown used to select available encyclopedia articles")]
    private TMP_Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
