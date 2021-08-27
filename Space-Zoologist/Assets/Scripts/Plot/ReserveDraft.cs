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
        GameManager.Instance.TryToPause();
        UpdateUI(false);
    }

    public void finishDrafting()
    {
        GameManager.Instance.Unpause();
        UpdateUI(true);
    }

    private void UpdateUI(bool onOff)
    {
        PlayerController.CanUseIngameControls = onOff;
        PauseButton.SetActive(onOff);
        NextDayButton.SetActive(onOff);
    }
}
