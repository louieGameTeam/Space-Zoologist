using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemID))]
public class ItemIDDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty category = property.FindPropertyRelative(nameof(category));
        SerializedProperty index = property.FindPropertyRelative(nameof(index));

        // Set height for only one control
        position.height = EditorExtensions.StandardControlHeight;

        // Add a foldout for the property
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;

        // If property is expanded the edit other fields
        if(property.isExpanded)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Edit category
            EditorGUI.PropertyField(position, category);
            position.y += position.height;

            // Layout the index field of the id drawer
            Popup(position, property, new GUIContent(index.displayName), (ItemRegistry.Category)category.enumValueIndex);

            // Decrease indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    #endregion

    #region Public Methods
    public static void Popup(Rect position, SerializedProperty property, GUIContent label, ItemRegistry.Category category)
    {
        Func<ItemID, bool> categoryFilter = id => id.Category == category;
        Popup(position, property, label, categoryFilter);
    }
    public static void Popup(Rect position, SerializedProperty property, GUIContent label, Func<ItemID, bool> filter)
    {
        // Get the relative properties of the id
        SerializedProperty category = property.FindPropertyRelative(nameof(category));
        SerializedProperty index = property.FindPropertyRelative(nameof(index));

        if (category != null && index != null)
        {
            // Get the property as an item id
            ItemID id = new ItemID(
                (ItemRegistry.Category)category.enumValueIndex, 
                index.intValue);

            // Get a list of the items that match the filter
            ItemID[] ids = ItemRegistry
                .GetAllItemIDs()
                .Where(filter)
                .ToArray();
            // Get a list of item names
            string[] itemNames = ids
                .Select(x =>
                {
                    if (x.Data.Name != null) return x.Data.Name.ToString();
                    else return "(no name)";
                }).ToArray();

            // Get the id's current index in the filtered array
            int filterIndex = Array.IndexOf(ids, id);

            // Get the index of the element in the filter 
            filterIndex = EditorExtensions.Popup(position, filterIndex, itemNames, label);

            // Assign the values of the item id back into the property
            id = ids[filterIndex];
            category.enumValueIndex = (int)id.Category;
            index.intValue = id.Index;
        }
        else if (category == null)
        {
            Debug.LogError($"Expected to find property at path {property.name}.{nameof(category)}, " +
                $"but no such property could be found");
        }
        else
        {
            Debug.LogError($"Expected to find property at path {property.name}.{nameof(index)}, " +
                $"but no such property could be found");
        }
    }
    #endregion
}
