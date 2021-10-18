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
        TEnum[] enums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
        base.OnGUI(position, list, label, enums);
    }
    public virtual void OnGUI(Rect position, SerializedProperty list)
    {
        TEnum[] enums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
        base.OnGUI(position, list, enums);
    }
    #endregion

    #region Custom Editor Helpers
    public virtual void OnInspectorGUI(SerializedProperty list)
    {
        TEnum[] enums = (TEnum[])System.Enum.GetValues(typeof(TEnum));
        base.OnInspectorGUI(list, enums);
    }
    #endregion
}
