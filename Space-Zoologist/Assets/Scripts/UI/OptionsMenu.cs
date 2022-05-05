using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject IngameUI = default;
    public bool IsInOptionsMenu => gameObject.activeSelf;


    public void CloseOptionsMenu()
    {
        this.gameObject.SetActive(false);
        GameManager.Instance.TryToUnpause("OptionsMenu");
        //this.PauseManager.PauseButton.SetActive(true);
        DialogueEditor.ConversationManager.Instance.UnfreezeConversation();
        //this.IngameUI.SetActive(true);
    }

    public void OpenOptionsMenu()
    {
        this.gameObject.SetActive(true);
        //this.IngameUI.SetActive(false);
        GameManager.Instance.TryToPause("OptionsMenu");
        DialogueEditor.ConversationManager.Instance.FreezeConversation();
        //this.PauseManager.PauseButton.SetActive(true);
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
