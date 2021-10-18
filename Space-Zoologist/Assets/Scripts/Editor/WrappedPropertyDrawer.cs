using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(WrappedPropertyAttribute))]
public class WrappedPropertyDrawer : PropertyDrawer
{
    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, WrappedProperty(property), label, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(WrappedProperty(property), label, true);
    }
    public SerializedProperty WrappedProperty(SerializedProperty property)
    {
        // Get the underlying attribute
        WrappedPropertyAttribute att = attribute as WrappedPropertyAttribute;
        SerializedProperty wrappedProperty = property.FindPropertyRelative(att.WrappedPropertyPath);

        if (wrappedProperty != null) return wrappedProperty;
        else
        {
            Debug.LogError("WrappedPropertyDrawer: wrapper property at path '" +
                property.propertyPath + "' expected to find wrapped property at path '" +
                property.propertyPath + "." + att.WrappedPropertyPath +
                "' but no such property could be found.  Make sure that the 'wrappedPropertyPath' field " +
                "on the attribute applied to this wrapper points to a property within the wrapper");
            throw new ExitGUIException();
        }
    }

    #endregion
}
