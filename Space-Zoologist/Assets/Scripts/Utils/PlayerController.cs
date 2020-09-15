using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] MenuManager MenuManager = default;
    [SerializeField] Inspector Inspector = default;
    [SerializeField] LogSystem LogSystem = default;
    [SerializeField] ObjectiveManager ObjectiveManager = default;
    [SerializeField] List<GameObject> StoreMenus = default;
    [SerializeField] List<GameObject> MachineHUDs = default;
    private Dictionary<KeyCode, GameObject> StoreBindings = new Dictionary<KeyCode, GameObject>();

    private void Awake()
    {
        for (int i=0; i<this.StoreMenus.Count; i++)
        {
            switch(i)
            {
                case 0:
                    this.StoreBindings.Add(KeyCode.Alpha1, StoreMenus[i]);
                    break;
                case 1:
                    this.StoreBindings.Add(KeyCode.Alpha2, StoreMenus[i]);
                    break;
                case 2:
                    this.StoreBindings.Add(KeyCode.Alpha3, StoreMenus[i]);
                    break;
                case 3:
                    this.StoreBindings.Add(KeyCode.Alpha4, StoreMenus[i]);
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        foreach(KeyValuePair<KeyCode, GameObject> keyBinding in this.StoreBindings)
        {
            if (Input.GetKeyDown(keyBinding.Key))
            {
                this.MenuManager.OnToggleMenu(keyBinding.Value);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            this.MenuManager.CloseMenu();
            foreach(GameObject machineHUD in this.MachineHUDs)
            {
                if (machineHUD.activeSelf)
                {
                    machineHUD.SetActive(false);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            this.Inspector.ToggleInspectMode();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            this.LogSystem.ToggleLog();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            this.ObjectiveManager.ToggleObjectivePanel();
        }
    }
}
