using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveDraft : MonoBehaviour
{
    [SerializeField] PlotIO plotIO;
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
    SerializedPlot draftPlot;
    SerializedPlot plot;
    float initialBalance;



    public void Start()
    {
        plotIO = FindObjectOfType<PlotIO>();
        plot = plotIO.SavePlot();
        draftPlot = plotIO.SavePlot();
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
        plotIO.LoadPlot(this.draftPlot);
        PauseManager.TryToPause();

        // save current resources
        resourceManager.Save();
        initialBalance = playerBalance.Balance;
        UpdateUI(false);
    }

    public void finishDrafting()
    {
        plotIO.LoadPlot(this.plot);
        PauseManager.Unpause();

        // load resources - won't change if applied draft
        resourceManager.Load();
        playerBalance.SetBalance(initialBalance);
        UpdateUI(true);
    }

    public void applyDraft()
    {
        plot = plotIO.SavePlot();

        // save changes
        resourceManager.Save();
        initialBalance = playerBalance.Balance;
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
        plotIO.LoadPlot(this.draftPlot);
        plot = plotIO.SavePlot();
        draftPlot = plotIO.SavePlot();
    }
}
