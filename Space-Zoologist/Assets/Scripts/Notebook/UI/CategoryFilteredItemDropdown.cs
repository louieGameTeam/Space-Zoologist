using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class CategoryFilteredItemDropdown : ItemDropdown
{
    #region Public Properties
    public List<ItemRegistry.Category> CategoryFilter => categoryFilter;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Research category type that this dropdown represents")]
    protected List<ItemRegistry.Category> categoryFilter;
    #endregion

    #region Public Methods
    public void Setup(params ItemRegistry.Category[] categoryFilter)
    {
        this.categoryFilter = new List<ItemRegistry.Category>(categoryFilter);
        
        // Now that type filter is set we will setup the base class
        base.Setup();
    }
    #endregion

    #region Private/Protected Methods
    protected override ItemID[] GetItemIDs()
    {
        return ItemRegistry.GetAllItemIDs()
            .Where(id => categoryFilter.Contains(id.Category) && UIParent.Notebook.ItemIsUnlocked(id))
            .ToArray();
    }
    #endregion
}
