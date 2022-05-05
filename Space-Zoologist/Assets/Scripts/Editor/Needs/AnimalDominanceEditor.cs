using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimalDominance))]
public class AnimalDominanceEditor : Editor
{
    #region Editor Overrides
    public override void OnInspectorGUI()
    {
        // Get animal ids for use in each field
        ItemID[] animaIDs = ItemRegistry.GetItemIDsWithCategory(ItemRegistry.Category.Species);

        // Update the object
        serializedObject.Update();

        // Layout the terrain dominance
        EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainDominance"), true);

        // Get the food dominance field
        SerializedProperty foodDominance = serializedObject.FindProperty(nameof(foodDominance));

        // Food dominance should have exactly one entry for each animal in the item registry
        foodDominance.arraySize = animaIDs.Length;

        // Use a foldout for the food dominance
        foodDominance.isExpanded = EditorGUILayout.Foldout(foodDominance.isExpanded, foodDominance.displayName);
        
        if (foodDominance.isExpanded)
        {
            // Add indent
            EditorGUI.indentLevel++;

            // Layout each element with no way to edit the length
            for (int i = 0; i < foodDominance.arraySize; i++)
            {
                SerializedProperty element = foodDominance.GetArrayElementAtIndex(i);
                AnimalDominanceItemDrawer.SetID(element, animaIDs[i]);
                EditorGUILayout.PropertyField(element);
            }

            // Restore indent
            EditorGUI.indentLevel--;
        }

        // Apply modified properties to the object
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
