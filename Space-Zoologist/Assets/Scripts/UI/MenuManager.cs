using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

// This enum now exists in ResearchCategoryType
// public enum MenuType { Food, Tiles, Animals };
public class MenuManager : MonoBehaviour
{
    [System.Serializable] public class BoolEvent : UnityEvent<bool> { }

    readonly string[] menuNames = { "Food", "Tiles", "Animals" };
    GameObject currentMenu = null;
    [SerializeField] GameObject PlayerBalanceHUD = default;
    [SerializeField] List<StoreSection> StoreMenus = default;
    [SerializeField] ResourceManager ResourceManager = default;
    [Header("Shared menu dependencies")]
    [SerializeField] CanvasObjectStrobe PlayerBalanceDisplay = default;
    [SerializeField] CursorItem CursorItem = default;
    [SerializeField] List<RectTransform> UIElements = default;
    [SerializeField] RectTransform StoreCanvas = default;
    [SerializeField] List<GameObject> UI = default;
    [Header("Events")]
    [SerializeField] BoolEvent onStoreToggled = default;
    public BoolEvent OnStoreToggled => onStoreToggled;
    public bool IsInStore { get; private set; }
    private int curMenu = 0;


    public void Start()
    {
        this.IsInStore = false;
        foreach (StoreSection storeMenu in this.StoreMenus)
        {
            storeMenu?.SetupDependencies(this.CursorItem, this.UIElements, this.PlayerBalanceDisplay, this.ResourceManager);
            storeMenu?.Initialize();
        }
        StoreMenus[curMenu]?.gameObject.SetActive(true);
        StoreCanvas.localScale = Vector3.zero;
    }

    public void OnToggleMenu(GameObject menu)
    {
        if (currentMenu != menu)
        {
            if (!this.IsInStore)
            {
                GameManager.Instance.TryToPause();
                EventManager.Instance.InvokeEvent(EventType.StoreOpened, null);
            }
            this.StoreMenuToggledOn(menu);
        }
        else
        {
            this.StoreMenuToggledOff(menu);
        }
    }

    public void ToggleStore()
    {
        SetStoreIsOn(!IsInStore);
    }

    public void SetStoreIsOn(bool isOn)
    {
        if (isOn == this.IsInStore)
        {
            return;
        }

        if (isOn) 
            OpenStore();
        else 
            CloseStore();

        onStoreToggled.Invoke(isOn);
    }

    private void OpenStore()
    {
        if (!this.IsInStore)
        {
            GameManager.Instance.TryToPause();
            EventManager.Instance.InvokeEvent(EventType.StoreOpened, null);
        }
        StoreCanvas.DOScale(0.8f, 0.5f);
        this.IsInStore = true;

        GameManager.Instance.m_gridSystem.StartDrafting();
        GameManager.Instance.m_gridSystem.SetGridOverlay(true);

        AudioManager.instance?.PlayOneShot(SFXType.BuildModeOpen);
    }

    private void CloseStore()
    {
        StoreCanvas.DOScale(0, 0.5f);
        this.IsInStore = false;
        EventManager.Instance.InvokeEvent(EventType.StoreClosed, null);

        GameManager.Instance.m_gridSystem.FinishDrafting();
        GameManager.Instance.m_gridSystem.SetGridOverlay(false);

        AudioManager.instance?.PlayOneShot(SFXType.BuildModeClose);
    }


    private void StoreMenuToggledOn(GameObject menu)
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

    private void StoreMenuToggledOff(GameObject menu)
    {
        if (menu != null)
        {
            menu.SetActive(false);
            this.currentMenu = null;
            ///this.PlayerBalanceHUD.SetActive(false);
            this.IsInStore = false;
            GameManager.Instance.TryToUnpause();
        }

        EventManager.Instance.InvokeEvent(EventType.StoreClosed, null);
    }

    public void OpenMenu(int menu) {

        StoreMenus[curMenu]?.gameObject.SetActive(false);

        curMenu = menu;

        StoreMenus[curMenu]?.gameObject.SetActive(true);

        AudioManager.instance.PlayOneShot(SFXType.TabSwitch);
    }

    // Currently this function is only called by the dialogue system
    public void ToggleUI(bool isActive)
    {
        foreach(GameObject ui in UI)
        {
            ui.SetActive(isActive);
        }

        // Commented out 10/07/2021 because dialogue system shouldn't close inspector
        if (!isActive)
        {
            GameManager.Instance.m_inspector.CloseInspector();
            GameManager.Instance.TurnObjectivePanelOn();
            GameManager.Instance.EnableInspectorToggle(false);

        }
        else
        {
            GameManager.Instance.EnableInspectorToggle(true);
        }
    }
}
