using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    [System.Serializable]
    public class ResourceData
    {
        public string resourceName;
        public int initialAmount;

        public ResourceData(string ID)
        {
            resourceName = ID;
        }
    }

    // [SerializeField] LevelDataReference LevelDataRef = default;
    [SerializeField] EventResponseManager EventResponseManager = default;
    [SerializeField] List<ResourceData> resourceData = default;
    Dictionary<string, int> remainingResources = new Dictionary<string, int>();

    // a copy of the dictionary before draft
    Dictionary<string, int> initialResources = new Dictionary<string, int>();
    private Dictionary<string, StoreItemCell> itemDisplayInfo = new Dictionary<string, StoreItemCell>();

    // Auto-generate the list of ResourceData for you if resourceData is empty
    //public LevelDataReference LevelDataRef = default;
    //public void OnValidate()
    //{
    //    if (LevelDataRef == null) return;
    //    if (resourceData == null || resourceData.Count > 0) return;

    //    resourceData = new List<ResourceData>();
    //    foreach (Item item in LevelDataRef.LevelData.Items)
    //    {
    //        resourceData.Add(new ResourceData(item.ID));
    //    }
    //    foreach (AnimalSpecies species in LevelDataRef.LevelData.AnimalSpecies)
    //    {
    //        resourceData.Add(new ResourceData(species.SpeciesName));
    //    }
    //}

    public void Awake()
    {
        foreach (ResourceData data in resourceData)
        {
            remainingResources.Add(data.resourceName, data.initialAmount);
            initialResources.Add(data.resourceName, data.initialAmount);
        }
    }

    public void Start()
    {
        EventResponseManager.InitializeResponseHandler(EventType.PopulationCountIncreased, AddItem);
    }

    public bool hasLimitedSupply(string itemName)
    {
        return remainingResources.ContainsKey(itemName);
    }

    public void setupItemSupplyTracker(StoreItemCell storeItem)
    {
        itemDisplayInfo.Add(storeItem.item.ItemName, storeItem);
        storeItem.RemainingAmount = remainingResources[storeItem.item.ItemName];
    }

    void AddItem(string itemName, int amount)
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
        PlacedItem(item.ID, amount);
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


    public void StartDraft() {
        remainingResources = new Dictionary<string, int>();
        Copy(initialResources, remainingResources);
    }

    public void CancelDraft() {
        // nothing needed to be done
    }

    public void ApplyDraft()
    {
        initialResources = new Dictionary<string, int>();
        Copy(remainingResources, initialResources);
    }

    
    private void Copy<TKeys, TValues>(Dictionary<TKeys, TValues> from, Dictionary<TKeys, TValues> to) {
        foreach (var pair in from)
        {
            to.Add(pair.Key, pair.Value);
        }
    }
}