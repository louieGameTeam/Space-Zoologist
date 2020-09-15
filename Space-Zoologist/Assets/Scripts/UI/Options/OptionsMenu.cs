using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] PauseManager PauseManager = default;
    public void CloseOptionsMenu()
    {
        this.gameObject.SetActive(false);
        this.PauseManager.TryToUnpause();
        this.PauseManager.PauseButton.SetActive(true);
    }

    public void ToggleOptionsMenu()
    {
        if (this.gameObject.activeSelf)
        {
            this.PauseManager.TryToUnpause();
            this.PauseManager.PauseButton.SetActive(true);
        }
        else
        {
            this.PauseManager.TryToPause();
            this.PauseManager.PauseButton.SetActive(false);
        }
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
