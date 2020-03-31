using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Should this be directly tied to JournalSystem so entries can be updated?
// Or use an event system? 
public class PlayerInventory : MonoBehaviour, ISelectableItem
{
    [SerializeField] private GameObject SelectableItemPrefab = default;
    private List<GameObject> SelectableItems = new List<GameObject>();
    [SerializeField] private GameObject InventoryItemPopupPrefab = default;
    [SerializeField] private float playerFunds = default;
    public float PlayerFunds { get => playerFunds; set => playerFunds = value; }
    // [SerializeField] GameObject journal = default;
    // private JournalSystem PlayerJournal = default;
    private readonly ItemSelectedEvent OnItemSelectedEvent = new ItemSelectedEvent();
    private GameObject ItemSelected = default;

    public void Start()
    {
        this.OnItemSelectedEvent.AddListener(this.OnItemSelected);
        this.InventoryItemPopupPrefab.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(UseItem);
        this.InventoryItemPopupPrefab.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(SellItem);
        this.InventoryItemPopupPrefab.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(ClosePopup);
        // this.PlayerJournal = this.journal.GetComponent<JournalSystem>();
    }

    public void InitializeItem(ScriptableObject itemObtained)
    {
        GameObject newItem = Instantiate(this.SelectableItemPrefab, this.transform);
        SelectableItem selectableItem = newItem.GetComponent<SelectableItem>();
        selectableItem.Initialize(itemObtained, this.OnItemSelectedEvent);
        this.SelectableItems.Add(newItem);
        //if (itemObtained.Discovered)
        //{
        //    this.PlayerJournal.InitializeItem(itemObtained);
        //}
    }

    public void OnItemSelected(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        this.InventoryItemPopupPrefab.SetActive(true);
    }

    private void UseItem()
    {
        if (this.ItemSelected != null)
        {

        }
        ClosePopup();
    }

    private void SellItem()
    {
        if (this.ItemSelected != null)
        {
            SelectableItem itemToRemove = this.ItemSelected.GetComponent<SelectableItem>();
            float SomeModifier = 0.5f;
            this.playerFunds += itemToRemove.ItemInfo.ItemCost * SomeModifier;
            this.ItemSelected.SetActive(false);
            this.SelectableItems.Remove(this.ItemSelected);
        }
        ClosePopup();
    }

    private void ClosePopup()
    {
        this.ItemSelected = null;
        this.InventoryItemPopupPrefab.SetActive(false);
        TestDisplayFunds();
    }

    public void TestDisplayFunds()
    {
        GameObject.Find("PlayerFundsInInventory").GetComponent<Text>().text = "Funds: " + this.PlayerFunds;
    }
}
