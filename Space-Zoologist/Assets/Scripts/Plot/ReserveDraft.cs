using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveDraft : MonoBehaviour
{
    [SerializeField] GridIO GridIO = default;
    [SerializeField] string currentLevel = "Level1";
    [SerializeField] PauseManager PauseManager = default;
    // TODO refactor UI stuff into seperate script
    //[SerializeField] GameObject StoreButtons = default;
    //[SerializeField] List<GameObject> StoreMenus = default;
    [SerializeField] GameObject PauseButton = default;
    [SerializeField] GameObject NextDayButton = default;
    [SerializeField] PlayerController PlayerController = default;
    private bool isToggled = false;

    [SerializeField] ResourceManager resourceManager = default;
    [SerializeField] PlayerBalance playerBalance = default;
    float savedBalance;
    


    public void Start()
    {
        GridIO.SaveGrid(currentLevel);
        GridIO.SaveGrid(currentLevel + "Draft");
    }

    public void toggleDrafting()
    {
        if (!isToggled)
        {
            startDrafting();
            isToggled = true;
        }
        else
        {
            finishDrafting();
            isToggled = false;
        }
    }

    public void startDrafting()
    {
        GridIO.LoadGrid(currentLevel + "Draft");
        PauseManager.TryToPause();
        resourceManager.StartDraft();
        savedBalance = playerBalance.Balance;
        UpdateUI(false);
    }

    public void finishDrafting()
    {
        GridIO.LoadGrid(currentLevel);
        PauseManager.Unpause();
        resourceManager.CancelDraft();
        playerBalance.SetBalance(savedBalance);
        UpdateUI(true);
    }

    public void applyDraft()
    {
        GridIO.SaveGrid(currentLevel + "Draft");
        resourceManager.ApplyDraft();
        savedBalance = playerBalance.Balance;
    }

    private void UpdateUI(bool onOff)
    {
        PlayerController.CanUseIngameControls = onOff;
        PauseButton.SetActive(onOff);
        NextDayButton.SetActive(onOff);
    }

    // Load drafted level and overwrite previous save files with new level
    public void loadDraft()
    {
        GridIO.LoadGrid(currentLevel + "Draft");
        GridIO.SaveGrid(currentLevel);
        GridIO.SaveGrid(currentLevel + "Draft");
    }
}
