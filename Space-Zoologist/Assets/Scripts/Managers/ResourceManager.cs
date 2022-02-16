using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    // [SerializeField] LevelDataReference LevelDataRef = default;
    [SerializeField] EventResponseManager EventResponseManager = default;
    Dictionary<ItemID, int> remainingResources = new Dictionary<ItemID, int>();

    // a copy of the dictionary before draft
    Dictionary<ItemID, int> initialResources = new Dictionary<ItemID, int>();
    private Dictionary<ItemID, StoreItemCell> itemDisplayInfo = new Dictionary<ItemID, StoreItemCell>();

    public void Initialize()
    {
        foreach (LevelData.ItemData item in GameManager.Instance.LevelData.itemQuantities)
        {
            if (!remainingResources.ContainsKey(item.itemObject.ID))
            {
                remainingResources.Add(item.itemObject.ID, item.initialAmount);
                initialResources.Add(item.itemObject.ID, item.initialAmount);
            }
            remainingResources[item.itemObject.ID] = item.initialAmount;
            initialResources[item.itemObject.ID] = item.initialAmount;
        }

        EventResponseManager.InitializeResponseHandler(EventType.PopulationCountIncreased, AddItem);
    }

    public bool hasLimitedSupply(ItemID itemID)
    {
        return remainingResources.ContainsKey(itemID);
    }

    public void setupItemSupplyTracker(StoreItemCell storeItem)
    {
        if (!itemDisplayInfo.ContainsKey(storeItem.item.ID))
        {
            itemDisplayInfo.Add(storeItem.item.ID, storeItem);
            storeItem.RemainingAmount = remainingResources[storeItem.item.ID];
        }
    }

    public void AddItem(ItemID itemID, int amount)
    {
        if (remainingResources.ContainsKey(itemID))
        {
            // Debug.Log("Added " + amount + " to " + remainingResources[itemName] + " remaining " + itemName);
            remainingResources[itemID] += amount;
            updateItemDisplayInfo(itemID);
        }
        else
        {
            Debug.Log("ResourceManager: " + itemID + " does not exist!");
        }
    }

    public void Placed(Item item, int amount)
    {
        PlacedItem(item.ID, amount);
    }

    public void Placed(AnimalSpecies species, int amount)
    {
        PlacedItem(species.ID, amount);
    }

    void PlacedItem(ItemID itemID, int amount)
    {
        if (remainingResources.ContainsKey(itemID))
        {
            remainingResources[itemID] -= amount;
            updateItemDisplayInfo(itemID);
        }
        else
        {
            Debug.Log("ResourceManager: " + itemID + " does not exist!");
        }
    }

    private void updateItemDisplayInfo(ItemID itemID)
    {
        itemDisplayInfo[itemID].RemainingAmount = remainingResources[itemID];
    }

    public int CheckRemainingResource(Item item)
    {
        if (remainingResources.ContainsKey(item.ID))
        {
            return remainingResources[item.ID];
        }
        else
        {
            // Debug.Log("ResourceManager: " + item.ID + " does not exist!");
            return -1;
        }
    }

    public int CheckRemainingResource(AnimalSpecies species)
    {
        if (remainingResources.ContainsKey(species.ID))
        {
            return remainingResources[species.ID];
        }
        else
        {
            // Debug.Log("ResourceManager: " + species.SpeciesName + " does not exist!");
            return -1;
        }
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
            updateItemDisplayInfo(pair.Key);
        }
    }
}