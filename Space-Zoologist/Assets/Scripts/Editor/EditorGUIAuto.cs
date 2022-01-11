using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorGUIAuto
{
    #region Public Fields
    public static float SingleControlHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    #endregion

    #region Foldout
    public static bool Foldout(ref Rect position, bool foldout, string content, GUIStyle style = null)
    {
        return Foldout(ref position, foldout, content, false, style);
    }
    public static bool Foldout(ref Rect position, bool foldout, GUIContent content, GUIStyle style = null)
    {
        return Foldout(ref position, foldout, content, false, style);
    }
    public static bool Foldout(ref Rect position, bool foldout, string content, bool toggleOnLabelClick, GUIStyle style = null)
    {
        return Foldout(ref position, foldout, new GUIContent(content), toggleOnLabelClick, style);
    }
    public static bool Foldout(ref Rect position, bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style = null)
    {
        // Set style to default when null
        if (style == null) style = EditorStyles.foldout;

        position.height = SingleControlHeight;
        foldout = EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
        position.y += position.height;
        return foldout;
    }
    #endregion

    #region Int Slider
    public static void IntSlider(ref Rect position, SerializedProperty property, int leftValue, int rightValue)
    {
        IntSlider(ref position, property, leftValue, rightValue, property.displayName);
    }
    public static void IntSlider(ref Rect position, SerializedProperty property, int leftValue, int rightValue, string label)
    {
        IntSlider(ref position, property, leftValue, rightValue, new GUIContent(label));
    }
    public static void IntSlider(ref Rect position, SerializedProperty property, int leftValue, int rightValue, GUIContent label)
    {
        position.height = EditorGUI.GetPropertyHeight(property, true);
        EditorGUI.IntSlider(position, property, leftValue, rightValue, label);
        position.y += position.height;
    }
    #endregion

    #region Label Field
    public static void LabelField(ref Rect position, string label)
    {
        position.height = SingleControlHeight;
        EditorGUI.LabelField(position, label);
        position.y += position.height;
    }
    public static void PrefixedLabelField(ref Rect position, GUIContent prefix, string label)
    {
        position.height = SingleControlHeight;
        Rect prefixPosition = EditorGUI.PrefixLabel(position, prefix);

        // Reset indent so that prefix position is not pushed further over
        int oldIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Display the label and move the position down
        EditorGUI.LabelField(prefixPosition, label);

        // Restore the old indent
        EditorGUI.indentLevel = oldIndent;

        // Move the position down
        position.y += position.height;
    }
    #endregion

    #region Property Field
    public static bool PropertyField(ref Rect position, SerializedProperty property, bool includeChildren = false)
    {
        return PropertyField(ref position, property, new GUIContent(property.displayName), includeChildren);
    }
    public static bool PropertyField(ref Rect position, SerializedProperty property, GUIContent label, bool includeChildren = false)
    {
        position.height = EditorGUI.GetPropertyHeight(property, true);
        bool result = EditorGUI.PropertyField(position, property, label, includeChildren);
        position.y += position.height;
        return result;
    }
    #endregion
}
