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
    [SerializeField] private GameObject JournalDisplay = default;
    private GameObject JournalDisplayButton = default;
    private GameObject CloseDisplayButton = default;

    public void Start()
    {
        this.CloseDisplayButton = this.gameObject.transform.GetChild(0).gameObject;
        this.InventoryDisplayButton = this.gameObject.transform.GetChild(1).gameObject;
        this.StoreDisplayButton = this.gameObject.transform.GetChild(2).gameObject;
        this.JournalDisplayButton = this.gameObject.transform.GetChild(3).gameObject;
    }

    public void DisplayStore()
    {
        this.StoreDisplay.SetActive(true);
        this.OpenNewDisplay();
        this.CloseInventoryDisplay();
        this.CloseJournalDisplay();
    }

    public void DisplayInventory()
    {
        this.InventoryDisplay.SetActive(true);
        this.OpenNewDisplay();
        this.CloseStoreDisplay();
        this.CloseJournalDisplay();
    }

    public void DisplayJournal()
    {
        this.JournalDisplay.SetActive(true);
        this.OpenNewDisplay();
        this.CloseStoreDisplay();
        this.CloseInventoryDisplay();
    }

    public void OpenNewDisplay()
    {
        this.InventoryDisplayButton.SetActive(false);
        this.StoreDisplayButton.SetActive(false);
        this.JournalDisplayButton.SetActive(false);
        this.CloseDisplayButton.SetActive(true);
    }

    public void CloseDisplays()
    {
        this.CloseStoreDisplay();
        this.CloseInventoryDisplay();
        this.CloseJournalDisplay();
        this.InventoryDisplayButton.SetActive(true);
        this.StoreDisplayButton.SetActive(true);
        this.JournalDisplayButton.SetActive(true);
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

    public void CloseJournalDisplay()
    {
        this.JournalDisplay.SetActive(false);
    }
}
