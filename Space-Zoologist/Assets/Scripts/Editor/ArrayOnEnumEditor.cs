using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class ArrayOnEnumEditor<TEnum> : ParallelArrayEditor<TEnum> where TEnum : System.Enum
{
    #region Custom Property Drawer Helpers
    public virtual void OnGUI(Rect position, SerializedProperty list, GUIContent label)
    {
        SetEnumValues(list);
        TEnum[] enums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
        base.OnGUI(position, list, label, enums);
    }
    public virtual void OnGUI(Rect position, SerializedProperty list)
    {
        OnGUI(position, list, new GUIContent(list.displayName));
    }
    public virtual float GetPropertyHeight(SerializedProperty array)
    {
        TEnum[] enums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
        return base.GetPropertyHeight(array, enums);
    }
    #endregion

    #region Custom Editor Helpers
    public virtual void OnInspectorGUI(SerializedProperty list)
    {
        SetEnumValues(list);
        TEnum[] enums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
        base.OnInspectorGUI(list, enums);
    }
    #endregion

    #region Private Methods
    private void SetEnumValues(SerializedProperty list)
    {
        if (list.isArray)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                SerializedProperty enumValue = element.FindPropertyRelative(nameof(element));

                // Set the enum value if this element has one
                if (enumValue != null)
                {
                    enumValue.enumValueIndex = i;
                }
            }
        }
        else throw PropertyIsNotArray(list);
    }
    #endregion
}
