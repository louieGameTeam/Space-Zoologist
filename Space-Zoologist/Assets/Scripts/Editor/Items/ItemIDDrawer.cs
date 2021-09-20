using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemID))]
public class ItemIDDrawer : PropertyDrawer
{
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

            // Get a list of the items and then a list of their names
            ItemData[] items = ItemRegistry.GetItemsWithCategory((ItemRegistry.Category)category.intValue);
            string[] itemNames = items.Select(x =>
            {
                if (x.Name != null) return x.Name.ToString();
                else return "(no name)";
            }).ToArray();

            // Edit the index as a popup that selects 
            index.intValue = EditorExtensions.Popup(position, index.intValue, itemNames, new GUIContent(index.displayName));

            // Decrease indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
