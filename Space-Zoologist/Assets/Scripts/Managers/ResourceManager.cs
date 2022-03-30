using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    // [SerializeField] LevelDataReference LevelDataRef = default;
    Dictionary<string, int> remainingResources = new Dictionary<string, int>();

    // a copy of the dictionary before draft
    Dictionary<string, int> initialResources = new Dictionary<string, int>();
    private Dictionary<string, StoreItemCell> itemDisplayInfo = new Dictionary<string, StoreItemCell>();

    public void Initialize()
    {
        foreach (LevelData.ItemData item in GameManager.Instance.LevelData.itemQuantities)
        {
            if (!remainingResources.ContainsKey(item.itemObject.ItemName))
            {
                remainingResources.Add(item.itemObject.ItemName, item.initialAmount);
                initialResources.Add(item.itemObject.ItemName, item.initialAmount);
            }
            remainingResources[item.itemObject.ItemName] = item.initialAmount;
            initialResources[item.itemObject.ItemName] = item.initialAmount;
        }
    }

    public bool hasLimitedSupply(string itemName)
    {
        return remainingResources.ContainsKey(itemName);
    }

    public void setupItemSupplyTracker(StoreItemCell storeItem)
    {
        if (!itemDisplayInfo.ContainsKey(storeItem.item.ItemName))
        {
            itemDisplayInfo.Add(storeItem.item.ItemName, storeItem);
            storeItem.RemainingAmount = remainingResources[storeItem.item.ItemName];
        }
    }

    public void AddItem(string itemName, int amount)
    {
        if (remainingResources.ContainsKey(itemName))
        {
            // Debug.Log("Added " + amount + " to " + remainingResources[itemName] + " remaining " + itemName);
            remainingResources[itemName] += amount;
            updateItemDisplayInfo(itemName);
        }
        else
        {
            Debug.Log("ResourceManager: " + itemName + " does not exist!");
        }
    }

    public void Placed(Item item, int amount)
    {
        PlacedItem(item.ItemName, amount);
    }

    public void Placed(AnimalSpecies species, int amount)
    {
        PlacedItem(species.SpeciesName, amount);
    }

    void PlacedItem(string itemName, int amount)
    {
        if (remainingResources.ContainsKey(itemName))
        {
            remainingResources[itemName] -= amount;
            updateItemDisplayInfo(itemName);
        }
        else
        {
            Debug.Log("ResourceManager: " + itemName + " does not exist!");
        }
    }

    private void updateItemDisplayInfo(string itemName)
    {
        itemDisplayInfo[itemName].RemainingAmount = remainingResources[itemName];
    }

    public int CheckRemainingResource(Item item)
    {
        if (remainingResources.ContainsKey(item.ItemName))
        {
            return remainingResources[item.ItemName];
        }
        else
        {
            // Debug.Log("ResourceManager: " + item.ID + " does not exist!");
            return -1;
        }
    }

    public int CheckRemainingResource(AnimalSpecies species)
    {
        if (remainingResources.ContainsKey(species.SpeciesName))
        {
            return remainingResources[species.SpeciesName];
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
    private void Copy(Dictionary<string, int> from, Dictionary<string, int> to)
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