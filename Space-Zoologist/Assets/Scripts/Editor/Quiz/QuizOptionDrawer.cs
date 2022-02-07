using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(QuizOption))]
public class QuizOptionDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent content)
    {
        SerializedProperty label = property.FindPropertyRelative(nameof(label));
        SerializedProperty weight = property.FindPropertyRelative(nameof(weight));
        content = new GUIContent(QuizOption.ComputeDisplayName(label.stringValue, weight.intValue));
        EditorGUI.PropertyField(position, property, content, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    #endregion
}
