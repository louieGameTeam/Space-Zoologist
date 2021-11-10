using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] MenuManager MenuManager = default;
    //[SerializeField] Inspector Inspector = default;
    //[SerializeField] LogSystem LogSystem = default;
    //[SerializeField] ObjectiveManager ObjectiveManager = default;
    //[SerializeField] DialogueManager DialogueManager = default;
    [SerializeField] GameObject HUD = default;
    //[Tooltip("Binds to numbers in order of list (Element 0 -> 1, Element 1 -> 2, etc.")]
    //[SerializeField] List<GameObject> StoreMenus = default;
    //[SerializeField] List<GameObject> MachineHUDs = default;
    private Dictionary<KeyCode, GameObject> StoreBindings = new Dictionary<KeyCode, GameObject>();
    public bool CanUseIngameControls = true;
    private bool GameOver = false;
    public GameObject DebugMenu;

    private void Awake()
    {

    }

    //private void setupStoreBindings()
    //{
    //    for (int i = 0; i < this.StoreMenus.Count; i++)
    //    {
    //        switch (i)
    //        {
    //            case 0:
    //                this.StoreBindings.Add(KeyCode.Alpha1, StoreMenus[i]);
    //                break;
    //            case 1:
    //                this.StoreBindings.Add(KeyCode.Alpha2, StoreMenus[i]);
    //                break;
    //            case 2:
    //                this.StoreBindings.Add(KeyCode.Alpha3, StoreMenus[i]);
    //                break;
    //            case 3:
    //                this.StoreBindings.Add(KeyCode.Alpha4, StoreMenus[i]);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}

    private void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.GameOver, this.UpdateGameOver);
    }

    private void Update()
    {
        if (this.GameOver)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            DebugMenu?.SetActive(!DebugMenu.activeSelf);
        }
        // this.CanUseIngameControls = !this.OptionsMenu.gameObject.activeSelf;
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    if (this.MenuManager.IsInStore)
        //    {
        //        this.MenuManager.CloseStore();
        //    }
        //    else if (this.Inspector.IsInInspectorMode)
        //    {
        //        this.Inspector.CloseInspector();
        //    }
        //    else if (this.OptionsMenu.IsInOptionsMenu)
        //    {
        //        this.OptionsMenu.CloseOptionsMenu();
        //    }
        //    foreach(GameObject machineHUD in this.MachineHUDs)
        //    {
        //        if (machineHUD.activeSelf)
        //        {
        //            machineHUD.SetActive(false);
        //        }
        //    }
        //}
        //if (this.CanUseIngameControls)
        //{
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        this.Inspector.ToggleInspectMode();
        //    }
        //    if (Input.GetKeyDown(KeyCode.L))
        //    {
        //        this.LogSystem.ToggleLog();
        //    }
        //    if (Input.GetKeyDown(KeyCode.O))
        //    {
        //        this.ObjectiveManager.ToggleObjectivePanel();
        //    }
        //    if (Input.GetKeyDown(KeyCode.N))
        //    {
        //        print("Start Conversation");
        //        this.DialogueManager.StartInteractiveConversation();
        //    }
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        Debug.Log("Unpause");
        //        this.PauseManager.TogglePause();
        //    }
        //    foreach(KeyValuePair<KeyCode, GameObject> keyBinding in this.StoreBindings)
        //    {
        //        if (Input.GetKeyDown(keyBinding.Key))
        //        {
        //            this.MenuManager.OnToggleMenu(keyBinding.Value);
        //        }
        //    }
        //}

        // DESIGN UPDATE: the "O" key no longer disables the HUD
        // Actually... maybe this could still work if we make a proper distinction between 
        // "HUD" - heads up display meant to AUGMENT play (such as information about currently enclosure)
        // "UI" - interactive elements that are the FOCUS of play (such as the store and notebook)
        
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    this.HUD.SetActive(!this.HUD.activeSelf);
        //}
    }

    private void UpdateGameOver()
    {
        this.GameOver = true;
    }
}
