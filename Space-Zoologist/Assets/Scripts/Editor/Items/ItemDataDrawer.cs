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
            SerializedProperty tile = property.FindPropertyRelative(nameof(tile));
            SerializedProperty species = property.FindPropertyRelative(nameof(species));
            SerializedProperty categoryFilter = property.FindPropertyRelative(nameof(categoryFilter));

            EditorGUIAuto.PropertyField(ref position, name, true);
            EditorGUIAuto.PropertyField(ref position, shopItem, true);

            if (categoryFilter.enumValueIndex == (int)ItemRegistry.Category.Species)
            {
                EditorGUIAuto.ObjectField(ref position, species, typeof(AnimalSpecies));
            }
            else if (categoryFilter.enumValueIndex == (int)ItemRegistry.Category.Food)
            {
                EditorGUIAuto.ObjectField(ref position, species, typeof(FoodSourceSpecies));
            }
            else if (categoryFilter.enumValueIndex == (int)ItemRegistry.Category.Tile)
            {
                EditorGUIAuto.PropertyField(ref position, tile, true);
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

            height += EditorGUI.GetPropertyHeight(name, true);
            height += EditorGUI.GetPropertyHeight(shopItem, true);

            // Add space for species object or tile type
            height += EditorGUIAuto.SingleControlHeight;
        }

        return height;
    }
}
