using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GameObject currentMenu = null;
    [SerializeField] GameObject PlayerBalance = default;
    [SerializeField] NeedSystemUpdater NeedSystemUpdater = default;

    public void OnToggleMenu(GameObject menu)
    {
        if (currentMenu != menu)
        {
            if (currentMenu)
            {
                currentMenu.SetActive(false);
            }
            this.PlayerBalance.SetActive(true);
            menu.SetActive(true);
            currentMenu = menu;
            NeedSystemUpdater.isInStore = true;
            NeedSystemUpdater.PauseAllAnimals();

        }
        else
        {
            NeedSystemUpdater.isInStore = false;
            NeedSystemUpdater.UpdateAccessibleLocations();
            NeedSystemUpdater.UnpauseAllAnimals();
            currentMenu = null;
            menu.SetActive(false);
            this.PlayerBalance.SetActive(false);
        }
    }
}
