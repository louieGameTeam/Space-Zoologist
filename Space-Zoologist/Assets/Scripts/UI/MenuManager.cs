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
    [SerializeField] TileSystem TileSystem = default;
    [SerializeField] GridSystem GridSystem = default;
    [SerializeField] List<RectTransform> UIElements = default;

    public void Start()
    {
        foreach (StoreSection storeMenu in this.StoreMenus)
        {
            storeMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.TileSystem, this.GridSystem);
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
            menu.SetActive(true);
            currentMenu = menu;
            this.StoreToggledOn();

        }
        else
        {
            currentMenu = null;
            menu.SetActive(false);
            this.StoreToggledOff();
        }
    }

    private void StoreToggledOn()
    {
        this.PlayerBalance.SetActive(true);
        NeedSystemUpdater.isInStore = true;
        NeedSystemUpdater.PauseAllAnimals();
        this.GridSystem.UpdateAnimalCellGrid();
    }

    private void StoreToggledOff()
    {
        this.PlayerBalance.SetActive(false);
        NeedSystemUpdater.isInStore = false;
        NeedSystemUpdater.UpdateAccessibleLocations();
        NeedSystemUpdater.UnpauseAllAnimals();
    }
}
