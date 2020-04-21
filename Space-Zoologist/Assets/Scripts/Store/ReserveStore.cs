using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReserveStore : MonoBehaviour, ISelectable
{
    [SerializeField] private GameObject StoreItemSmallDisplay = default;
    [SerializeField] private GameObject StoreItemExpandedDisplay = default;
    [Expandable] public List<StoreItemSO> StoreItemReferences = default;
    [Tooltip("Under Store Scroll View GameObject")]
    [SerializeField] GameObject StoreContent = default;
    [SerializeField] GameObject StoreContentExpanded = default;
    private List<GameObject> AvailableItems = new List<GameObject>();
    // Use to setup HUD events and player interaction events
    [Header("Add methods that should occur when player selects item (e.g., BuyItem from PlayerController)")]
    public ItemSelectedEvent ItemSelectedEvent = new ItemSelectedEvent();

    public void Start()
    {
        this.ItemSelectedEvent.AddListener(OnItemSelectedEvent);
        foreach (StoreItemSO storeItem in this.StoreItemReferences)
        {    
            AddItem(storeItem);
        }
    }

    public void SetupDisplay(string storeSection)
    {
        foreach (GameObject storeItem in this.AvailableItems)
        {
            if (storeItem.GetComponent<StoreItem>().ItemInformation.StoreItemCategory.Equals(storeSection))
            {
                storeItem.SetActive(true);
            }
            else 
            {
                storeItem.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Creates a new store item based off of the store item prefab and sets up SelectableItem component
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(StoreItemSO itemInformation)
    {
        GameObject newItem = Instantiate(this.StoreItemSmallDisplay, this.StoreContent.transform);
        newItem.GetComponent<StoreItem>().InitializeStoreItem(itemInformation);
        GameObject extendedDisplay = newItem.GetComponent<StoreItem>().SetupStoreItemExtendedDisplay(this.StoreItemExpandedDisplay, this.StoreContentExpanded.transform);
        newItem.GetComponent<OnMouseHover>().InitializeDisplay(extendedDisplay);
        this.SetupHandler(newItem.GetComponent<SelectableItem>());
        newItem.SetActive(false);
        extendedDisplay.SetActive(false);
        this.AvailableItems.Add(newItem);
    }

    public void SetupHandler(SelectableItem selectableItem)
    {
        selectableItem.SetupItemSelectedHandler(this.ItemSelectedEvent);
    }

    // Use to setup additional functionality on item selected if needed
    public void OnItemSelectedEvent(GameObject itemSelected)
    {
       
    }

    /// <summary>
    /// Updates playerFunds through reference if purchase is successful, otherwise returns false
    /// </summary>
    /// <param name="playerFunds"></param>
    /// <param name="itemToBuy"></param>
    /// <param name="numCopies"></param>
    /// <returns></returns>
    /// TODO: determine if itemToBuy can be changed to a storeItemSO
    public bool BuyItem(ref float playerFunds, GameObject itemToBuy, int numCopies)
    {
        StoreItemSO storeItemSO = itemToBuy.GetComponent<StoreItem>().ItemInformation;
        bool isPurchaseSuccessful = false;
        if (storeItemSO != null)
        {
            float totalCost = storeItemSO.ItemCost * numCopies;
            // Check if the player has enough funds
            if (totalCost <= playerFunds)
            {
                playerFunds -= totalCost;
                Debug.Log("Item purchased");
                isPurchaseSuccessful = true;
            }
            else
            {
                Debug.Log("Insufficient funds");
                isPurchaseSuccessful = false;
            }
        }
        return isPurchaseSuccessful;
    }
}
