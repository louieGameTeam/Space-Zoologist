using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] NeedSystemUpdater NeedSystemUpdater = default;
    [SerializeField] BehaviorPatternUpdater BehaviorPatternUpdater = default;
    [SerializeField] GridSystem GridSystem = default;

    public bool IsPaused { get; private set; }

    public void Pause()
    {
        this.IsPaused = true;
        this.BehaviorPatternUpdater.IsPaused = true;
        this.NeedSystemUpdater.IsPaused = true;
        this.PauseAllAnimalsMovementController();
        this.GridSystem.UpdateAnimalCellGrid();
        this.GridSystem.HighlightHomeLocations();
    }

    public void Unpause()
    {
        this.IsPaused = false;
        this.BehaviorPatternUpdater.IsPaused = false;
        this.NeedSystemUpdater.IsPaused = false;
        this.PopulationManager.UpdateAccessibleLocations();
        this.UnpauseAllAnimalsMovementController();
        this.GridSystem.UnhighlightHomeLocations();
    }

    public void PauseAllAnimalsMovementController()
    {
       foreach (Population population in PopulationManager.Populations)
        {
            population.PauseAnimalsMovementController();
        }
    }

    public void UnpauseAllAnimalsMovementController()
    {
        foreach (Population population in PopulationManager.Populations)
        {
            population.UnpauseAnimalsMovementController();
        }
    }
}
