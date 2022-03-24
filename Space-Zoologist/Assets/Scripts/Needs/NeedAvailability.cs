using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registers the availability 
/// of different needed items
/// </summary>
/// <remarks>
/// Food needs: the object will hold food availability
/// even for species that do not eat that food.  The NeedRatingFactory
/// should check if the food is edible to rate the food need of animals.
/// 
/// Terrain needs: the need availability will have an item
/// for every individual tile that the animal can occupy. 
/// For contested terrain this results in multiple availability items 
/// that have decimal amounts available. For example, when cows and 
/// goats compete for a land tile, the cow might get 0.4 of that tile
/// and the goat might get 0.6 of that tile.
/// 
/// Water needs: water needs are identified with the fresh water id.
/// The metadata of the item should hold an object of type "LiquidBodyContent"
/// so that the content of the water tile can be read.
/// </remarks>
[Serializable]
public class NeedAvailability
{
    #region Public Typedefs
    /// <summary>
    /// Compares two need availability items id and metadata, ignoring amount available
    /// </summary>
    public class ItemComparer : IEqualityComparer<NeedAvailabilityItem>
    {
        public bool Equals(NeedAvailabilityItem a, NeedAvailabilityItem b)
        {
            // Check if both are not null
            if (a != null && b != null)
            {
                // Equal if they have equal id and metadata
                return a.ID == b.ID && Equals(a.MetaData, b.MetaData);
            }
            // If both are null then they are equal
            else if (a == null && b == null) return true;
            // If one is null and the other is not null they are unequal
            else return false;
        }
        public int GetHashCode(NeedAvailabilityItem item)
        {
            int hash = item.ID.GetHashCode();
            if (item.MetaData != null) hash ^= item.MetaData.GetHashCode();
            return hash;
        }
    }
    #endregion

    #region Public Properties
    public NeedAvailabilityItem[] Items => items;
    #endregion

    #region Private Fields
    [SerializeField]
    [Tooltip("List of the items that are available")]
    private NeedAvailabilityItem[] items;
    #endregion

    #region Constructors
    public NeedAvailability(params NeedAvailabilityItem[] oldItems)
    {
        // Create the list and the comparer
        List<NeedAvailabilityItem> newItems = new List<NeedAvailabilityItem>();
        ItemComparer comparer = new ItemComparer();

        // Add each of the initial items to the list
        foreach (NeedAvailabilityItem oldItem in oldItems)
        {
            int index = newItems.FindIndex(newItem => comparer.Equals(newItem, oldItem));

            // Check if that item already exists
            if (index >= 0)
            {
                // Collapse the old and new item into one
                NeedAvailabilityItem newItem = newItems[index];
                newItems[index] = new NeedAvailabilityItem(
                    newItem.ID,
                    newItem.ItemCount + oldItem.ItemCount,
                    newItem.AmountAvailable + oldItem.AmountAvailable,
                    newItem.MetaData);
            }
            // If the item does not exist yet then add it to the list
            else newItems.Add(oldItem);
        }

        items = newItems.ToArray();
    }
    #endregion

    #region Public Methods
    public NeedAvailabilityItem[] FindPredatorItems()
    {
        return FindAll(item => item.ID.Category == ItemRegistry.Category.Species);
    }
    public NeedAvailabilityItem[] FindWaterItems()
    {
        return FindAll(item => item.IsDrinkingWater);
    }
    public NeedAvailabilityItem[] FindFoodItems()
    {
        return FindAll(item => item.ID.Category == ItemRegistry.Category.Food);
    }
    public NeedAvailabilityItem[] FindTreeItems()
    {
        return FindAll(item => item.ID.Category == ItemRegistry.Category.Food);
    }
    public NeedAvailabilityItem[] FindTerrainItems()
    {
        bool IsTileAndNotWater(NeedAvailabilityItem item)
        {
            return item.ID.Category == ItemRegistry.Category.Tile &&
                !item.IsDrinkingWater;
        }
        return FindAll(IsTileAndNotWater);
    }
    
    public NeedAvailabilityItem FindWithItem(ItemID id)
    {
        return Find(item => item.ID == id);
    }
    public NeedAvailabilityItem Find(Predicate<NeedAvailabilityItem> predicate)
    {
        return Array.Find(items, predicate);
    }

    public NeedAvailabilityItem[] FindAllWithItem(ItemID id)
    {
        return FindAll(item => item.ID == id);
    }
    public NeedAvailabilityItem[] FindAll(Predicate<NeedAvailabilityItem> predicate)
    {
        return Array.FindAll(items, predicate);
    }
    #endregion
}
