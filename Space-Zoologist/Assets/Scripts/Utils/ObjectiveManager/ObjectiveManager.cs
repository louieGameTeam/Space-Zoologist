using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    [Expandable] public LevelObjectives levelObjectives = default;

    private bool isOpen = false;

    // To access the populations 
    [SerializeField] private PopulationManager populationManager = default;
    // To access the player balance
    [SerializeField] private PlayerBalance playerBalance = default;
    // Objective panel
    [SerializeField] private GameObject objectivePanel = default;

    public void ToggleObjectivePanel()
    {
        this.isOpen = !this.isOpen;
        this.objectivePanel.SetActive(this.isOpen);
    }
}
