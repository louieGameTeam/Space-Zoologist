using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReserveStore : MonoBehaviour, ISelectable
{
    [SerializeField] private GameObject StoreItemPrefab = default;
    [Expandable] public List<StoreItemSO> StoreItemReferences = default;
    [Tooltip("Under Store Scroll View GameObject")]
    [SerializeField] GameObject StoreContent = default;
    // Use to setup HUD events and player interaction events
    [Header("Add methods that should occur when player selects item (e.g., BuyItem from PlayerController)")]
    public ItemSelectedEvent ItemSelectedEvent = new ItemSelectedEvent();
    public UnityEvent SpriteStopFollowing = new UnityEvent();
    private bool StoreInitialized = false;

    public void Start()
    {
        this.ItemSelectedEvent.AddListener(OnItemSelectedEvent);
    }

    public void InitializeStore()
    {
        if (!this.StoreInitialized)
        {
            foreach (StoreItemSO storeItem in this.StoreItemReferences)
            {
                AddItem(storeItem);
            }
            this.StoreInitialized = true;
        }
    }

    /// <summary>
    /// Creates a new store item based off of the store item prefab and sets up SelectableItem component
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(StoreItemSO itemInformation)
    {
        GameObject newItem = Instantiate(this.StoreItemPrefab, this.StoreContent.transform);
        newItem.GetComponent<StoreItem>().InitializeStoreItem(itemInformation, this.StoreItemPrefab);
        this.SetupHandler(newItem.GetComponent<SelectableItem>());
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
        this.SpriteStopFollowing.Invoke();
        return isPurchaseSuccessful;
    }
}
