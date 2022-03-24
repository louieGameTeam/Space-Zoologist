using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NeedData))]
public class NeedDataDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get ID and parent
        SerializedProperty parent = property.Copy();
        SerializedProperty id = property.FindPropertyRelative("id");

        // Go to the first boolean property
        property.Next(true);
        property.boolValue = EditorGUIAuto.ToggleLeft(ref position, label, property.boolValue);

        if (property.boolValue)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Go to the next property
            property.Next(false);

            // Get enumerable of all the children
            IEnumerable<SerializedProperty> children = EditorGUIAuto.ToEnd(property, id, false, false);

            // Iterate over all children
            foreach (SerializedProperty child in children)
            {
                // Check if we should indent this property
                bool indent = IndentProperty(parent, child);

                // If we should indent then increase the indent
                if (indent) EditorGUI.indentLevel++;

                // Only display the property if we are supposed to
                if (DisplayProperty(parent, child))
                {
                    EditorGUIAuto.PropertyField(ref position, child, true);
                }

                // If we indented before then restore normal indentation
                if (indent) EditorGUI.indentLevel--;
            }

            // Decrease indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Height starts at standard control
        float height = EditorExtensions.StandardControlHeight;

        // Get end property before iterating
        SerializedProperty parent = property.Copy();
        SerializedProperty id = property.FindPropertyRelative("id");

        // Go to first property
        property.Next(true);

        // If the need is needed then add the height for the children
        if (property.boolValue)
        {
            property.Next(false);

            // Get an iterator over all children
            IEnumerable<SerializedProperty> children = EditorGUIAuto.ToEnd(property, id, false, false);

            height += children
                .Where(child => DisplayProperty(parent, child))
                .Sum(child => EditorGUI.GetPropertyHeight(child, true));
        }

        return height;
    }
    #endregion

    #region Private Methods
    private static bool IndentProperty(SerializedProperty parent, SerializedProperty child)
    {
        ItemID itemID = GetID(parent);

        // Traversible-only field is conditional on "useAsTerrainNeed" for first water, so indent it
        return (itemID.IsWater && itemID.WaterIndex == 0 && child.name == "traversibleOnly") ||

            // Preferred field is conditional on "useAsTerrainNeed" for first water, so indent it
            (itemID.IsWater && itemID.WaterIndex == 0 && child.name == "preferred") ||

            // Preferred field is conditional on "useAsFoodNeed" for foods, so indent it
            (itemID.Category == ItemRegistry.Category.Food && child.name == "preferred") ||

            // Preferred tree field is conditional on "useAsTreeNeed", so indent it 
            (child.name == "preferredTree") ||

            // Minimum is conditional on "useAsWaterNeed" for first water, so indent it
            (itemID.IsWater && itemID.WaterIndex == 0 && child.name == "minimum") ||

            // Maximum is conditional on "useAsWaterNeed" for first water, so indent it
            (itemID.IsWater && itemID.WaterIndex == 0 && child.name == "maximum");
    }
    private static bool DisplayProperty(SerializedProperty parent, SerializedProperty child)
    {
        // Construct the item id
        ItemID itemID = GetID(parent);

        // Get some values to help with displaying other properties
        bool useAsTerrainNeed = parent.FindPropertyRelative(nameof(useAsTerrainNeed)).boolValue;
        bool useAsFoodNeed = parent.FindPropertyRelative(nameof(useAsFoodNeed)).boolValue;
        bool useAsTreeNeed = parent.FindPropertyRelative(nameof(useAsTreeNeed)).boolValue;
        bool useAsWaterNeed = parent.FindPropertyRelative(nameof(useAsWaterNeed)).boolValue;
        bool traversibleOnly = parent.FindPropertyRelative(nameof(traversibleOnly)).boolValue;
        bool isNonWaterTile = itemID.Category == ItemRegistry.Category.Tile && !itemID.IsWater;
        bool isFirstWater = itemID.IsWater && itemID.WaterIndex == 0;
        bool nonFirstWater = itemID.IsWater && itemID.WaterIndex != 0;

        // Display species need type for all species
        return (itemID.Category == ItemRegistry.Category.Species && child.name == "speciesNeedType") ||

            // Display "useAsTerrainNeed" only for the first water
            (isFirstWater && child.name == "useAsTerrainNeed") ||

            // Display "traversibleOnly" for any non water tile
            (isNonWaterTile && child.name == "traversibleOnly") ||

            // Display "traversibleOnly" for the first water if it is used as a terrain need
            (isFirstWater && useAsTerrainNeed && child.name == "traversibleOnly") ||

            // Display "useAsFoodNeed" for all food
            (itemID.Category == ItemRegistry.Category.Food && child.name == "useAsFoodNeed") ||

            // Display "preferred" only for foods used as food needs
            (itemID.Category == ItemRegistry.Category.Food && useAsFoodNeed && child.name == "preferred") ||

            // Display "preferred" for any non water tile that is not traversible only
            (isNonWaterTile && !traversibleOnly && child.name == "preferred") ||

            // Display "preferred" for first water tile only if it is used as a terrain need
            (isFirstWater && useAsTerrainNeed && !traversibleOnly && child.name == "preferred") ||

            // Display "useAsTreeNeed" for any food
            (itemID.Category == ItemRegistry.Category.Food && child.name == "useAsTreeNeed") ||

            // Display "preferredTree" only for food that is used as tree need
            (itemID.Category == ItemRegistry.Category.Food && useAsTreeNeed && child.name == "preferredTree") ||

            // Display "useAsWaterNeed" only for first water
            (isFirstWater && child.name == "useAsWaterNeed") ||

            // Display "minimum" for first water only if it is used as a water need
            (isFirstWater && useAsWaterNeed && child.name == "minimum") ||

            // Display "minimum" for any non-first water
            (nonFirstWater && child.name == "minimum") ||

            // Display "maximum" for first water only if it is used as a water need
            (isFirstWater && useAsWaterNeed && child.name == "maximum") ||

            // Display "maximum" for any non-first water
            (nonFirstWater && child.name == "maximum");
    }
    private static ItemID GetID(SerializedProperty needData)
    {
        // Get the category
        SerializedProperty id = needData.FindPropertyRelative(nameof(id));
        SerializedProperty category = id.FindPropertyRelative(nameof(category));
        ItemRegistry.Category categoryData = (ItemRegistry.Category)category.enumValueIndex;

        // Get the index
        SerializedProperty index = id.FindPropertyRelative(nameof(index));
        int indexData = index.intValue;

        // Construct the item id
        return new ItemID(categoryData, indexData);
    }
    #endregion
}
