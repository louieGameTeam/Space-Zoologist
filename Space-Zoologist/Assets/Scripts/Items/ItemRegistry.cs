using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemRegistry : ScriptableObjectSingleton<ItemRegistry>
{
    #region Public Typedefs
    public enum Category { Species, Food, Tile }
    // So that the attributes work correctly in the editor
    [System.Serializable]
    public class ItemRegistryData
    {
        [Tooltip("List of item data lists - parallel to the 'Category' enum")]
        [WrappedProperty("items")]
        public ItemDataList[] itemDataLists;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of item data lists - parallel to the 'Category' enum")]
    [EditArrayWrapperOnEnum("itemDataLists", typeof(Category))]
    private ItemRegistryData itemData;
    #endregion

    #region Public Methods
    public static bool ValidID(ItemID id)
    {
        ItemData[] datas = GetItemsWithCategory(id.Category);
        return id.Index >= 0 && id.Index < datas.Length;
    }
    public static ItemData Get(ItemID id)
    {
        ItemData[] datas = GetItemsWithCategory(id.Category);
        if (ValidID(id)) return datas[id.Index];
        else throw new IndexOutOfRangeException($"{nameof(ItemRegistry)}: " +
            $"No item exists at index {id.Index} for category {id.Category}. " +
            $"Total items in category: {datas.Length}");
    }
    public static ItemData[] GetItemsWithCategory(Category category) => Instance.itemData.itemDataLists[(int)category].Items;
    public static ItemData[] GetItemsWithCategoryName(string categoryName)
    {
        if (Enum.TryParse(categoryName, true, out Category category))
        {
            return GetItemsWithCategory(category);
        }
        else throw new System.ArgumentException($"{nameof(ItemRegistry)}: " +
            $"attempted to get items with category '{categoryName}', " +
            $"but no such category exists");
    }
    public static int CountItemsWithCategory(Category category) => GetItemsWithCategory(category).Length;
    public static int CountAllItems()
    {
        int count = 0;
        Category[] categories = (Category[])Enum.GetValues(typeof(Category));

        // Add the lengths of each array to the total count
        foreach(Category category in categories)
        {
            count += CountItemsWithCategory(category);
        }

        return count;
    }
    public static ItemID[] GetAllItemIDs()
    {
        // Create an array as big as all the items in the registry
        ItemID[] ids = new ItemID[CountAllItems()];
        int index = 0;

        // Get a list of categories
        Category[] categories = (Category[])Enum.GetValues(typeof(Category));

        // Loop through all categories
        foreach(Category category in categories)
        {
            // Add an id with each index for each item in this category
            for(int i = 0; i < CountItemsWithCategory(category); i++, index++)
            {
                ids[index] = new ItemID(category, i);
            }
        }

        return ids;
    }
    public static ItemID SpeciesID(ScriptableObject species)
    {
        // Get all species
        ItemData[] speciesList = GetItemsWithCategory(Category.Species);

        // Find an item data with the same species
        int index = Array.FindIndex(speciesList, item => item.Species == species);

        // If an item was found then return its id
        if (index >= 0) return new ItemID(Category.Species, index);
        // If no item was found then return an invalid id
        else throw new ArgumentException($"{nameof(ItemRegistry)}: " +
            $"species {species} does not exist in the item registry");
    }
    public static ItemID ShopItemID(Item searchItem)
    {
        ItemDataList[] dataLists = Instance.itemData.itemDataLists;

        for(int category = 0; category < dataLists.Length; category++)
        {
            // Get the list of item data in this category
            ItemData[] datas = dataLists[category].Items;

            for (int index = 0; index < datas.Length; index++)
            {
                Item shopItem = datas[index].ShopItem;

                // If this shop item is the same as the search item,
                // then return the id of this item
                if (shopItem == searchItem)
                {
                    return new ItemID((Category)category, index);
                }
            }
        }

        // If the search did not find the shop item
        // then throw an exception
        throw new ArgumentException($"{nameof(ItemRegistry)}: " +
            $"shop item {searchItem} does not exist in the item registry");
    }
    #endregion
}
