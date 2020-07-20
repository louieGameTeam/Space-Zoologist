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
            Debug.Log("In store");
            NeedSystemUpdater.ins.isInStore = true;

            if (currentMenu)
            {
                currentMenu.SetActive(false);
            }
            menu.SetActive(true);
            currentMenu = menu;
        }
        else
        {
            Debug.Log("Exit Store");
            NeedSystemUpdater.ins.isInStore = false;

            currentMenu = null;
            menu.SetActive(false);
        }
    }
}
