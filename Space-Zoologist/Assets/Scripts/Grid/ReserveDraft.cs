using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveDraft : MonoBehaviour
{
    [SerializeField] GridIO GridIO = default;
    [SerializeField] string currentLevel = "Level1";
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] PauseManager PauseManager = default;
    [SerializeField] DialogueManager DialogueManager = default;
    [SerializeField] GameObject StoreButtons = default;
    [SerializeField] List<GameObject> StoreMenus = default;
    [SerializeField] GameObject PlaceHolder = default;

    public void startDrafting()
    {
        GridIO.LoadGrid(currentLevel + "Draft");
        PauseManager.TryToPause();
        setupStoreStuff();
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.gameObject.SetActive(false);
        }
    }

    public void finishDrafting()
    {
        GridIO.SaveGrid(currentLevel + "Draft");
        GridIO.LoadGrid(currentLevel);
        closeStoreStuff();
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.gameObject.SetActive(true);
        }
        PauseManager.Unpause();
    }

    // Load drafted level and overwrite previous save files with new level
    public void loadDraft()
    {
        GridIO.LoadGrid(currentLevel + "Draft");
        GridIO.SaveGrid(currentLevel);
        GridIO.SaveGrid(currentLevel + "Draft");
    }

    private void setupStoreStuff()
    {
        DialogueManager.StoreButtonsGameObject = StoreButtons;
        StoreButtons.SetActive(true);
    }

    private void closeStoreStuff()
    {
        DialogueManager.StoreButtonsGameObject = PlaceHolder;
        StoreButtons.SetActive(false);
        foreach (GameObject storeMenu in this.StoreMenus)
        {
            storeMenu.SetActive(false);
        }
    }
}
