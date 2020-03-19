using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDTesting : MonoBehaviour
{
    [SerializeField] private GameObject StoreDisplay = default;
    private GameObject StoreDisplayButton = default;
    [SerializeField] private GameObject InventoryDisplay = default;
    private GameObject InventoryDisplayButton = default;
    private GameObject CloseDisplayButton = default;

    public void Start()
    {
        this.CloseDisplayButton = this.gameObject.transform.GetChild(0).gameObject;
        this.InventoryDisplayButton = this.gameObject.transform.GetChild(1).gameObject;
        this.StoreDisplayButton = this.gameObject.transform.GetChild(2).gameObject;
    }

    public void DisplayStore()
    {
        this.StoreDisplay.SetActive(true);
        this.InventoryDisplayButton.SetActive(false);
        this.StoreDisplayButton.SetActive(false);
        this.CloseDisplayButton.SetActive(true);
        this.CloseInventoryDisplay();
    }

    public void DisplayInventory()
    {
        this.InventoryDisplay.SetActive(true);
        this.InventoryDisplayButton.SetActive(false);
        this.StoreDisplayButton.SetActive(false);
        this.CloseDisplayButton.SetActive(true);
        this.CloseStoreDisplay();
    }

    public void CloseDisplays()
    {
        this.CloseStoreDisplay();
        this.CloseInventoryDisplay();
        this.InventoryDisplayButton.SetActive(true);
        this.StoreDisplayButton.SetActive(true);
        this.CloseDisplayButton.SetActive(false);
    }

    public void CloseStoreDisplay()
    {
        this.StoreDisplay.SetActive(false);
    }

    public void CloseInventoryDisplay()
    {
        this.InventoryDisplay.SetActive(false);
    }
}
