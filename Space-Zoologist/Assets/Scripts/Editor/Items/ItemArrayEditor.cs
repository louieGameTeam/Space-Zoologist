using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemArrayEditor : ArrayOnEnumEditor<ItemRegistry.Category>
{
    #region Private Fields
    private ParallelArrayEditor<ItemData> innerArrayEditor = new ParallelArrayEditor<ItemData>();
    #endregion

    #region Constructors
    public ItemArrayEditor(string innerArrayRelativePath)
    {
        arrayElementPropertyField = (r, s, g, e) =>
        {
            ItemData[] datas = ItemRegistry.GetItemsWithCategory(e);
            innerArrayEditor.OnGUI(r, s.FindPropertyRelative(innerArrayRelativePath), g, datas);
        };
        arrayElementPropertyHeight = (s, e) =>
        {
            ItemData[] datas = ItemRegistry.GetItemsWithCategory(e);
            return innerArrayEditor.GetPropertyHeight(s.FindPropertyRelative(innerArrayRelativePath), datas);
        };

        innerArrayEditor.arrayElementLabel = (s, e) => new GUIContent(e.Name.ToString());
    }
    #endregion
}
