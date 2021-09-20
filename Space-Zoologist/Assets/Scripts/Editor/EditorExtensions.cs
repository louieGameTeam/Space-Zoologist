using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorExtensions
{
    #region Public Properties
    public static float StandardControlHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    #endregion

    #region Public Methods
    public static int Popup(Rect position, int index, string[] values, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, label);

        // Wipe indent so that rects are property placed
        int oldIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        if (values.Length > 0)
        {
            // Make sure index is not an invalid value
            index = Mathf.Clamp(index, 0, values.Length);
            index = EditorGUI.Popup(position, index, values);
        }
        else
        {
            EditorGUI.LabelField(position, "Nothing to select");
            index = -1;
        }

        // Restore the old indent
        EditorGUI.indentLevel = oldIndent;

        return index;
    }
    #endregion
}
