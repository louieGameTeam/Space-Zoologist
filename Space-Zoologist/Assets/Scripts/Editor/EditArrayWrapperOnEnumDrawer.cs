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
        else
        {
            Debug.LogError("EditArrayWrapperOnEnumDrawer: array wrapper at path '" +
                property.propertyPath + "' expected the relative property at path '" +
                wrappedArray.propertyPath + "' to be an array type, but instead it has the type '" +
                wrappedArray.propertyType + "'. Make sure that the relative property at path '" +
                wrappedArray.propertyType + "' is a type of array");
            throw new ExitGUIException();
        }
    }
    #endregion
}
