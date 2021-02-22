using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveDraft : MonoBehaviour
{
    [SerializeField] GridIO GridIO = default;
    [SerializeField] string currentLevel = "Level1";
    [SerializeField] PopulationManager PopulationManager = default;
    [SerializeField] PauseManager PauseManager = default;
    // TODO refactor UI stuff into seperate script
    //[SerializeField] GameObject StoreButtons = default;
    //[SerializeField] List<GameObject> StoreMenus = default;
    [SerializeField] GameObject NextDayButton = default;
    [SerializeField] GameObject PauseButton = default;
    [SerializeField] PlayerController PlayerController = default;
    //[SerializeField] GameObject DraftingButton = default;
    [SerializeField] GameObject FinishDrafting = default;
    [SerializeField] GameObject CancelDrafting = default;


    public void Start()
    {
        GridIO.SaveGrid(currentLevel + "Draft");
    }

    public void startDrafting()
    {
        GridIO.LoadGrid(currentLevel + "Draft");

        setAnimalsVisible(false);
        setupStoreStuff();
        PauseManager.TryToPause();
        UpdateUI(false);
    }

    public void finishDrafting()
    {
        GridIO.SaveGrid(currentLevel + "Draft");
        GridIO.LoadGrid(currentLevel);

        closeStoreStuff();
        setAnimalsVisible(true);
        PauseManager.Unpause();
        UpdateUI(true);
    }

    public void cancelDrafting()
    {
        GridIO.LoadGrid(currentLevel);

        closeStoreStuff();
        setAnimalsVisible(true);
        PauseManager.Unpause();
        UpdateUI(true);
    }

    public void applyDraft()
    {
        GridIO.SaveGrid(currentLevel);
        GridIO.SaveGrid(currentLevel + "Draft");

        closeStoreStuff();
        setAnimalsVisible(true);
        PauseManager.Unpause();
        UpdateUI(true);
    }

    private void setAnimalsVisible(bool isVisible)
    {
        foreach (Population population in this.PopulationManager.Populations)
        {
            population.gameObject.SetActive(isVisible);
        }
    }

    private void UpdateUI(bool onOff)
    {
        NextDayButton.SetActive(onOff);
        PlayerController.CanUseIngameControls = onOff;
        PauseButton.SetActive(onOff);
        //DraftingButton.SetActive(onOff);
        FinishDrafting.SetActive(!onOff);
        CancelDrafting.SetActive(!onOff);
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
        //StoreButtons.SetActive(true);
    }

    private void closeStoreStuff()
    {
        //StoreButtons.SetActive(false);
        //foreach (GameObject storeMenu in this.StoreMenus)
        //{
        //    storeMenu.SetActive(false);
        //}
    }
}
