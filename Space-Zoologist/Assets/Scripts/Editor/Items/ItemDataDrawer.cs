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
            ItemName itemName = new ItemName();
            ItemName.Type[] nameTypes = (ItemName.Type[])System.Enum.GetValues(typeof(ItemName.Type));

            // Set each name in the temporary item name object
            for(int i = 0; i < array.arraySize; i++)
            {
                itemName.Set(nameTypes[i], array.GetArrayElementAtIndex(i).stringValue);
            }

            // Get the string version of the name
            label = new GUIContent(itemName.ToString());
        }

        // Put in the foldout field
        property.isExpanded = EditorGUIAuto.Foldout(ref position, property.isExpanded, label);

        // If the property is expanded then layout the others
        if (property.isExpanded)
        {
            // Get other important properties
            SerializedProperty name = property.FindPropertyRelative(nameof(name));
            SerializedProperty icon = property.FindPropertyRelative(nameof(icon));
            SerializedProperty shopItem = property.FindPropertyRelative(nameof(shopItem));
            SerializedProperty species = property.FindPropertyRelative(nameof(species));
            //SerializedProperty

            EditorGUIAuto.PropertyField(ref position, name, true);
            EditorGUIAuto.PropertyField(ref position, icon, true);
            EditorGUIAuto.PropertyField(ref position, shopItem, true);

            //if ()
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label);
    }
}
