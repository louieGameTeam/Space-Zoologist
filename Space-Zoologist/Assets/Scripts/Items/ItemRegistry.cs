using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemRegistry : ScriptableObjectSingleton<ItemRegistry>
{
    #region Public Typedefs
    public enum Category { Species, Food, Tile }
    #endregion

    #region Private Properties
    private static ItemRegistry Instance => GetOrCreateInstance(nameof(ItemRegistry), nameof(ItemRegistry));
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of item data lists - parallel to the 'Category' enum")]
    private ItemDataList[] itemDatas;
    #endregion

    #region Public Methods
    public static ItemData Get(ItemID id)
    {
        ItemData[] datas = GetItemsWithCategory(id.Category);
        if (id.Index >= 0 && id.Index < datas.Length) return datas[id.Index];
        else return null;
    }
    public static ItemData[] GetItemsWithCategory(Category category) => Instance.itemDatas[(int)category].List;
    #endregion
}
