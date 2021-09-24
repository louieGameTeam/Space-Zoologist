using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemData))]
public class ItemDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the array of names
        SerializedProperty array = property.FindPropertyRelative("name.names");

        // If there are items in the array, then use them to change the content label
        if (array.arraySize > 0)
        {
            // Create a temporary item name and set it up with the values in the serialized property
            ItemName name = new ItemName();
            ItemName.Type[] nameTypes = (ItemName.Type[])System.Enum.GetValues(typeof(ItemName.Type));

            // Set each name in the temporary item name object
            for(int i = 0; i < array.arraySize; i++)
            {
                name.Set(nameTypes[i], array.GetArrayElementAtIndex(i).stringValue);
            }

            // Get the string version of the name
            label = new GUIContent(name.ToString());
        }

        // Layout the property with the possibly modified label
        EditorGUI.PropertyField(position, property, label, true);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }
}
