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
    #region Public Properties
    public NeedAvailabilityItem[] Items => items;
    #endregion

    #region Private Fields
    [SerializeField]
    [Tooltip("List of the items that are available")]
    private NeedAvailabilityItem[] items;
    #endregion

    #region Constructors
    public NeedAvailability(params NeedAvailabilityItem[] items)
    {
        this.items = items;
    }
    #endregion

    #region Public Methods
    public NeedAvailabilityItem[] FindAllWater()
    {
        return FindAll(item => item.IsDrinkingWater);
    }
    public NeedAvailabilityItem[] FindAll(Predicate<NeedAvailabilityItem> predicate)
    {
        return Array.FindAll(items, predicate);
    }
    /// <summary>
    /// Count the number of items available that have this item id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public float CountAvailable(ItemID id)
    {
        return CountAvailable(item => item.ID == id);
    }
    /// <summary>
    /// Count the number of items available that match the predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public float CountAvailable(Predicate<NeedAvailabilityItem> predicate)
    {
        NeedAvailabilityItem[] items = FindAll(predicate);
        return items.Sum(item => item.AmountAvailable);
    }
    #endregion
}
