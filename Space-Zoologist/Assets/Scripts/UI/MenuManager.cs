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
    [SerializeField] UICursorInput CursorItem = default;
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
            storeMenu?.SetupDependencies(this.CursorItem, this.UIElements, this.ResourceManager);
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
                EventManager.Instance.InvokeEvent(EventType.StoreToggled, true);
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
            GameManager.Instance.TryToPause("StoreMenu");
            EventManager.Instance.InvokeEvent(EventType.StoreToggled, true);
        }
        StoreCanvas.DOScale(0.8f, 0.5f);
        this.IsInStore = true;

        GameManager.Instance.m_tileDataController.StartDrafting();
        GameManager.Instance.m_tileDataController.SetGridOverlay(true);

        AudioManager.instance?.PlayOneShot(SFXType.BuildModeOpen);
    }

    private void CloseStore()
    {
        StoreCanvas.DOScale(0, 0.5f);
        this.IsInStore = false;
        EventManager.Instance.InvokeEvent(EventType.StoreToggled, false);

        GameManager.Instance.m_tileDataController.FinishDrafting();
        GameManager.Instance.m_tileDataController.SetGridOverlay(false);
        GameManager.Instance.TryToUnpause("StoreMenu");
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
        }

        EventManager.Instance.InvokeEvent(EventType.StoreToggled, false);
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
            ui.GetComponent<Button>().interactable = isActive;

            // If this ui element has children,
            // try to get the image in the first child and set it's color to disabled
            if (ui.transform.childCount > 0)
            {
                ui.transform.GetChild(0).GetComponent<Image>().color = isActive ? Color.white : Color.gray;
            }
        }

        // Commented out 10/07/2021 because dialogue system shouldn't close inspector
        // if (!isActive)
        // {
        //     GameManager.Instance.m_inspector.CloseInspector();
        //     GameManager.Instance.TurnObjectivePanelOn();
        //     GameManager.Instance.EnableInspectorToggle(false);

        // }
        // else
        // {
        //     GameManager.Instance.EnableInspectorToggle(true);
        // }
    }

    public void ToggleUISingleButton(string buttonName)
    {
        foreach(GameObject ui in UI)
        {
            if(ui.name == buttonName)
            {
                bool isActive = !ui.GetComponent<Button>().interactable;
                ui.GetComponent<Button>().interactable = isActive;

                // Make sure the UI element has a child with an image to change color for
                if (ui.transform.childCount >= 1)
                {
                    ui.transform.GetChild(0).GetComponent<Image>().color = isActive ? Color.white : Color.gray;
                }
                break;
            }
        }
    }
}
