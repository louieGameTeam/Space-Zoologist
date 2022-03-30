using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TerrainDominanceItem))]
public class TerrainDominanceItemDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get animal ids for use in each field
        ItemID[] animaIDs = ItemRegistry.GetItemIDsWithCategory(ItemRegistry.Category.Species);

        // Get the type for this tile
        SerializedProperty enumValue = property.FindPropertyRelative(nameof(enumValue));
        TileType tile = (TileType)enumValue.enumValueIndex;

        // Get the food dominance field
        SerializedProperty dominances = property.FindPropertyRelative(nameof(dominances));

        // Set the label
        label = new GUIContent($"{tile} {dominances.displayName}");

        // Food dominance should have exactly one entry for each animal in the item registry
        dominances.arraySize = animaIDs.Length;

        // Use a foldout for the food dominance
        dominances.isExpanded = EditorGUIAuto.Foldout(ref position, dominances.isExpanded, label);

        if (dominances.isExpanded)
        {
            // Add indent
            EditorGUI.indentLevel++;

            // Layout each element with no way to edit the length
            for (int i = 0; i < dominances.arraySize; i++)
            {
                // Set the id of the element
                SerializedProperty element = dominances.GetArrayElementAtIndex(i);
                AnimalDominanceItemDrawer.SetID(element, animaIDs[i]);

                // Disable dominances that cannot traverse this terrain
                ItemID itemID = AnimalDominanceItemDrawer.GetID(element);
                AnimalSpecies animal = itemID.Data.Species as AnimalSpecies;
                bool traversible = false;

                // Check if an animal species has been specified
                if (animal)
                {
                    traversible = animal.Needs.TerrainIsNeeded(tile);

                    // If this is not traversible then set the dominance to zero
                    if (!traversible)
                    {
                        SerializedProperty dominance = element.FindPropertyRelative(nameof(dominance));
                        dominance.floatValue = 0f;
                    }
                }

                GUI.enabled = traversible;
                EditorGUIAuto.PropertyField(ref position, element);
                GUI.enabled = true;
            }

            // Restore indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty dominances = property.FindPropertyRelative(nameof(dominances));
        float height = EditorExtensions.StandardControlHeight;

        if (dominances.isExpanded)
        {
            // Layout each element with no way to edit the length
            for (int i = 0; i < dominances.arraySize; i++)
            {
                SerializedProperty element = dominances.GetArrayElementAtIndex(i);
                height += EditorGUI.GetPropertyHeight(element, true);
            }
        }

        return height;
    }
    #endregion
}
