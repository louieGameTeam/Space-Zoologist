using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveDraft : MonoBehaviour
{
    public bool IsToggled => isToggled;
    [SerializeField] PauseManager PauseManager = default;
    [SerializeField] GameObject PauseButton = default;
    [SerializeField] GameObject NextDayButton = default;
    [SerializeField] PlayerController PlayerController = default;
    private bool isToggled = false;

    [SerializeField] PlayerBalance playerBalance = default;
    SerializedPlot draftPlot;
    SerializedPlot plot;
    float initialBalance;

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
        PauseManager.TryToPause();

        // save current resources
        // this is currently an issue
        UpdateUI(false);
    }

    public void finishDrafting()
    {
        PauseManager.Unpause();
        UpdateUI(true);
    }

    private void UpdateUI(bool onOff)
    {
        PlayerController.CanUseIngameControls = onOff;
        PauseButton.SetActive(onOff);
        NextDayButton.SetActive(onOff);
    }
}
