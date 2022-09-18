using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    [Tooltip("Reference to the dropdown arrow")]
    protected Image dropdownArrow;
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
    protected override ItemID[] GetItemIDs(ItemID[] source)
    {
        return base.GetItemIDs(source)
            .Where(id => categoryFilter.Contains(id.Category) && UIParent.Data.ItemIsUnlocked(id))
            .ToArray();
    }
    private void FlipDropdownArrow()
    {
        Debug.Log("AAA");
        dropdownArrow.transform.eulerAngles = new Vector3(dropdownArrow.transform.eulerAngles.x, dropdownArrow.transform.eulerAngles.y, dropdownArrow.transform.eulerAngles.z + 180);
    }
    #endregion
}
