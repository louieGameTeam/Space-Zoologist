using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveDraft : MonoBehaviour
{
    [SerializeField] GridIO GridIO = default;
    [SerializeField] string currentLevel = "Level1";
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] PauseManager PauseManager = default;

    public void startDrafting()
    {
        GridIO.LoadGrid(currentLevel + "Draft");
        PauseManager.TryToPause();
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.gameObject.SetActive(false);
        }
    }

    public void finishDrafting()
    {
        GridIO.SaveGrid(currentLevel + "Draft");
        GridIO.LoadGrid(currentLevel);
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.gameObject.SetActive(true);
        }
        PauseManager.Unpause();
    }
}
