using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject UsableItem = default;
    private List<GameObject> UsableItems = new List<GameObject>();
    [SerializeField] private GameObject InventoryItemPopup = default;
    [SerializeField] private float playerFunds = default;
    public float PlayerFunds { get => playerFunds; set => playerFunds = value; }
    private readonly ItemSelectedEvent OnItemSelected = new ItemSelectedEvent();
    private GameObject ItemSelected = default;

    public void Start()
    {
        OnItemSelected.AddListener(DisplayOptions);
    }

    public void AddItem(StoreItemSO itemPurchased)
    {
        GameObject newItem = Instantiate(this.UsableItem, this.transform);
        UsableItem usableItem = newItem.GetComponent<UsableItem>();
        usableItem.InitializeItem(itemPurchased, this.OnItemSelected);
        this.UsableItems.Add(newItem);
    }

    public void DisplayOptions(GameObject itemSelected)
    {
        this.ItemSelected = itemSelected;
        this.InventoryItemPopup.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(UseItem);
        this.InventoryItemPopup.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(SellItem);
        this.InventoryItemPopup.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(ClosePopup);
        this.InventoryItemPopup.SetActive(true);
    }

    public void UseItem()
    {
        ClosePopup();
    }

    public void SellItem()
    {
        if (this.ItemSelected != null)
        {
            UsableItem itemToRemove = this.ItemSelected.GetComponent<UsableItem>();
            float something = 0.5f;
            this.playerFunds += itemToRemove.itemInfo.ItemCost * something;
            this.ItemSelected.SetActive(false);
            this.UsableItems.Remove(this.ItemSelected);
            TestDisplayFunds();
            ClosePopup();
        }
    }

    public void ClosePopup()
    {
        this.ItemSelected = null;
        this.InventoryItemPopup.SetActive(false);
    }

    public void TestDisplayFunds()
    {
        GameObject.Find("PlayerFunds").GetComponent<Text>().text = "PlayerFunds: " + this.PlayerFunds;
    }
}
