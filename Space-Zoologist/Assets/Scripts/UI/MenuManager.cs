using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    GameObject currentMenu = null;
    [SerializeField] GameObject PlayerBalanceHUD = default;
    [SerializeField] List<GameObject> StoreMenuImages = default;
    [SerializeField] List<StoreSection> StoreMenus = default;
    [SerializeField] ResourceManager ResourceManager = default;
    [SerializeField] PauseManager PauseManager = default;
    [Header("Shared menu dependencies")]
    [SerializeField] PlayerBalance PlayerBalance = default;
    [SerializeField] CanvasObjectStrobe PlayerBalanceDisplay = default;
    [SerializeField] LevelDataReference LevelDataReference = default;
    [SerializeField] CursorItem CursorItem = default;
    [SerializeField] GridSystem GridSystem = default;
    [SerializeField] List<RectTransform> UIElements = default;
    [SerializeField] RectTransform StoreCanvas = default;
    public bool IsInStore { get; private set; }
    private int curMenu = 0;


    public void Start()
    {
        this.IsInStore = false;
        foreach (StoreSection storeMenu in this.StoreMenus)
        {
            storeMenu.SetupDependencies(this.LevelDataReference, this.CursorItem, this.UIElements, this.GridSystem, this.PlayerBalance, this.PlayerBalanceDisplay, this.ResourceManager);
            storeMenu.Initialize();
        }
        this.PlayerBalanceHUD.GetComponent<TopHUD>().SetupPlayerBalance(this.PlayerBalance);
        StoreMenus[curMenu].gameObject.SetActive(true);
        StoreMenuImages[curMenu].SetActive(true);

    }

    public void OnToggleMenu(GameObject menu)
    {
        if (currentMenu != menu)
        {
            if (!this.IsInStore)
            {
                this.PauseManager.TryToPause();
                EventManager.Instance.InvokeEvent(EventType.StoreOpened, null);
            }
            this.StoreToggledOn(menu);
        }
        else
        {
            this.StoreToggledOff(menu);
        }
    }

    public void ToggleStore()
    {
        if (!this.IsInStore)
        {
            OpenStore();
        }
        else
        {
            CloseStore();
        }
    }


    public void OpenStore()
    {
        if (!this.IsInStore)
        {
            this.PauseManager.TryToPause();
            EventManager.Instance.InvokeEvent(EventType.StoreOpened, null);
        }
        StoreCanvas.DOAnchorPosX(StoreCanvas.anchoredPosition.x + StoreCanvas.rect.width / 2.5f, 0.5f);
        this.IsInStore = true;
    }

    public void CloseStore()
    {
        StoreCanvas.DOAnchorPosX(StoreCanvas.anchoredPosition.x - StoreCanvas.rect.width / 2.5f, 0.5f);
        this.IsInStore = false;
        EventManager.Instance.InvokeEvent(EventType.StoreClosed, null);
    }


    private void StoreToggledOn(GameObject menu)
    {
        if (this.currentMenu)
        {
            this.currentMenu.SetActive(false);
        }
        menu.SetActive(true);
        currentMenu = menu;
        //this.PlayerBalanceHUD.SetActive(true);
        this.IsInStore = true;
    }

    private void StoreToggledOff(GameObject menu)
    {
        if (menu != null)
        {
            menu.SetActive(false);
            this.currentMenu = null;
            ///this.PlayerBalanceHUD.SetActive(false);
            this.IsInStore = false;
            this.PauseManager.TryToUnpause();
        }

        EventManager.Instance.InvokeEvent(EventType.StoreClosed, null);
    }

    public void NextMenu()
    {
        StoreMenus[curMenu].gameObject.SetActive(false);
        StoreMenuImages[curMenu].SetActive(false);

        curMenu++;
        if (curMenu >= StoreMenus.Count) curMenu = 0;

        StoreMenus[curMenu].gameObject.SetActive(true);
        StoreMenuImages[curMenu].SetActive(true);
    }

    public void PrevMenu()
    {

        StoreMenus[curMenu].gameObject.SetActive(false);
        StoreMenuImages[curMenu].SetActive(false);

        curMenu--;
        if (curMenu < 0) curMenu = StoreMenus.Count - 1;

        StoreMenus[curMenu].gameObject.SetActive(true);
        StoreMenuImages[curMenu].SetActive(true);
    }
}
