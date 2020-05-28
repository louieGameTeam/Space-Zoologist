using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GameObject currentMenu = null;

    public void OnToggleMenu(GameObject menu)
    {
        if (currentMenu != menu)
        {
            if (currentMenu)
            {
                currentMenu.SetActive(false);
            }
            menu.SetActive(true);
            currentMenu = menu;
        }
        else
        {
            currentMenu = null;
            menu.SetActive(false);
        }
    }
}
