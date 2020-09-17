using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] PauseManager PauseManager = default;
    [SerializeField] GameObject IngameUI = default;
    public bool IsInOptionsMenu => gameObject.activeSelf;


    public void CloseOptionsMenu()
    {
        this.gameObject.SetActive(false);
        this.PauseManager.TryToUnpause();
        this.PauseManager.PauseButton.SetActive(true);
        this.IngameUI.SetActive(true);
    }

    public void OpenOptionsMenu()
    {
        this.gameObject.SetActive(true);
        this.IngameUI.SetActive(false);
        this.PauseManager.TryToPause();
        this.PauseManager.PauseButton.SetActive(true);
    }

    public void ToggleOptionsMenu()
    {
        if (this.gameObject.activeSelf)
        {
            this.CloseOptionsMenu();
        }
        else
        {
            this.OpenOptionsMenu();
        }
        //this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
