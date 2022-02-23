using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UIBlockerMask))]
public class UIBlockerMaskDrawer : PropertyDrawer
{
    #region Private Fields
    private ParallelArrayEditor<string> editor = new ParallelArrayEditor<string>();
    #endregion

    #region Overridden Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty operationsBlocked = property.FindPropertyRelative(nameof(operationsBlocked));
        //Debug.Log($"Num operations blocked: {operationsBlocked.arraySize}. Num blockable operations: ")
        editor.OnGUI(position, operationsBlocked, label, UIBlockerSettings.BlockableOperations);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty operationsBlocked = property.FindPropertyRelative(nameof(operationsBlocked));
        return editor.GetPropertyHeight(operationsBlocked, UIBlockerSettings.BlockableOperations);
    }
    #endregion
}
