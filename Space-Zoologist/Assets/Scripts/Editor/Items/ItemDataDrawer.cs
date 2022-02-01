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
            // Increase indent
            EditorGUI.indentLevel++;

            // Get other important properties
            SerializedProperty name = property.FindPropertyRelative(nameof(name));
            SerializedProperty shopItem = property.FindPropertyRelative(nameof(shopItem));
            SerializedProperty species = property.FindPropertyRelative(nameof(species));
            SerializedProperty hasSpecies = property.FindPropertyRelative(nameof(hasSpecies));
            SerializedProperty categoryFilter = property.FindPropertyRelative(nameof(categoryFilter));

            EditorGUIAuto.PropertyField(ref position, name, true);
            EditorGUIAuto.PropertyField(ref position, shopItem, true);

            // If this item data has species then layout the field for it
            if (hasSpecies.boolValue)
            {
                System.Type typeFilter = typeof(ScriptableObject);

                // Filter specific species based on the category filter
                if (categoryFilter.enumValueIndex == 0)
                {
                    typeFilter = typeof(AnimalSpecies);
                }
                else if (categoryFilter.enumValueIndex == 1)
                {
                    typeFilter = typeof(FoodSourceSpecies);
                }

                // Layout the object field
                EditorGUIAuto.ObjectField(ref position, species, typeFilter);
            }

            // Restore old indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Put in the foldout field
        float height = EditorGUIAuto.SingleControlHeight;

        // If the property is expanded then layout the others
        if (property.isExpanded)
        {
            // Get other important properties
            SerializedProperty name = property.FindPropertyRelative(nameof(name));
            SerializedProperty shopItem = property.FindPropertyRelative(nameof(shopItem));
            SerializedProperty species = property.FindPropertyRelative(nameof(species));
            SerializedProperty hasSpecies = property.FindPropertyRelative(nameof(hasSpecies));

            height += EditorGUI.GetPropertyHeight(name, true);
            height += EditorGUI.GetPropertyHeight(shopItem, true);

            // If this item data has species then layout the field for it
            if (hasSpecies.boolValue)
            {
                height += EditorGUI.GetPropertyHeight(species, true);
            }
        }

        return height;
    }
}
