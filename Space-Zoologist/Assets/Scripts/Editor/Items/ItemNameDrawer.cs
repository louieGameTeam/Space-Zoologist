using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemName))]
public class ItemNameDrawer : PropertyDrawer
{
    #region Private Fields
    private ArrayOnEnumEditor<ItemName.Type> editor = new ArrayOnEnumEditor<ItemName.Type>();
    #endregion

    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        editor.OnGUI(position, property.FindPropertyRelative("names"));
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return editor.GetPropertyHeight(property.FindPropertyRelative("names"));
    }
    #endregion
}
