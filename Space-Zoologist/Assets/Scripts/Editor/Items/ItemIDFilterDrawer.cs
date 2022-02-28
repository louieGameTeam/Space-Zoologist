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
        // Get the attribute filter
        ItemIDFilterAttribute filter = attribute as ItemIDFilterAttribute;

        // Do a popup
        ItemIDDrawer.Popup(position, property, label, filter.Filter);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorExtensions.StandardControlHeight;
    }
    #endregion
}
