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

    #region Private Properties
    private static ItemRegistry Instance => GetOrCreateInstance(nameof(ItemRegistry), nameof(ItemRegistry));
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of item data lists - parallel to the 'Category' enum")]
    [EditArrayWrapperOnEnum("itemDataLists", typeof(Category))]
    private ItemRegistryData itemData;
    #endregion

    #region Public Methods
    public static ItemData Get(ItemID id)
    {
        ItemData[] datas = GetItemsWithCategory(id.Category);
        if (id.Index >= 0 && id.Index < datas.Length) return datas[id.Index];
        else return null;
    }
    public static ItemData[] GetItemsWithCategory(Category category) => Instance.itemData.itemDataLists[(int)category].Items;
    public static int CountItemsWithCategory(Category category) => GetItemsWithCategory(category).Length;
    public static int CountAllItems()
    {
        int count = 0;
        Category[] categories = (Category[])System.Enum.GetValues(typeof(Category));

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
        Category[] categories = (Category[])System.Enum.GetValues(typeof(Category));

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
    #endregion
}
