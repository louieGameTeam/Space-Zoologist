using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EditArrayWrapperOnEnumAttribute))]
public class EditArrayWrapperOnEnumDrawer : WrappedPropertyDrawer
{
    #region Protected Properties
    protected string[] EnumNames
    {
        get
        {
            // Get the underlying attribute
            EditArrayWrapperOnEnumAttribute att = attribute as EditArrayWrapperOnEnumAttribute;
            // Get a list of the enum names
            string[] enumNames = System.Enum.GetNames(att.EnumType);
            return enumNames;
        }
    }
    #endregion

    #region Protected Fields
    protected ParallelArrayEditor<string> editor = new ParallelArrayEditor<string>();
    #endregion

    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SetEnumValues(WrappedArray(property));
        editor.OnGUI(position, WrappedArray(property), label, EnumNames);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return editor.GetPropertyHeight(WrappedArray(property), EnumNames);
    }
    public SerializedProperty WrappedArray(SerializedProperty property)
    {
        // Get the underlying attribute
        SerializedProperty wrappedArray = WrappedProperty(property);

        if (wrappedArray.isArray) return wrappedArray;
        else throw ParallelArrayEditor<string>.PropertyIsNotArray(wrappedArray);
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
                SerializedProperty enumValue = element.FindPropertyRelative(nameof(enumValue));

                // Set the enum value if this element has one
                if (enumValue != null)
                {
                    enumValue.enumValueIndex = i;
                }
            }
        }
        else throw ParallelArrayEditor<string>.PropertyIsNotArray(list);
    }
    #endregion
}
