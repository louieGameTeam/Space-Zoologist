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

    #region Object Field
    public static void ObjectField(ref Rect position, SerializedProperty property, System.Type objType)
    {
        position.height = EditorGUI.GetPropertyHeight(property, true);
        EditorGUI.ObjectField(position, property, objType);
        position.y += position.height;
    }
    #endregion

    #region Property Field
    public static bool PropertyField(ref Rect position, SerializedProperty property, bool includeChildren = false)
    {
        position.height = EditorGUI.GetPropertyHeight(property, true);
        bool result = EditorGUI.PropertyField(position, property, includeChildren);
        position.y += position.height;
        return result;
    }
    public static bool PropertyField(ref Rect position, SerializedProperty property, GUIContent label, bool includeChildren = false)
    {
        position.height = EditorGUI.GetPropertyHeight(property, true);
        bool result = EditorGUI.PropertyField(position, property, label, includeChildren);
        position.y += position.height;
        return result;
    }
    #endregion

    #region Toggle Left
    public static bool ToggleLeft(ref Rect position, string label, bool value, GUIStyle labelStyle = null)
    {
        return ToggleLeft(ref position, label, value, labelStyle);
    }
    public static bool ToggleLeft(ref Rect position, GUIContent label, bool value, GUIStyle labelStyle = null)
    {
        if (labelStyle == null)
            labelStyle = EditorStyles.label;

        position.height = SingleControlHeight;
        bool result = EditorGUI.ToggleLeft(position, label, value, labelStyle);
        position.y += position.height;
        return result;
    }
    #endregion

    #region Custom Methods
    public static SerializedProperty[] GetArrayElements(SerializedProperty array)
    {
        // Check if array is null
        if (array == null) throw new System.ArgumentNullException(
            $"Argument {nameof(array)} cannot be null");

        // Check if this is an array
        if (!array.isArray) throw new System.ArgumentNullException(
            $"Property {array.name} is not an array. Please provide a property " +
            $"that is an arry");

        // Create the array
        SerializedProperty[] properties = new SerializedProperty[array.arraySize];

        // Load up each of the array elements
        for (int i = 0; i < array.arraySize; i++)
        {
            properties[i] = array.GetArrayElementAtIndex(i);
        }

        // Return the array
        return properties;
    }
    public static IEnumerable<SerializedProperty> ToEnd(SerializedProperty parent, string startFieldName, bool enterChildren, bool skipInvisible)
    {
        SerializedProperty start = parent.FindPropertyRelative(startFieldName);

        if (start != null) return ToEnd(start, enterChildren, skipInvisible);
        else throw new System.NullReferenceException($"Property {parent.name} " +
            $"has no serialized property at the relative path " +
            $"{parent.name}.{startFieldName}. Make sure the name " +
            $"of the relative path is correct " +
            $"and that the property at that path is serializable");
    }
    public static IEnumerable<SerializedProperty> ToEnd(SerializedProperty iterator, bool enterChildren, bool skipInvisible)
    {
        // Check if start is not null
        if (iterator != null)
        {
            bool hasNext = true;

            while (hasNext)
            {
                yield return iterator;

                // If we should skip invisible then go to the next visible property
                if (skipInvisible)
                {
                    hasNext = iterator.NextVisible(enterChildren);
                }
                // If we do not skip invisible then go to next property
                else hasNext = iterator.Next(enterChildren);
            }
        }
        else throw new System.ArgumentNullException(
            $"Parameter '{nameof(iterator)}' cannot be null");
    }
    public static IEnumerable<SerializedProperty> ToEnd(SerializedProperty parent, string startFieldName, string endFieldName, bool enterChildren, bool skipInvisible)
    {
        // Get start and end properties
        SerializedProperty start = parent.FindPropertyRelative(startFieldName);
        SerializedProperty end = parent.FindPropertyRelative(endFieldName);

        // If start and end are not null then invoke the function
        if (start != null && end != null)
        {
            // Advance to the property after the end
            end.Next(false);
            return ToEnd(start, end, enterChildren, skipInvisible);
        }

        // If start is null then throw exception
        else if (start == null) throw new System.NullReferenceException($"Property {parent.name} " +
            $"has no serialized property at the relative path " +
            $"{parent.name}.{startFieldName}. Make sure the name " +
            $"of the relative path is correct " +
            $"and that the property at that path is serializable");

        // If end is null then throw exception
        else throw new System.NullReferenceException($"Property {parent.name} " +
            $"has no serialized property at the relative path " +
            $"{parent.name}.{endFieldName}. Make sure the name " +
            $"of the relative path is correct " +
            $"and that the property at that path is serializable");
    }
    public static IEnumerable<SerializedProperty> ToEnd(SerializedProperty start, SerializedProperty end, bool enterChildren, bool skipInvisible)
    {
        if (start == null) throw new System.ArgumentNullException(
            $"Parameter '{nameof(start)}' cannot be null");

        if (end == null) throw new System.ArgumentNullException(
            $"Parameter '{nameof(end)}' cannot be null");

        // Store if the start has a next property
        bool hasNext = true;

        // Loop until start equals end or it has no next property
        while (start.propertyPath != end.propertyPath && hasNext)
        {
            yield return start;

            // If we should skip invisible then go to the next visible property
            if (skipInvisible)
            {
                hasNext = start.NextVisible(enterChildren);
            }
            // If we do not skip invisible then go to next property
            else hasNext = start.Next(enterChildren);
        }
    }
    #endregion
}
