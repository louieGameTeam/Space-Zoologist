using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemRegistry))]
public class ItemRegistryEditor : Editor
{
    #region Override Methods
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        // Get the item data
        SerializedProperty itemData = serializedObject.FindProperty(nameof(itemData));
        int numCategories = System.Enum.GetNames(typeof(ItemRegistry.Category)).Length;

        // Loop over all categories
        for (int i = 0; i < numCategories; i++)
        {
            // Get the array of items for this category
            SerializedProperty items = itemData
                .FindPropertyRelative("itemDataLists")
                .GetArrayElementAtIndex(i)
                .FindPropertyRelative("items");

            // Go through each item data in the array
            for (int j = 0; j < items.arraySize; j++)
            {
                SerializedProperty hasSpecies = items
                    .GetArrayElementAtIndex(j)
                    .FindPropertyRelative("hasSpecies");

                // This has species if the category is not tile
                hasSpecies.boolValue = i < 2;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
