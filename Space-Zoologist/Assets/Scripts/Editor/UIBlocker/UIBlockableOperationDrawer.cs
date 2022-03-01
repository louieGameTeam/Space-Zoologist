using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UIBlockableOperationAttribute))]
public class UIBlockableOperationDrawer : PropertyDrawer
{
    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.Integer)
        {
            // Put in a prefix label
            position = EditorGUI.PrefixLabel(position, label);

            int selected;

            // Set the initially selected index based on the type of the property
            if (property.propertyType == SerializedPropertyType.String)
            {
                selected = Mathf.Max(UIBlockerSettings.IndexOf(property.stringValue), 0);
            }
            else selected = property.intValue;

            // Use a popup to update the selected value
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            selected = EditorGUI.Popup(position, selected, UIBlockerSettings.BlockableOperations);
            EditorGUI.indentLevel = oldIndent;

            // Read the result of the popup back into the property
            if (property.propertyType == SerializedPropertyType.String)
            {
                property.stringValue = UIBlockerSettings.GetBlockableOperation(selected);
            }
            else property.intValue = selected;
        }
        else EditorGUI.PropertyField(position, property, label, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.Integer)
        {
            return EditorExtensions.StandardControlHeight;
        }
        else return EditorGUI.GetPropertyHeight(property, label, true);
    }
    #endregion
}
