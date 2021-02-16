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
    // TODO refactor UI stuff into seperate script
    [SerializeField] GameObject StoreButtons = default;
    [SerializeField] List<GameObject> StoreMenus = default;
    [SerializeField] GameObject PlaceHolder = default;
    [SerializeField] GameObject NextDayButton = default;
    [SerializeField] GameObject PauseButton = default;
    [SerializeField] PlayerController PlayerController = default;
    [SerializeField] GameObject DraftingButton = default;


    public void startDrafting()
    {
        GridIO.LoadGrid(currentLevel + "Draft");
        PauseManager.TryToPause();
        setupStoreStuff();
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.gameObject.SetActive(false);
        }
        NextDayButton.SetActive(false);
        PlayerController.CanUseIngameControls = false;
        PauseButton.SetActive(false);
        DraftingButton.SetActive(false);
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
        NextDayButton.SetActive(true);
        PlayerController.CanUseIngameControls = true;
        PauseButton.SetActive(true);
        DraftingButton.SetActive(true);
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
