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
    private ItemRegistryData itemData = null;
    #endregion

    #region Data Access Methods
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
        else throw new ArgumentException($"{nameof(ItemRegistry)}: " +
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
    public static ItemID[] GetItemIDsWithCategory(Category category)
    {
        ItemID[] ids = new ItemID[CountItemsWithCategory(category)];

        // Load the 
        for (int i = 0; i < ids.Length; i++)
        {
            ids[i] = new ItemID(category, i);
        }

        return ids;
    }
    #endregion

    #region Find Methods
    public static ItemID FindHasName(string name)
    {
        return FindInternal(data => data.Name.HasName(name), name);
    }
    public static ItemID FindHasName(string name, StringComparison comparison)
    {
        return FindInternal(data => data.Name.HasName(name, comparison), name, comparison);
    }
    public static ItemID FindAnyNameContains(string name)
    {
        return FindInternal(data => data.Name.AnyNameContains(name), name);
    }
    public static ItemID FindSpecies(ScriptableObject species)
    {
        return FindInternal(data => data.Species == species, species);
    }
    public static ItemID FindShopItem(Item searchItem)
    {
        return FindInternal(data => data.ShopItem == searchItem, searchItem);
    }
    public static ItemID FindTile(TileType tile)
    {
        return FindInternal(data => data.Tile == tile, tile);
    }
    public static ItemID Find(ItemData search)
    {
        return FindInternal(data => data == search, search);
    }
    public static ItemID Find(Predicate<ItemData> predicate)
    {
        return FindInternal(predicate);
    }
    private static ItemID FindInternal(Predicate<ItemData> predicate, params object[] searchParameters)
    {
        ItemID id = Exists(predicate);

        if (ValidID(id)) return id;
        else
        {
            // Create a string to make a statement about the search parameters
            string searchParametersStatement = string.Empty;

            // If some search parameters were specified then 
            // state them in the string
            if (searchParameters != null && searchParameters.Length > 0)
            {
                searchParametersStatement = "Search parameters: ";

                // Add a string for each argument
                for (int i = 0; i < searchParameters.Length; i++)
                {
                    searchParametersStatement += $"\n\tParameter {i + 1}: {searchParameters[i]}";
                }
            }

            // If the search did not find the data then return invalid id
            throw new ItemNotFoundException("No item could be found " +
                $"that matched the given predicate. {searchParametersStatement}");
        }
    }
    #endregion

    #region Exists Methods
    public static ItemID Exists(ItemData search)
    {
        return Exists(data => data == search);
    }
    public static ItemID Exists(Predicate<ItemData> predicate)
    {
        ItemDataList[] dataLists = Instance.itemData.itemDataLists;

        for (int category = 0; category < dataLists.Length; category++)
        {
            // Get the list of item data in this category
            ItemData[] datas = dataLists[category].Items;

            for (int index = 0; index < datas.Length; index++)
            {
                ItemData data = datas[index];

                // If this shop item is the same as the search item,
                // then return the id of this item
                if (predicate.Invoke(data))
                {
                    return new ItemID((Category)category, index);
                }
            }
        }

        // Return the invalid ID
        return ItemID.Invalid;
    }
    public static ItemID[] ExistsAll(Predicate<ItemData> predicate)
    {
        ItemDataList[] dataLists = Instance.itemData.itemDataLists;
        List<ItemID> ids = new List<ItemID>();

        for (int category = 0; category < dataLists.Length; category++)
        {
            // Get the list of item data in this category
            ItemData[] datas = dataLists[category].Items;

            for (int index = 0; index < datas.Length; index++)
            {
                ItemData data = datas[index];

                // If this data matches then add it to the list
                if (predicate.Invoke(data))
                {
                    ids.Add(new ItemID((Category)category, index));
                }
            }
        }

        // Return the invalid ID
        return ids.ToArray();
    }
    #endregion
}
