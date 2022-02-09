using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemIDFilterAttribute))]
public class ItemIDFilterDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty category = property.FindPropertyRelative(nameof(category));

        if (category != null)
        {
            // Get the attribute filter
            ItemIDFilterAttribute filter = attribute as ItemIDFilterAttribute;

            // Set the category to the category in the filter
            category.enumValueIndex = (int)filter.Category;

            // Layout the index field
            ItemIDDrawer.IndexField(position, property, label);
        }
        else EditorGUI.PropertyField(position, property, label, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty category = property.FindPropertyRelative(nameof(category));

        // If the category exists this only has a height of one
        if (category != null) return EditorExtensions.StandardControlHeight;
        // If no category exists this must be some other property type
        else return EditorGUI.GetPropertyHeight(property, label, true);
    }
    #endregion
}
