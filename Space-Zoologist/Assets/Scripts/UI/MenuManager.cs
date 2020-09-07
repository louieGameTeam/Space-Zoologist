using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO refactor PodMenu to inherit same as other store menus
public class MenuManager : MonoBehaviour
{
    GameObject currentMenu = null;
    [SerializeField] GameObject PlayerBalanceHUD = default;
    [SerializeField] List<StoreSection> StoreMenus = default;
    // PodMenu had original different design so could refactor to align with store sections but works for now
    [SerializeField] PodMenu PodMenu = default;
    [Header("Shared menu dependencies")]
    [SerializeField] PlayerBalance PlayerBalance = default;
    [SerializeField] CanvasObjectStrobe PlayerBalanceDisplay = default;
    [SerializeField] LevelDataReference LevelDataReference = default;
    [SerializeField] CursorItem CursorItem = default;
    [SerializeField] GridSystem GridSystem = default;
    [SerializeField] List<RectTransform> UIElements = default;

    public void Start()
    {
        foreach (StoreSection storeMenu in this.StoreMenus)
        {
            storeMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.GridSystem, this.PlayerBalance, this.PlayerBalanceDisplay);
            storeMenu.Initialize();
        }
        PodMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.GridSystem);
        PodMenu.Initialize();
        this.PlayerBalanceHUD.GetComponent<TopHUD>().SetupPlayerBalance(this.PlayerBalance);
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

    public void CloseMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.SetActive(false);
            currentMenu = null;
            this.StoreToggledOff();
        }
    }

    private void StoreToggledOn()
    {
        this.PlayerBalanceHUD.SetActive(true);
        this.GridSystem.UpdateAnimalCellGrid();
        this.GridSystem.HighlightHomeLocations();

        EventManager.Instance.InvokeEvent(EventType.StoreOpened, null);
    }

    private void StoreToggledOff()
    {
        this.PlayerBalanceHUD.SetActive(false);
        this.GridSystem.UnhighlightHomeLocations();

        EventManager.Instance.InvokeEvent(EventType.StoreClosed, null);
    }
}
