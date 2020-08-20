using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    GameObject currentMenu = null;
    [SerializeField] GameObject PlayerBalance = default;
    [SerializeField] NeedSystemUpdater NeedSystemUpdater = default;
    [SerializeField] List<StoreSection> StoreMenus = default;
    // PodMenu had original different design so could refactor to align with store sections but works for now
    [SerializeField] PodMenu PodMenu = default;
    [Header("Shared menu dependencies")]
    [SerializeField] LevelDataReference LevelDataReference = default;
    [SerializeField] CursorItem CursorItem = default;
    [SerializeField] List<RectTransform> UIElements = default;
    [SerializeField] TileSystem TileSystem = default;

    public void Start()
    {
        foreach (StoreSection storeMenu in this.StoreMenus)
        {
            storeMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.TileSystem);
            storeMenu.Initialize();
        }
        PodMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.TileSystem);
        PodMenu.Initialize();
    }

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
