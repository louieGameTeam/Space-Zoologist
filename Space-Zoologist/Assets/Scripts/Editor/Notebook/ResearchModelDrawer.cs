using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ResearchModel))]
public class ResearchModelDrawer : PropertyDrawer
{
    #region Private Fields
    private ItemArrayEditor arrayEditor = new ItemArrayEditor("entries");
    #endregion

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        arrayEditor.OnGUI(position, property.FindPropertyRelative("entryLists"), label);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return arrayEditor.GetPropertyHeight(property.FindPropertyRelative("entryLists"));
    }
}
