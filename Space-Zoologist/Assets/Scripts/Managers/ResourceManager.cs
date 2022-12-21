using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    // [SerializeField] LevelDataReference LevelDataRef = default;
    Dictionary<ItemID, int> remainingResources = new Dictionary<ItemID, int>();

    // a copy of the dictionary before draft
    Dictionary<ItemID, int> initialResources = new Dictionary<ItemID, int>();

    // Events
    /// <summary>
    /// Params: ItemID, int: new quantity
    /// </summary>
    public System.Action<ItemID, int> OnRemainingResourcesChanged;

    public void Initialize()
    {
        // create entries for all the different item types 
        foreach(var item in ItemRegistry.GetAllItems())
        {
            remainingResources.TryAdd(item.ShopItem.ID, 0);
            initialResources.TryAdd(item.ShopItem.ID, 0);
        }
        // fill in the entries with starting values
        foreach (LevelData.ItemData item in GameManager.Instance.LevelData.ItemQuantities)
        {
            initialResources[item.itemObject.ID] = item.initialAmount;
            ChangeItemQuantity(item.itemObject.ID, item.initialAmount);
        }
    }

    public bool hasLimitedSupply(ItemID itemID)
    {
        return remainingResources.ContainsKey(itemID);
    }

    /// <summary>
    /// Adjust the quantity of an item by some value
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="changeAmount"></param>
    /// <returns></returns>
    public int ChangeItemQuantity(ItemID itemID, int changeAmount)
    {
        if (remainingResources.ContainsKey(itemID))
        {
            // make sure cant reduce below 0
            if(changeAmount < 0)
            {
                changeAmount = Mathf.Min(remainingResources[itemID], changeAmount);
            }
            SetItemQuantity(itemID, remainingResources[itemID] + changeAmount);
            return changeAmount;
        }
        else
        {
            Debug.LogError("ResourceManager: " + itemID + " does not exist!");
            return -1;
        }
    }

    private void SetItemQuantity(ItemID itemID, int newAmount)
    {
        remainingResources[itemID] = newAmount;
        OnRemainingResourcesChanged?.Invoke(itemID, newAmount);
    }

    public int SellItem(Item item, int amount)
    {
        return ChangeItemQuantity(item.ID, -amount);
    }

    public void Placed(Item item, int amount)
    {
        ChangeItemQuantity(item.ID, -amount);
    }

    public void Placed(AnimalSpecies species, int amount)
    {
        ChangeItemQuantity(species.ID, -amount);
    }

    public int CheckRemainingResource(ItemID ID)
    {
        if (remainingResources.ContainsKey(ID))
        {
            return remainingResources[ID];
        }
        else
        {
            // Debug.Log("ResourceManager: " + item.ID + " does not exist!");
            return -1;
        }
    }

    public int CheckRemainingResource(AnimalSpecies species)
    {
        return CheckRemainingResource(species.ID);
    }
    public int CheckRemainingResource(Item item)
    {
        return CheckRemainingResource(item.ID);
    }

    public void Save()
    {
        Copy(remainingResources, initialResources);
    }

    public void Load()
    {
        Copy(initialResources, remainingResources);
    }

    // Should not be too costly since # of keys is very small (n <= 30) and this won't be called many times
    private void Copy(Dictionary<ItemID, int> from, Dictionary<ItemID, int> to)
    {
        foreach (var pair in from)
        {
            if (to.ContainsKey(pair.Key))
            {
                to[pair.Key] = pair.Value;
            }
            else
            {
                to.Add(pair.Key, pair.Value);
            }
            OnRemainingResourcesChanged?.Invoke(pair.Key, pair.Value);
        }
    }
}