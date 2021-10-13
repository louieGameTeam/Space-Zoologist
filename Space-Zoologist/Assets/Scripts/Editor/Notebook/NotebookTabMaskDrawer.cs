using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NotebookTabMask))]
public class NotebookTabMaskDrawer : PropertyDrawer
{
    #region Private Fields
    private ArrayOnEnumEditor<NotebookTab> editor = new ArrayOnEnumEditor<NotebookTab>();
    #endregion

    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        editor.OnGUI(position, property.FindPropertyRelative("mask"));
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return editor.GetPropertyHeight(property.FindPropertyRelative("mask"));
    }
    #endregion
}
