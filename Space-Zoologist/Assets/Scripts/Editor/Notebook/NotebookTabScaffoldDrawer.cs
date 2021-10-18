using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NotebookTabScaffold))]
public class NotebookTabScaffoldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Set height for one control
        position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Put in a foldout for the property
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;

        if(property.isExpanded)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Get properties
            SerializedProperty enclosureScaffold = property.FindPropertyRelative(nameof(enclosureScaffold));
            SerializedProperty masks = property.FindPropertyRelative(nameof(masks));

            // Layout the enclosure scaffold and array fields
            NotebookEditorUtility.EnclosureScaffoldAndArrayField(position, enclosureScaffold, masks);

            // Restore indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Set height for just one control
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        // If property is expanded then add height of the sub properties
        if(property.isExpanded)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("enclosureScaffold"));
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("masks"));
        }
        
        return height;
    }
}
